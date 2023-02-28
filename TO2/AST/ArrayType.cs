using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.AST {
    public class ArrayType : RealizedType {
        public TO2Type ElementType { get; }

        public ArrayType(TO2Type elementType) {
            ElementType = elementType;
            DeclaredMethods = new Dictionary<string, IMethodInvokeFactory> {
                {
                    "map", new BoundMethodInvokeFactory("Map the content of the array", true,
                        () => new ArrayType(new GenericParameter("U")),
                        () => new List<RealizedParameter> {
                            new RealizedParameter("mapper", new FunctionType(false, new List<TO2Type> {
                                ElementType
                            }, new GenericParameter("U")))
                        },
                        false, typeof(ArrayMethods), typeof(ArrayMethods).GetMethod("Map"),
                        context => ("T", this.ElementType.UnderlyingType(context)).Yield())
                }, {
                    "map_with_index", new BoundMethodInvokeFactory("Map the content of the array", true,
                        () => new ArrayType(new GenericParameter("U")),
                        () => new List<RealizedParameter> {
                            new RealizedParameter("mapper",
                                new FunctionType(false, new List<TO2Type> {ElementType, BuiltinType.Int},
                                    new GenericParameter("U")))
                        },
                        false, typeof(ArrayMethods), typeof(ArrayMethods).GetMethod("MapWithIndex"),
                        context => ("T", this.ElementType.UnderlyingType(context)).Yield())
                }, {
                    "filter", new BoundMethodInvokeFactory("Filter the content of the array by a `predicate", true,
                        () => new ArrayType(new GenericParameter("T")),
                        () => new List<RealizedParameter> {
                            new RealizedParameter("predictate",
                                new FunctionType(false, new List<TO2Type> {ElementType}, BuiltinType.Bool))
                        },
                        false, typeof(ArrayMethods), typeof(ArrayMethods).GetMethod("Filter"),
                        context => ("T", this.ElementType.UnderlyingType(context)).Yield())
                }, {
                    "find", new BoundMethodInvokeFactory("Find first item of the array matching `predicate`", true,
                        () => new OptionType(new GenericParameter("T")),
                        () => new List<RealizedParameter> {
                            new RealizedParameter("predictate",
                                new FunctionType(false, new List<TO2Type> {ElementType}, BuiltinType.Bool))
                        },
                        false, typeof(ArrayMethods), typeof(ArrayMethods).GetMethod("Find"),
                        context => ("T", ElementType.UnderlyingType(context)).Yield())
                }, {
                    "exists", new BoundMethodInvokeFactory("Check if an item of the array matches `predicate`", true,
                        () => BuiltinType.Bool,
                        () => new List<RealizedParameter> {
                            new RealizedParameter("predictate",
                                new FunctionType(false, new List<TO2Type> {ElementType}, BuiltinType.Bool))
                        },
                        false, typeof(ArrayMethods), typeof(ArrayMethods).GetMethod("Exists"),
                        context => ("T", ElementType.UnderlyingType(context)).Yield())
                }, {
                    "to_string", new BoundMethodInvokeFactory("Get string representation of the array", true,
                        () => BuiltinType.String,
                        () => new List<RealizedParameter>(),
                        false, typeof(ArrayMethods), typeof(ArrayMethods).GetMethod("ArrayToString"),
                        context => ("T", ElementType.UnderlyingType(context)).Yield())
                },
            };
            DeclaredFields = new Dictionary<string, IFieldAccessFactory> {
                {
                    "length",
                    new InlineFieldAccessFactory("Length of the array, i.e. number of elements in the array.",
                        () => BuiltinType.Int, OpCodes.Ldlen, OpCodes.Conv_I8)
                }
            };
        }

        public override string Name => $"{ElementType}[]";

        public override bool IsValid(ModuleContext context) => ElementType.IsValid(context);

        public override RealizedType UnderlyingType(ModuleContext context) =>
            new ArrayType(ElementType.UnderlyingType(context));

        public override Type GeneratedType(ModuleContext context) => ElementType.GeneratedType(context).MakeArrayType();

        public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods { get; }

        public override Dictionary<string, IFieldAccessFactory> DeclaredFields { get; }

        public override IIndexAccessEmitter AllowedIndexAccess(ModuleContext context, IndexSpec indexSpec) {
            switch (indexSpec.indexType) {
            case IndexSpecType.Single:
                RealizedType underlyingElement = ElementType.UnderlyingType(context);

                return new InlineArrayIndexAccessEmitter(underlyingElement, indexSpec.start);
            default:
                return null;
            }
        }

        public override IForInSource ForInSource(ModuleContext context, TO2Type typeHint) =>
            new ArrayForInSource(GeneratedType(context), ElementType.UnderlyingType(context));

        public override RealizedType
            FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) =>
            new ArrayType(ElementType.UnderlyingType(context).FillGenerics(context, typeArguments));

        public override IEnumerable<(string name, RealizedType type)> InferGenericArgument(ModuleContext context,
            RealizedType concreteType) {
            ArrayType concreteArray = concreteType as ArrayType;
            if (concreteArray == null) return Enumerable.Empty<(string name, RealizedType type)>();
            return ElementType.InferGenericArgument(context, concreteArray.ElementType.UnderlyingType(context));
        }
    }

    public class ArrayForInSource : IForInSource {
        private readonly Type arrayType;
        private readonly RealizedType elementType;

        private ILocalRef currentIndex;
        private ILocalRef arrayRef;

        public ArrayForInSource(Type arrayType, RealizedType elementType) {
            this.arrayType = arrayType;
            this.elementType = elementType;
        }

        public RealizedType ElementType => elementType;

        public void EmitInitialize(IBlockContext context) {
            arrayRef = context.DeclareHiddenLocal(arrayType);
            arrayRef.EmitStore(context);
            currentIndex = context.DeclareHiddenLocal(typeof(int));
            context.IL.Emit(OpCodes.Ldc_I4_M1);
            currentIndex.EmitStore(context);
        }

        public void EmitCheckDone(IBlockContext context, LabelRef loop) {
            currentIndex.EmitLoad(context);
            context.IL.Emit(OpCodes.Ldc_I4_1);
            context.IL.Emit(OpCodes.Add);
            context.IL.Emit(OpCodes.Dup);
            currentIndex.EmitStore(context);
            arrayRef.EmitLoad(context);
            context.IL.Emit(OpCodes.Ldlen);
            context.IL.Emit(OpCodes.Conv_I4);
            context.IL.Emit(loop.isShort ? OpCodes.Blt_S : OpCodes.Blt, loop);
        }

        public void EmitNext(IBlockContext context) {
            arrayRef.EmitLoad(context);
            currentIndex.EmitLoad(context);
            if (elementType == BuiltinType.Bool) context.IL.Emit(OpCodes.Ldelem_I4);
            else if (elementType == BuiltinType.Int) context.IL.Emit(OpCodes.Ldelem_I8);
            else if (elementType == BuiltinType.Float) context.IL.Emit(OpCodes.Ldelem_R8);
            else context.IL.Emit(OpCodes.Ldelem, elementType.GeneratedType(context.ModuleContext));
        }
    }
}
