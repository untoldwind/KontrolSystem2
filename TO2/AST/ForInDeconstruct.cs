using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class ForInDeconstruct : Expression, IVariableContainer {
        private readonly List<DeclarationParameter> declarations;
        private readonly Expression sourceExpression;
        private readonly Expression loopExpression;

        private IVariableContainer parentContainer;

        public ForInDeconstruct(List<DeclarationParameter> declarations, Expression sourceExpression,
            Expression loopExpression, Position start = new Position(), Position end = new Position()) : base(start,
            end) {
            this.declarations = declarations;
            this.sourceExpression = sourceExpression;
            this.loopExpression = loopExpression;
        }

        public IVariableContainer ParentContainer => parentContainer;

        public TO2Type FindVariableLocal(IBlockContext context, string name) {
            for (int i = 0; i < declarations.Count; i++) {
                DeclarationParameter declaration = declarations[i];

                if (declaration.IsPlaceholder || name != declaration.target) continue;
                if (declaration.type != null) return declaration.type;

                RealizedType elementType = sourceExpression.ResultType(context)
                    ?.ForInSource(context.ModuleContext, null).ElementType;
                if (elementType == null) return null;
                switch (elementType) {
                case TupleType tupleType:
                    return i < tupleType.itemTypes.Count ? tupleType.itemTypes[i] : null;
                case RecordType recordType:
                    return recordType.ItemTypes.Get(declaration.source);
                }
            }

            return null;
        }

        public override IVariableContainer VariableContainer {
            set {
                parentContainer = value;
                sourceExpression.VariableContainer = this;
                loopExpression.VariableContainer = this;
            }
        }

        public override TO2Type ResultType(IBlockContext context) => BuiltinType.Unit;

        public override void Prepare(IBlockContext context) {
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            RealizedType sourceType = sourceExpression.ResultType(context).UnderlyingType(context.ModuleContext);
            IForInSource source = sourceType.ForInSource(context.ModuleContext, null);

            if (source == null)
                context.AddError(
                    new StructuralError(
                        StructuralError.ErrorType.InvalidType,
                        $"{sourceType} cannot be use as for ... in source",
                        Start,
                        End
                    )
                );
            foreach (DeclarationParameter declaration in declarations)
                if (context.FindVariable(declaration.target) != null)
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
                    $"Expected source of for loop to be an array of tupple or record, but got {sourceType}",
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

            for (int i = 0; i < declarations.Count; i++) {
                DeclarationParameter declaration = declarations[i];

                if (declaration.IsPlaceholder) continue;

                TO2Type variableType = declaration.IsInferred ? tupleType.itemTypes[i] : declaration.type;

                if (context.FindVariable(declaration.target) != null) {
                    context.AddError(new StructuralError(
                        StructuralError.ErrorType.DuplicateVariableName,
                        $"Variable '{declaration.target}' already declared in this scope",
                        Start,
                        End
                    ));
                    return;
                }

                if (!variableType.IsAssignableFrom(context.ModuleContext, tupleType.itemTypes[i])) {
                    context.AddError(new StructuralError(
                        StructuralError.ErrorType.IncompatibleTypes,
                        $"Expected element {i} of {tupleType} to be of type {variableType}",
                        Start,
                        End
                    ));
                    return;
                }
            }


            ILCount loopSize = EstimateLoopTuple(context, source, tupleType);
            LabelRef start = context.IL.DefineLabel(loopSize.opCodes < 124);
            LabelRef end = context.IL.DefineLabel(loopSize.opCodes < 124);
            LabelRef loop = context.IL.DefineLabel(loopSize.opCodes < 124);

            IBlockContext loopContext = context.CreateLoopContext(start, end);
            List<(int index, IBlockVariable variable)> variables = DeclareLoopVariablesTuple(loopContext, tupleType);

            sourceExpression.EmitCode(context, false);

            if (context.HasErrors) return;

            source.EmitInitialize(loopContext);
            loopContext.IL.Emit(start.isShort ? OpCodes.Br_S : OpCodes.Br, start);
            loopContext.IL.MarkLabel(loop);
            source.EmitNext(loopContext);
            foreach (var kv in variables) {
                loopContext.IL.Emit(OpCodes.Dup);
                tupleType.FindField(loopContext.ModuleContext, $"_{kv.index + 1}").Create(loopContext.ModuleContext)
                    .EmitLoad(loopContext);

                kv.variable.EmitStore(loopContext);
            }

            loopContext.IL.Emit(OpCodes.Pop);

            loopExpression.EmitCode(loopContext, true);
            loopContext.IL.MarkLabel(start);
            source.EmitCheckDone(loopContext, loop);
            loopContext.IL.MarkLabel(end);
        }

        private ILCount EstimateLoopTuple(IBlockContext context, IForInSource source, TupleType tupleType) {
            IBlockContext prepContext = context.CloneCountingContext();

            sourceExpression.EmitCode(prepContext, false);
            source.EmitInitialize(prepContext);

            IBlockContext countingContext = prepContext.CloneCountingContext()
                .CreateLoopContext(context.IL.DefineLabel(false), context.IL.DefineLabel(false));
            List<(int index, IBlockVariable variable)>
                variables = DeclareLoopVariablesTuple(countingContext, tupleType);
            LabelRef loop = countingContext.IL.DefineLabel(false);

            source.EmitNext(countingContext);
            foreach (var kv in variables) {
                countingContext.IL.Emit(OpCodes.Dup);
                tupleType.FindField(context.ModuleContext, $"_{kv.index + 1}").Create(countingContext.ModuleContext)
                    .EmitLoad(countingContext);

                kv.variable.EmitStore(countingContext);
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
            List<(int index, IBlockVariable variable)> variables = new List<(int index, IBlockVariable variable)>();

            for (int i = 0; i < declarations.Count; i++) {
                DeclarationParameter declaration = declarations[i];

                if (declaration.IsPlaceholder) continue;
                if (declaration.IsInferred)
                    variables.Add((i,
                        loopContext.DeclaredVariable(declaration.target, true,
                            tupleType.itemTypes[i].UnderlyingType(loopContext.ModuleContext))));
                else
                    variables.Add((i,
                        loopContext.DeclaredVariable(declaration.target, true,
                            declaration.type.UnderlyingType(loopContext.ModuleContext))));
            }

            return variables;
        }

        private void EmitCodeRecord(IBlockContext context, IForInSource source, RecordType recordType) {
            foreach (DeclarationParameter declaration in declarations) {
                if (declaration.IsPlaceholder) continue;

                if (!recordType.ItemTypes.ContainsKey(declaration.source)) {
                    context.AddError(new StructuralError(
                        StructuralError.ErrorType.IncompatibleTypes,
                        $"{recordType} does not have a field '{declaration.source}'",
                        Start,
                        End
                    ));
                    return;
                }

                TO2Type variableType =
                    declaration.IsInferred ? recordType.ItemTypes[declaration.source] : declaration.type;

                if (context.FindVariable(declaration.target) != null) {
                    context.AddError(new StructuralError(
                        StructuralError.ErrorType.DuplicateVariableName,
                        $"Variable '{declaration.target}' already declared in this scope",
                        Start,
                        End
                    ));
                    return;
                }

                if (!variableType.IsAssignableFrom(context.ModuleContext, recordType.ItemTypes[declaration.source])) {
                    context.AddError(new StructuralError(
                        StructuralError.ErrorType.IncompatibleTypes,
                        $"Expected element {declaration.source} of {recordType} to be of type {variableType}",
                        Start,
                        End
                    ));
                    return;
                }
            }

            ILCount loopSize = EstimateLoopRecord(context, source, recordType);
            LabelRef start = context.IL.DefineLabel(loopSize.opCodes < 124);
            LabelRef end = context.IL.DefineLabel(loopSize.opCodes < 124);
            LabelRef loop = context.IL.DefineLabel(loopSize.opCodes < 124);

            IBlockContext loopContext = context.CreateLoopContext(start, end);
            List<(string name, IBlockVariable variable)>
                variables = DeclareLoopVariablesRecord(loopContext, recordType);

            sourceExpression.EmitCode(context, false);

            if (context.HasErrors) return;

            source.EmitInitialize(loopContext);
            loopContext.IL.Emit(start.isShort ? OpCodes.Br_S : OpCodes.Br, start);
            loopContext.IL.MarkLabel(loop);
            source.EmitNext(loopContext);
            foreach (var kv in variables) {
                loopContext.IL.Emit(OpCodes.Dup);
                recordType.FindField(loopContext.ModuleContext, kv.name).Create(loopContext.ModuleContext)
                    .EmitLoad(loopContext);

                kv.variable.EmitStore(loopContext);
            }

            loopContext.IL.Emit(OpCodes.Pop);

            loopExpression.EmitCode(loopContext, true);
            loopContext.IL.MarkLabel(start);
            source.EmitCheckDone(loopContext, loop);
            loopContext.IL.MarkLabel(end);
        }

        private ILCount EstimateLoopRecord(IBlockContext context, IForInSource source, RecordType recordType) {
            IBlockContext prepContext = context.CloneCountingContext();

            sourceExpression.EmitCode(prepContext, false);
            source.EmitInitialize(prepContext);

            IBlockContext countingContext = prepContext.CloneCountingContext()
                .CreateLoopContext(context.IL.DefineLabel(false), context.IL.DefineLabel(false));
            List<(string name, IBlockVariable variable)> variables =
                DeclareLoopVariablesRecord(countingContext, recordType);
            LabelRef loop = countingContext.IL.DefineLabel(false);

            source.EmitNext(countingContext);
            foreach (var kv in variables) {
                countingContext.IL.Emit(OpCodes.Dup);
                recordType.FindField(countingContext.ModuleContext, kv.name).Create(countingContext.ModuleContext)
                    .EmitLoad(countingContext);

                kv.variable.EmitStore(countingContext);
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
            List<(string field, IBlockVariable variable)> variables =
                new List<(string field, IBlockVariable variable)>();

            foreach (DeclarationParameter declaration in declarations) {
                if (declaration.IsPlaceholder) continue;
                if (declaration.IsInferred)
                    variables.Add((declaration.target,
                        loopContext.DeclaredVariable(declaration.target, true,
                            recordType.ItemTypes[declaration.source].UnderlyingType(loopContext.ModuleContext))));
                else
                    variables.Add((declaration.target,
                        loopContext.DeclaredVariable(declaration.target, true,
                            declaration.type.UnderlyingType(loopContext.ModuleContext))));
            }

            return variables;
        }
    }
}
