using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST;

public class ImplOperatorsDeclaration(
    string name,
    List<IEither<LineComment, FunctionDeclaration>> operators,
    Position start = new(),
    Position end = new()) : Node(start, end), IModuleItem {
    private static readonly Dictionary<string, Operator> unaryOperatorMap = new() {
        { "unary_minus", Operator.Neg }
    };

    private static readonly Dictionary<string, Operator> binaryOperatorMap = new() {
        { "add", Operator.Add },
        { "sub", Operator.Sub },
        { "mul", Operator.Mul },
        { "div", Operator.Div },
        { "mod", Operator.Mod },
    };

    private List<DeclaredOperatorEmitter> operatorFunctions = [];

    public IEnumerable<StructuralError> TryDeclareTypes(ModuleContext context) => [];

    public IEnumerable<StructuralError> TryImportTypes(ModuleContext context) => [];

    public IEnumerable<StructuralError> TryImportConstants(ModuleContext context) {
        var structDelegate = context.mappedTypes.Get(name) as StructTypeAliasDelegate;

        if (structDelegate == null)
            return [
                new StructuralError(
                    StructuralError.ErrorType.InvalidType,
                    $"Struct with name {name} is not defined",
                    Start,
                    End)
            ];

        var underlyingType = structDelegate.UnderlyingType(context);
        var errors = new List<StructuralError>();

        foreach (var declaredOp in operators) {
            if (declaredOp.IsRight) {
                var function = declaredOp.Right;
                if (function.isAsync) {
                    errors.Add(new StructuralError(StructuralError.ErrorType.InvalidOperator,
                        $"Operator function {function.name} has to be sync", function.Start, function.End));
                    continue;
                }

                Operator op;
                var isUnary = unaryOperatorMap.TryGetValue(function.name, out op);
                var isBinary = !isUnary && binaryOperatorMap.TryGetValue(function.name, out op);

                if (!isUnary && !isBinary) {
                    errors.Add(new StructuralError(StructuralError.ErrorType.InvalidOperator,
                        $"Operator function {function.name} is not in list of allowed operators [{string.Join(", ", unaryOperatorMap.Keys.Concat(binaryOperatorMap.Keys))}]",
                        function.Start, function.End));
                    continue;
                }
                if (isUnary) {
                    if (function.parameters.Count != 1) {
                        errors.Add(new StructuralError(StructuralError.ErrorType.InvalidOperator,
                            $"Unary operator {function.name} must have exactly 1 parameters", function.Start,
                            function.End));
                        continue;
                    }

                    if (function.parameters[0].type?.UnderlyingType(context) != underlyingType) {
                        errors.Add(new StructuralError(StructuralError.ErrorType.InvalidType,
                            $"Parameter of unary operator {function.name} must be of type {structDelegate.Name}",
                            function.Start,
                            function.End));
                        continue;
                    }

                    if (underlyingType.AllowedPrefixOperators(context).GetMatching(context, op, BuiltinType.Unit) != null) {
                        errors.Add(new StructuralError(StructuralError.ErrorType.InvalidType,
                            $"Unary operator {function.name} already defined on type {structDelegate.Name}",
                            function.Start,
                            function.End));
                        continue;
                    }
                    var methodContext = context.CreateMethodContext(FunctionModifier.Public, false,
                        structDelegate.Name + "_unaryop_" + function.name, function.declaredReturn,
                        function.parameters);

                    var emitter = new UnaryDeclaredOperatorEmitter(methodContext, function);
                    operatorFunctions.Add(emitter);
                    structDelegate.AddPrefixOperator(op, emitter);
                } else {
                    if (function.parameters.Count != 2) {
                        errors.Add(new StructuralError(StructuralError.ErrorType.InvalidOperator,
                            $"Binary operator {function.name} must have exactly 2 parameters", function.Start,
                            function.End));
                        continue;
                    }

                    bool isSuffix = function.parameters[0].type?.UnderlyingType(context) == underlyingType;
                    bool isPrefix = function.parameters[1].type?.UnderlyingType(context) == underlyingType;
                    if (!isPrefix && !isSuffix) {
                        errors.Add(new StructuralError(StructuralError.ErrorType.InvalidType,
                            $"One of the parameters of operator {function.name} must be of type {structDelegate.Name}",
                            function.Start,
                            function.End));
                        continue;
                    }

                    if (isSuffix) {
                        if (underlyingType.AllowedSuffixOperators(context).GetMatching(context, op, function.parameters[1].type!) != null) {
                            errors.Add(new StructuralError(StructuralError.ErrorType.InvalidType,
                                $"Binary operator {function.name} ({string.Join(", ", function.parameters.Select(parameter => parameter.type!.Name))}) already defined on type {structDelegate.Name}",
                                function.Start,
                                function.End));
                            continue;
                        }
                        var methodContext = context.CreateMethodContext(FunctionModifier.Public, false,
                            structDelegate.Name + "_suffixop_" + function.name + "_" + function.parameters[1].type!.Name, function.declaredReturn,
                            function.parameters);

                        var emitter = new SuffixDeclaredOperatorEmitter(methodContext, function);
                        operatorFunctions.Add(emitter);
                        structDelegate.AddSuffixOperator(op, emitter);
                    } else {
                        if (underlyingType.AllowedPrefixOperators(context).GetMatching(context, op, function.parameters[0].type!) != null) {
                            errors.Add(new StructuralError(StructuralError.ErrorType.InvalidType,
                                $"Binary operator {function.name} ({string.Join(", ", function.parameters.Select(parameter => parameter.type!.Name))}) already defined on type {structDelegate.Name}",
                                function.Start,
                                function.End));
                            continue;
                        }

                        var methodContext = context.CreateMethodContext(FunctionModifier.Public, false,
                            structDelegate.Name + "_prefixop_" + function.name + "_" + function.parameters[0].type!.Name, function.declaredReturn,
                            function.parameters);

                        var emitter = new PrefixDeclaredOperatorEmitter(methodContext, function);
                        operatorFunctions.Add(emitter);
                        structDelegate.AddPrefixOperator(op, emitter);
                    }
                }
            }
        }

        return errors;
    }

    public IEnumerable<StructuralError> TryVerifyFunctions(ModuleContext context) {
        var errors = new List<StructuralError>();

        foreach (var operatorFunction in operatorFunctions) {
            errors.AddRange(operatorFunction.EmitCode());
        }

        return errors;
    }

    public IEnumerable<StructuralError> TryImportFunctions(ModuleContext context) => [];
}

public abstract class DeclaredOperatorEmitter(IBlockContext methodContext, FunctionDeclaration function) : IOperatorEmitter {
    protected readonly FunctionDeclaration function = function;

    public TO2Type ResultType => function.declaredReturn;

    public abstract TO2Type OtherType { get; }

    public abstract bool Accepts(ModuleContext context, TO2Type otherType);

    public void EmitCode(IBlockContext context, Node target) {
        context.IL.EmitCall(OpCodes.Call, methodContext.MethodBuilder!, function.parameters.Count());
    }

    public void EmitAssign(IBlockContext context, IBlockVariable variable, Node target) {
        EmitCode(context, target);
        variable.Type.AssignFrom(context.ModuleContext, ResultType).EmitConvert(context, !variable.IsConst);
        variable.EmitStore(context);
    }

    public IOperatorEmitter FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => this;

    public IEnumerable<StructuralError> EmitCode() {
        function.EmitCode(methodContext);
        return methodContext.AllErrors;
    }
}

public class UnaryDeclaredOperatorEmitter(IBlockContext methodContext, FunctionDeclaration function)
    : DeclaredOperatorEmitter(methodContext, function) {
    public override bool Accepts(ModuleContext context, TO2Type otherType) => otherType == BuiltinType.Unit;

    public override TO2Type OtherType => BuiltinType.Unit;
}

public class PrefixDeclaredOperatorEmitter(IBlockContext methodContext, FunctionDeclaration function)
    : DeclaredOperatorEmitter(methodContext, function) {
    public override bool Accepts(ModuleContext context, TO2Type otherType) => function.parameters[0].type!.IsAssignableFrom(context, otherType);

    public override TO2Type OtherType => function.parameters[0].type!;
}

public class SuffixDeclaredOperatorEmitter(IBlockContext methodContext, FunctionDeclaration function)
    : DeclaredOperatorEmitter(methodContext, function) {
    public override bool Accepts(ModuleContext context, TO2Type otherType) => function.parameters[1].type!.IsAssignableFrom(context, otherType);

    public override TO2Type OtherType => function.parameters[1].type!;
}
