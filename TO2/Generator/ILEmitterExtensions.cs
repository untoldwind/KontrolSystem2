using System.Reflection.Emit;

namespace KontrolSystem.TO2.Generator {
    public static class ILEmitterExtensions {
        public static void EmitLoad(this ILocalRef localRef, IBlockContext context) {
            switch (localRef.LocalIndex) {
            case 0:
                context.IL.Emit(OpCodes.Ldloc_0);
                return;
            case 1:
                context.IL.Emit(OpCodes.Ldloc_1);
                return;
            case 2:
                context.IL.Emit(OpCodes.Ldloc_2);
                return;
            case 3:
                context.IL.Emit(OpCodes.Ldloc_3);
                return;
            case { } n when n < 256:
                context.IL.Emit(OpCodes.Ldloc_S, localRef);
                return;
            default:
                context.IL.Emit(OpCodes.Ldloc, localRef);
                return;
            }
        }

        public static void EmitLoadPtr(this ILocalRef localRef, IBlockContext context) {
            if (localRef.LocalIndex < 256) context.IL.Emit(OpCodes.Ldloca_S, localRef);
            else context.IL.Emit(OpCodes.Ldloca, localRef);
        }

        public static void EmitStore(this ILocalRef localRef, IBlockContext context) {
            switch (localRef.LocalIndex) {
            case 0:
                context.IL.Emit(OpCodes.Stloc_0);
                return;
            case 1:
                context.IL.Emit(OpCodes.Stloc_1);
                return;
            case 2:
                context.IL.Emit(OpCodes.Stloc_2);
                return;
            case 3:
                context.IL.Emit(OpCodes.Stloc_3);
                return;
            case { } n when n < 256:
                context.IL.Emit(OpCodes.Stloc_S, localRef);
                return;
            default:
                context.IL.Emit(OpCodes.Stloc, localRef);
                return;
            }
        }
    }
}
