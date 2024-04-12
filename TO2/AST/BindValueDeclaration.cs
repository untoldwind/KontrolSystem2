using System;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST;

public class BindValueDeclaration : Node, IBlockItem, IVariableRef {
    private readonly string name;
    private readonly Expression expression;
    private bool lookingUp;
    private LambdaClass? lambdaClass;

    public BindValueDeclaration(string name, Expression expression, Position start, Position end) : base(start, end) {
        this.name = name;
        this.expression = expression;
    }

    public bool IsComment => false;

    public IVariableContainer VariableContainer {
        set => expression.VariableContainer = value;
    }

    public TypeHint? TypeHint {
        set { }
    }

    public TO2Type ResultType(IBlockContext context) => BuiltinType.Unit;

    public void Prepare(IBlockContext context) {
    }

    public void EmitCode(IBlockContext context, bool dropResult) {
        var valueType = expression.ResultType(context);
        var variableType = new BoundValueType(valueType);

        if (context.HasErrors) return;

        if (context.FindVariable(name) != null) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.DuplicateVariableName,
                $"Variable '{name}' already declared in this scope",
                Start,
                End
            ));
            return;
        }

        var variable = context.DeclaredVariable(name, true,
            variableType.UnderlyingType(context.ModuleContext));

        var lambdaType = new FunctionType(false, [], valueType);
        lambdaClass ??= LambdaClass.CreateLambdaClass(context, lambdaType, [], expression, Start, End);

        foreach (var (sourceName, _) in lambdaClass.Value.clonedVariables) {
            var source = context.FindVariable(sourceName)!;
            source.EmitLoad(context);
        }

        context.IL.EmitNew(OpCodes.Newobj, lambdaClass.Value.constructor, lambdaClass.Value.clonedVariables.Count);
        context.IL.EmitPtr(OpCodes.Ldftn, lambdaClass.Value.lambdaImpl);
        context.IL.EmitNew(OpCodes.Newobj, lambdaType.GeneratedType(context.ModuleContext)
                .GetConstructor([typeof(object), typeof(IntPtr)])!);

        variable.EmitStore(context);
    }

    public string Name => name;

    public TO2Type? VariableType(IBlockContext context) {
        if (lookingUp) return null;
        lookingUp = true; // Somewhat ugly workaround if there is a cycle in inferred variables that should produce a correct error message
        var type = expression.ResultType(context);
        lookingUp = false;
        return new BoundValueType(type);
    }
}
