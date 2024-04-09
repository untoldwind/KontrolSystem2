﻿using System;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST;

public abstract partial class BuiltinType {
    private class TO2Unit : BuiltinType {
        private readonly IAssignEmitter anyToUnitAssign;

        internal TO2Unit() {
            anyToUnitAssign = new AnyToUnitAssign();
        }

        public override string Name => "Unit";

        public override Type GeneratedType(ModuleContext context) => typeof(object);

        public override bool IsAssignableFrom(ModuleContext context, TO2Type otherType) => true;

        public override IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) => anyToUnitAssign;
    }

    private class AnyToUnitAssign : IAssignEmitter {
        public void EmitAssign(IBlockContext context, IBlockVariable variable, Expression expression,
            bool dropResult) {
            expression.EmitCode(context, true);
            context.IL.Emit(OpCodes.Ldnull);
            if (!dropResult) context.IL.Emit(OpCodes.Dup);
            variable.EmitStore(context);
        }

        public void EmitConvert(IBlockContext context, bool mutableTarget) {
            if (context.IL.StackCount > 0)
                context.IL.Emit(OpCodes.Pop);
            context.IL.Emit(OpCodes.Ldnull);
        }
    }

    private class TO2Any : BuiltinType {
        public override string Name => "Any";

        public override Type GeneratedType(ModuleContext context) => typeof(object);

        public override bool IsAssignableFrom(ModuleContext context, TO2Type otherType) => true;

        public override IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) => new AnyToAnyAssign(otherType.GeneratedType(context));
    }

    private class AnyToAnyAssign(Type otherType) : IAssignEmitter {
        public void EmitAssign(IBlockContext context, IBlockVariable variable, Expression expression,
            bool dropResult) {
            expression.EmitCode(context, false);
            if (otherType.IsValueType) context.IL.Emit(OpCodes.Box, otherType);
            if (!dropResult) context.IL.Emit(OpCodes.Dup);
            variable.EmitStore(context);
        }

        public void EmitConvert(IBlockContext context, bool mutableTarget) {
            if (otherType.IsValueType) context.IL.Emit(OpCodes.Box, otherType);
        }
    }
}
