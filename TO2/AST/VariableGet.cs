using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class VariableGet : Expression, IAssignContext {
        private readonly string moduleName;
        private readonly string name;
        private IVariableContainer variableContainer;

        public VariableGet(List<string> namePath, Position start = new Position(), Position end = new Position()) :
            base(start, end) {
            if (namePath.Count > 1) {
                moduleName = String.Join("::", namePath.Take(namePath.Count - 1));
                name = namePath.Last();
            } else {
                moduleName = null;
                name = namePath.Last();
            }
        }

        public override IVariableContainer VariableContainer {
            set => variableContainer = value;
        }

        public override TO2Type ResultType(IBlockContext context) {
            TO2Type resultType = variableContainer.FindVariable(context, name);
            if (resultType != null) return resultType;
            resultType = ReferencedConstant(context.ModuleContext)?.Type;
            if (resultType != null) return resultType;
            resultType = ReferencedFunction(context.ModuleContext)?.DelegateType();
            if (resultType != null) return resultType;

            context.AddError(new StructuralError(
                StructuralError.ErrorType.NoSuchVariable,
                $"No local variable, constant or function '{name}'",
                Start,
                End
            ));
            return BuiltinType.Unit;
        }

        public bool IsConst(IBlockContext context) => context.FindVariable(name)?.IsConst ?? true;

        public override void Prepare(IBlockContext context) {
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            IBlockVariable blockVariable = context.FindVariable(name);

            if (blockVariable != null) {
                if (!dropResult) blockVariable.EmitLoad(context);
                return;
            }

            IKontrolConstant constant = ReferencedConstant(context.ModuleContext);

            if (constant != null) {
                if (dropResult) return;

                context.IL.Emit(OpCodes.Ldsfld, constant.RuntimeField);
                return;
            }

            IKontrolFunction function = ReferencedFunction(context.ModuleContext);

            if (function != null) {
                if (dropResult) return;

                context.IL.Emit(OpCodes.Ldnull);
                context.IL.EmitPtr(OpCodes.Ldftn, function.RuntimeMethod);
                context.IL.EmitNew(OpCodes.Newobj,
                    function.DelegateType().GeneratedType(context.ModuleContext)
                        .GetConstructor(new[] { typeof(object), typeof(IntPtr) }));
                return;
            }

            context.AddError(new StructuralError(
                StructuralError.ErrorType.NoSuchVariable,
                $"No local variable, constant or function '{name}'",
                Start,
                End
            ));
        }

        public override void EmitPtr(IBlockContext context) {
            IBlockVariable blockVariable = context.FindVariable(name);

            if (blockVariable != null) {
                blockVariable.EmitLoadPtr(context);
                return;
            }

            EmitCode(context, false);

            if (context.HasErrors) return;

            using ITempLocalRef tempLocal =
                context.IL.TempLocal(ResultType(context).GeneratedType(context.ModuleContext));
            if (tempLocal.LocalIndex < 256) {
                context.IL.Emit(OpCodes.Stloc_S, tempLocal);
                context.IL.Emit(OpCodes.Ldloca_S, tempLocal);
            } else {
                context.IL.Emit(OpCodes.Stloc, tempLocal);
                context.IL.Emit(OpCodes.Ldloca, tempLocal);
            }
        }

        private IKontrolConstant ReferencedConstant(ModuleContext context) => moduleName != null
            ? context.FindModule(moduleName)?.FindConstant(name)
            : context.mappedConstants.Get(name);

        private IKontrolFunction ReferencedFunction(ModuleContext context) => moduleName != null
            ? context.FindModule(moduleName)?.FindFunction(name)
            : context.mappedFunctions.Get(name);
    }
}
