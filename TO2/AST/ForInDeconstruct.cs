using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class ForInDeconstruct(
    List<DeclarationParameter> declarations,
    Expression sourceExpression,
    Expression loopExpression,
    Position start = new(),
    Position end = new())
    : Expression(start,
        end), IVariableContainer {
    public override IVariableContainer? VariableContainer {
        set {
            ParentContainer = value;
            sourceExpression.VariableContainer = this;
            loopExpression.VariableContainer = this;
        }
    }

    public IVariableContainer? ParentContainer { get; private set; }

    public TO2Type? FindVariableLocal(IBlockContext context, string name) {
        for (var i = 0; i < declarations.Count; i++) {
            var declaration = declarations[i];

            if (declaration.IsPlaceholder || name != declaration.target) continue;
            if (declaration.type != null) return declaration.type;

            var elementType = sourceExpression.ResultType(context).ForInSource(context.ModuleContext, null)?.ElementType;
            if (elementType == null) return null;
            switch (elementType) {
            case TupleType tupleType:
                return i < tupleType.itemTypes.Count ? tupleType.itemTypes[i] : null;
            case RecordType recordType:
                return recordType.ItemTypes!.Get(declaration.source);
            }
        }

        return null;
    }

    public override TO2Type ResultType(IBlockContext context) => BuiltinType.Unit;

    public override void Prepare(IBlockContext context) {
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        var sourceType = sourceExpression.ResultType(context).UnderlyingType(context.ModuleContext);
        var source = sourceType.ForInSource(context.ModuleContext, null);

        if (source == null)
            context.AddError(
                new StructuralError(
                    StructuralError.ErrorType.InvalidType,
                    $"{sourceType} cannot be use as for ... in source",
                    Start,
                    End
                )
            );
        foreach (var declaration in declarations)
            if (context.FindVariable(declaration.target!) != null)
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.DuplicateVariableName,
                    $"Variable '{declaration.target}' already declared in this scope",
                    Start,
                    End
                ));

        if (context.HasErrors) return;

        switch (source!.ElementType) {
        case TupleType tupleType:
            EmitCodeTuple(context, source, tupleType);
            return;
        case RecordType recordType:
            EmitCodeRecord(context, source, recordType);
            return;
        default:
            context.AddError(new StructuralError(
                StructuralError.ErrorType.InvalidType,
                $"Expected source of for loop to be an array of tuple or record, but got {sourceType}",
                Start,
                End
            ));
            return;
        }
    }

    private void EmitCodeTuple(IBlockContext context, IForInSource source, TupleType tupleType) {
        if (tupleType.itemTypes.Count != declarations.Count) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.InvalidType,
                $"Expected right side to be a tuple with {declarations.Count} elements, but got {tupleType}",
                Start,
                End
            ));
            return;
        }

        for (var i = 0; i < declarations.Count; i++) {
            var declaration = declarations[i];

            if (declaration.IsPlaceholder) continue;

            var variableType = declaration.IsInferred ? tupleType.itemTypes[i] : declaration.type;

            if (context.FindVariable(declaration.target!) != null) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.DuplicateVariableName,
                    $"Variable '{declaration.target}' already declared in this scope",
                    Start,
                    End
                ));
                return;
            }

            if (!variableType!.IsAssignableFrom(context.ModuleContext, tupleType.itemTypes[i])) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"Expected element {i} of {tupleType} to be of type {variableType}",
                    Start,
                    End
                ));
                return;
            }
        }


        var loopSize = EstimateLoopTuple(context, source, tupleType);
        var start = context.IL.DefineLabel(loopSize.opCodes < 124);
        var end = context.IL.DefineLabel(loopSize.opCodes < 124);
        var loop = context.IL.DefineLabel(loopSize.opCodes < 124);

        var loopContext = context.CreateLoopContext(start, end);
        var variables = DeclareLoopVariablesTuple(loopContext, tupleType);

        sourceExpression.EmitCode(context, false);

        if (context.HasErrors) return;

        source.EmitInitialize(loopContext);
        loopContext.IL.Emit(start.isShort ? OpCodes.Br_S : OpCodes.Br, start);
        loopContext.IL.MarkLabel(loop);
        source.EmitNext(loopContext);
        foreach (var (index, variable) in variables) {
            loopContext.IL.Emit(OpCodes.Dup);
            tupleType.FindField(loopContext.ModuleContext, $"_{index + 1}")!.Create(loopContext.ModuleContext)
                .EmitLoad(loopContext);

            variable.EmitStore(loopContext);
        }

        loopContext.IL.Emit(OpCodes.Pop);

        loopExpression.EmitCode(loopContext, true);
        loopContext.IL.MarkLabel(start);
        source.EmitCheckDone(loopContext, loop);
        loopContext.IL.MarkLabel(end);
    }

    private ILCount EstimateLoopTuple(IBlockContext context, IForInSource source, TupleType tupleType) {
        var prepContext = context.CloneCountingContext();

        sourceExpression.EmitCode(prepContext, false);
        source.EmitInitialize(prepContext);

        var countingContext = prepContext.CloneCountingContext()
            .CreateLoopContext(context.IL.DefineLabel(false), context.IL.DefineLabel(false));
        var
            variables = DeclareLoopVariablesTuple(countingContext, tupleType);
        var loop = countingContext.IL.DefineLabel(false);

        source.EmitNext(countingContext);
        foreach (var (index, variable) in variables) {
            countingContext.IL.Emit(OpCodes.Dup);
            tupleType.FindField(context.ModuleContext, $"_{index + 1}")!.Create(countingContext.ModuleContext)
                .EmitLoad(countingContext);

            variable.EmitStore(countingContext);
        }

        countingContext.IL.Emit(OpCodes.Pop);

        loopExpression.EmitCode(countingContext, true);
        source.EmitCheckDone(countingContext, loop);

        return new ILCount {
            opCodes = countingContext.IL.ILSize,
            stack = countingContext.IL.StackCount
        };
    }

    private List<(int index, IBlockVariable variable)> DeclareLoopVariablesTuple(IBlockContext loopContext,
        TupleType tupleType) {
        var variables = new List<(int index, IBlockVariable variable)>();

        for (var i = 0; i < declarations.Count; i++) {
            var declaration = declarations[i];

            if (declaration.IsPlaceholder) continue;
            if (declaration.IsInferred)
                variables.Add((i,
                    loopContext.DeclaredVariable(declaration.target!, true,
                        tupleType.itemTypes[i].UnderlyingType(loopContext.ModuleContext))));
            else
                variables.Add((i,
                    loopContext.DeclaredVariable(declaration.target!, true,
                        declaration.type!.UnderlyingType(loopContext.ModuleContext))));
        }

        return variables;
    }

    private void EmitCodeRecord(IBlockContext context, IForInSource source, RecordType recordType) {
        foreach (var declaration in declarations) {
            if (declaration.IsPlaceholder) continue;

            if (!recordType.ItemTypes.ContainsKey(declaration.source!)) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"{recordType} does not have a field '{declaration.source}'",
                    Start,
                    End
                ));
                return;
            }

            var variableType =
                declaration.IsInferred ? recordType.ItemTypes[declaration.source!] : declaration.type;

            if (context.FindVariable(declaration.target!) != null) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.DuplicateVariableName,
                    $"Variable '{declaration.target}' already declared in this scope",
                    Start,
                    End
                ));
                return;
            }

            if (!variableType!.IsAssignableFrom(context.ModuleContext, recordType.ItemTypes[declaration.source!])) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"Expected element {declaration.source} of {recordType} to be of type {variableType}",
                    Start,
                    End
                ));
                return;
            }
        }

        var loopSize = EstimateLoopRecord(context, source, recordType);
        var start = context.IL.DefineLabel(loopSize.opCodes < 124);
        var end = context.IL.DefineLabel(loopSize.opCodes < 124);
        var loop = context.IL.DefineLabel(loopSize.opCodes < 124);

        var loopContext = context.CreateLoopContext(start, end);
        List<(string name, IBlockVariable variable)>
            variables = DeclareLoopVariablesRecord(loopContext, recordType);

        sourceExpression.EmitCode(context, false);

        if (context.HasErrors) return;

        source.EmitInitialize(loopContext);
        loopContext.IL.Emit(start.isShort ? OpCodes.Br_S : OpCodes.Br, start);
        loopContext.IL.MarkLabel(loop);
        source.EmitNext(loopContext);
        foreach (var (name, variable) in variables) {
            loopContext.IL.Emit(OpCodes.Dup);
            recordType.FindField(loopContext.ModuleContext, name)!.Create(loopContext.ModuleContext)
                .EmitLoad(loopContext);

            variable.EmitStore(loopContext);
        }

        loopContext.IL.Emit(OpCodes.Pop);

        loopExpression.EmitCode(loopContext, true);
        loopContext.IL.MarkLabel(start);
        source.EmitCheckDone(loopContext, loop);
        loopContext.IL.MarkLabel(end);
    }

    private ILCount EstimateLoopRecord(IBlockContext context, IForInSource source, RecordType recordType) {
        var prepContext = context.CloneCountingContext();

        sourceExpression.EmitCode(prepContext, false);
        source.EmitInitialize(prepContext);

        var countingContext = prepContext.CloneCountingContext()
            .CreateLoopContext(context.IL.DefineLabel(false), context.IL.DefineLabel(false));
        List<(string name, IBlockVariable variable)> variables =
            DeclareLoopVariablesRecord(countingContext, recordType);
        var loop = countingContext.IL.DefineLabel(false);

        source.EmitNext(countingContext);
        foreach (var (name, variable) in variables) {
            countingContext.IL.Emit(OpCodes.Dup);
            recordType.FindField(countingContext.ModuleContext, name)!.Create(countingContext.ModuleContext)
                .EmitLoad(countingContext);

            variable.EmitStore(countingContext);
        }

        countingContext.IL.Emit(OpCodes.Pop);

        loopExpression.EmitCode(countingContext, true);
        source.EmitCheckDone(countingContext, loop);

        return new ILCount {
            opCodes = countingContext.IL.ILSize,
            stack = countingContext.IL.StackCount
        };
    }

    private List<(string field, IBlockVariable variable)> DeclareLoopVariablesRecord(IBlockContext loopContext,
        RecordType recordType) {
        var variables =
            new List<(string field, IBlockVariable variable)>();

        foreach (var declaration in declarations) {
            if (declaration.IsPlaceholder) continue;
            if (declaration.IsInferred)
                variables.Add((declaration.target!,
                    loopContext.DeclaredVariable(declaration.target!, true,
                        recordType.ItemTypes[declaration.source!].UnderlyingType(loopContext.ModuleContext))));
            else
                variables.Add((declaration.target!,
                    loopContext.DeclaredVariable(declaration.target!, true,
                        declaration.type!.UnderlyingType(loopContext.ModuleContext))));
        }

        return variables;
    }

    public override REPLValueFuture Eval(REPLContext context) {
        throw new REPLException(this, "Not supported in REPL mode");
    }
}
