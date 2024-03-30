using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class ArrayType : RealizedType {
    private readonly OperatorCollection allowedSuffixOperators;

    public ArrayType(TO2Type elementType, int dimension = 1) {
        ElementType = dimension > 1 ? new ArrayType(elementType, dimension - 1) : elementType;
        allowedSuffixOperators = new OperatorCollection {
            {
                Operator.Add, new StaticMethodOperatorEmitter(() => this, () => this,
                    typeof(ArrayMethods).GetMethod("Concat"), context => [elementType.UnderlyingType(context)])
            }, {
                Operator.Add, new StaticMethodOperatorEmitter(() => elementType, () => this,
                    typeof(ArrayMethods).GetMethod("Append"), context => [elementType.UnderlyingType(context)])
            }, {
                Operator.AddAssign, new StaticMethodOperatorEmitter(() => this, () => this,
                    typeof(ArrayMethods).GetMethod("Concat"), context => [elementType.UnderlyingType(context)])
            }, {
                Operator.AddAssign, new StaticMethodOperatorEmitter(() => elementType, () => this,
                    typeof(ArrayMethods).GetMethod("Append"), context => [elementType.UnderlyingType(context)])
            }
        };
        DeclaredMethods = ArrayMethods.MethodInvokers(ElementType).ToDictionary(m => m.name, m => m.invoker);
        DeclaredFields = new Dictionary<string, IFieldAccessFactory> {
            {
                "length",
                new InlineFieldAccessFactory("Length of the array, i.e. number of elements in the array.",
                    () => BuiltinType.Int, REPLArrayLength, OpCodes.Ldlen, OpCodes.Conv_I8)
            },
            {
                "is_empty",
                new InlineFieldAccessFactory("Check if the array is empty, i.e. there are no elements in the array.",
                    () => BuiltinType.Bool, REPLArrayIsEmpty, OpCodes.Ldlen, OpCodes.Ldc_I4_0, OpCodes.Ceq)
            },
            {
                "is_not_empty",
                new InlineFieldAccessFactory("Check if the array is not empty, i.e. there are elements in the array.",
                    () => BuiltinType.Bool, REPLArrayIsNotEmpty, OpCodes.Ldlen, OpCodes.Ldc_I4_0, OpCodes.Cgt_Un)
            }
        };
    }

    public TO2Type ElementType { get; }

    public override string Name => $"{ElementType}[]";

    public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods { get; }

    public override Dictionary<string, IFieldAccessFactory> DeclaredFields { get; }

    public override bool IsValid(ModuleContext context) => ElementType.IsValid(context);

    public override RealizedType UnderlyingType(ModuleContext context) => new ArrayType(ElementType.UnderlyingType(context));

    public override Type GeneratedType(ModuleContext context) => ElementType.GeneratedType(context).MakeArrayType();

    public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedSuffixOperators;

    public override IIndexAccessEmitter? AllowedIndexAccess(ModuleContext context, IndexSpec indexSpec) {
        switch (indexSpec.indexType) {
        case IndexSpecType.Single:
            var underlyingElement = ElementType.UnderlyingType(context);

            return new InlineArrayIndexAccessEmitter(underlyingElement, indexSpec.start);
        default:
            return null;
        }
    }

    public override IForInSource ForInSource(ModuleContext context, TO2Type? typeHint) =>
        new ArrayForInSource(GeneratedType(context), ElementType.UnderlyingType(context));

    public override RealizedType
        FillGenerics(ModuleContext context, Dictionary<string, RealizedType>? typeArguments) {
        return new ArrayType(ElementType.UnderlyingType(context).FillGenerics(context, typeArguments));
    }

    public override IEnumerable<(string name, RealizedType type)> InferGenericArgument(ModuleContext context,
        RealizedType? concreteType) {
        var concreteArray = concreteType as ArrayType;
        if (concreteArray == null) return [];
        return ElementType.InferGenericArgument(context, concreteArray.ElementType.UnderlyingType(context));
    }

    public override IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) {
        return ArrayAssignEmitter.Instance;
    }

    private static IREPLValue REPLArrayLength(Node node, IREPLValue target) {
        if (target.Value is Array a) return new REPLInt(a.Length);

        throw new REPLException(node, $"Get array length from a non-array: {target.Type.Name}");
    }

    private static IREPLValue REPLArrayIsEmpty(Node node, IREPLValue target) {
        if (target.Value is Array a) return new REPLBool(a.Length == 0);

        throw new REPLException(node, $"Get array is_empty from a non-array: {target.Type.Name}");
    }

    private static IREPLValue REPLArrayIsNotEmpty(Node node, IREPLValue target) {
        if (target.Value is Array a) return new REPLBool(a.Length > 0);

        throw new REPLException(node, $"Get array is_not_empty from a non-array: {target.Type.Name}");
    }

    public override IREPLValue REPLCast(object? value) {
        if (value is Array a)
            return new REPLArray(this, a);

        throw new REPLException(new Position("Intern"), new Position("Intern"),
            $"{value?.GetType()} can not be cast to REPLArray");
    }
}

public class ArrayAssignEmitter : IAssignEmitter {
    public static readonly IAssignEmitter Instance = new ArrayAssignEmitter();

    public void EmitAssign(IBlockContext context, IBlockVariable variable, Expression expression,
        bool dropResult) {
        if (!variable.IsConst) {
            expression.EmitCode(context, false);
            context.IL.EmitCall(OpCodes.Call, typeof(ArrayMethods).GetMethod("DeepClone")!, 1);
            if (!dropResult) context.IL.Emit(OpCodes.Dup);

            variable.EmitStore(context);
        } else {
            expression.EmitStore(context, variable, dropResult);
        }
    }

    public void EmitConvert(IBlockContext context, bool mutableTarget) {
        if (mutableTarget)
            context.IL.EmitCall(OpCodes.Call, typeof(ArrayMethods).GetMethod("DeepClone")!, 1);
    }

    public IREPLValue EvalConvert(Node node, IREPLValue value) {
        return value;
    }
}

public class ArrayForInSource(Type arrayType, RealizedType elementType) : IForInSource {
    private ILocalRef? arrayRef;

    private ILocalRef? currentIndex;

    public RealizedType ElementType { get; } = elementType;

    public void EmitInitialize(IBlockContext context) {
        arrayRef = context.DeclareHiddenLocal(arrayType);
        arrayRef.EmitStore(context);
        currentIndex = context.DeclareHiddenLocal(typeof(int));
        context.IL.Emit(OpCodes.Ldc_I4_M1);
        currentIndex.EmitStore(context);
    }

    public void EmitCheckDone(IBlockContext context, LabelRef loop) {
        currentIndex!.EmitLoad(context);
        context.IL.Emit(OpCodes.Ldc_I4_1);
        context.IL.Emit(OpCodes.Add);
        context.IL.Emit(OpCodes.Dup);
        currentIndex!.EmitStore(context);
        arrayRef!.EmitLoad(context);
        context.IL.Emit(OpCodes.Ldlen);
        context.IL.Emit(OpCodes.Conv_I4);
        context.IL.Emit(loop.isShort ? OpCodes.Blt_S : OpCodes.Blt, loop);
    }

    public void EmitNext(IBlockContext context) {
        arrayRef!.EmitLoad(context);
        currentIndex!.EmitLoad(context);
        if (ElementType == BuiltinType.Bool) context.IL.Emit(OpCodes.Ldelem_I4);
        else if (ElementType == BuiltinType.Int) context.IL.Emit(OpCodes.Ldelem_I8);
        else if (ElementType == BuiltinType.Float) context.IL.Emit(OpCodes.Ldelem_R8);
        else context.IL.Emit(OpCodes.Ldelem, ElementType.GeneratedType(context.ModuleContext));
    }
}
