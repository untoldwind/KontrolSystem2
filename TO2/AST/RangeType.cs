using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public class RangeType : RealizedType {
        public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods { get; }
        public override Dictionary<string, IFieldAccessFactory> DeclaredFields { get; }

        public RangeType() {
            DeclaredMethods = new Dictionary<string, IMethodInvokeFactory> {
                {
                    "map", new BoundMethodInvokeFactory("Map the elements of the range, i.e. convert it into an array.",
                        true,
                        () => new ArrayType(new GenericParameter("T")),
                        () => new List<RealizedParameter> {
                            new RealizedParameter("mapper",
                                new FunctionType(false, new List<TO2Type> {BuiltinType.Int}, new GenericParameter("T")))
                        },
                        false, typeof(Range), typeof(Range).GetMethod("Map"))
                }
            };
            DeclaredFields = new Dictionary<string, IFieldAccessFactory> {
                {
                    "length",
                    new BoundPropertyLikeFieldAccessFactory("The length of the range", () => BuiltinType.Int,
                        typeof(Range), typeof(Range).GetProperty("Length"))
                }
            };
        }

        public override string Name => "Range";

        public override bool IsValid(ModuleContext context) => true;

        public override RealizedType UnderlyingType(ModuleContext context) => this;

        public override Type GeneratedType(ModuleContext context) => typeof(Range);

        public override IIndexAccessEmitter AllowedIndexAccess(ModuleContext context, IndexSpec indexSpec) => null;

        public override IForInSource ForInSource(ModuleContext context, TO2Type typeHint) => new RangeForInSource();
    }

    public class RangeForInSource : IForInSource {
        private ILocalRef currentIndex;
        private ILocalRef rangeRef;

        public RealizedType ElementType => BuiltinType.Int;

        public void EmitInitialize(IBlockContext context) {
            rangeRef = context.DeclareHiddenLocal(typeof(Range));
            rangeRef.EmitStore(context);
            currentIndex = context.DeclareHiddenLocal(typeof(long));
            rangeRef.EmitLoad(context);
            context.IL.Emit(OpCodes.Ldfld, typeof(Range).GetField("from"));
            context.IL.Emit(OpCodes.Ldc_I4_1);
            context.IL.Emit(OpCodes.Conv_I8);
            context.IL.Emit(OpCodes.Sub);
            currentIndex.EmitStore(context);
        }

        public void EmitCheckDone(IBlockContext context, LabelRef loop) {
            currentIndex.EmitLoad(context);
            context.IL.Emit(OpCodes.Ldc_I4_1);
            context.IL.Emit(OpCodes.Conv_I8);
            context.IL.Emit(OpCodes.Add);
            context.IL.Emit(OpCodes.Dup);
            currentIndex.EmitStore(context);
            rangeRef.EmitLoad(context);
            context.IL.Emit(OpCodes.Ldfld, typeof(Range).GetField("to"));
            context.IL.Emit(loop.isShort ? OpCodes.Blt_S : OpCodes.Blt, loop);
        }

        public void EmitNext(IBlockContext context) => currentIndex.EmitLoad(context);
    }
}
