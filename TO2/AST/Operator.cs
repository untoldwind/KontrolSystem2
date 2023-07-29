using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public enum Operator {
        Assign, // =
        Add, // +
        AddAssign, // +=
        Sub, // -
        SubAssign, // -=
        Mul, // *
        MulAssign, // *=
        Div, // /
        DivAssign, // /=
        Mod, // %
        ModAssign, // %=
        BitOr, // |
        BitOrAssign, // |=
        BitAnd, // &
        BitAndAssign, // &=
        BitXor, // **
        BitXorAssign, // **=
        Pow, // **
        PowAssign, // **=
        Eq, // ==
        NotEq, // !=
        Lt, // <
        Le, // <=
        Gt, // >
        Ge, // >=
        Neg, // -
        Not, // !
        BitNot, // ~
        BoolAnd, // &&
        BoolOr, // ||
        Unwrap // ?
    }

    public delegate IREPLValue REPLOperator(Node node, IREPLValue left, IREPLValue right);

    public interface IOperatorEmitter {
        TO2Type ResultType { get; }
        TO2Type OtherType { get; }

        bool Accepts(ModuleContext context, TO2Type otherType);

        void EmitCode(IBlockContext context, Node target);

        void EmitAssign(IBlockContext context, IBlockVariable variable, Node target);

        IOperatorEmitter FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments);

        IREPLValue Eval(Node node, IREPLValue left, IREPLValue right);
    }

    public class DirectOperatorEmitter : IOperatorEmitter {
        private readonly Func<TO2Type> otherTypeFactory;
        private readonly Func<TO2Type> resultTypeFactory;
        private readonly OpCode[] opCodes;
        private readonly REPLOperator replOperator;

        public DirectOperatorEmitter(Func<TO2Type> otherTypeFactory, Func<TO2Type> resultTypeFactory,
            REPLOperator replOperator, params OpCode[] opCodes) {
            this.otherTypeFactory = otherTypeFactory;
            this.resultTypeFactory = resultTypeFactory;
            this.replOperator = replOperator;
            this.opCodes = opCodes;
        }

        public bool Accepts(ModuleContext context, TO2Type otherType) =>
            otherTypeFactory().IsAssignableFrom(context, otherType);

        public TO2Type OtherType => otherTypeFactory();

        public TO2Type ResultType => resultTypeFactory();

        public void EmitCode(IBlockContext context, Node target) {
            foreach (OpCode opCode in opCodes) context.IL.Emit(opCode);
        }

        public void EmitAssign(IBlockContext context, IBlockVariable variable, Node target) {
            EmitCode(context, target);
            variable.Type.AssignFrom(context.ModuleContext, ResultType).EmitConvert(context);
            variable.EmitStore(context);
        }

        public IOperatorEmitter FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) =>
            this;

        public IREPLValue Eval(Node node, IREPLValue left, IREPLValue right) => replOperator(node, left, right);
    }

    public class StaticMethodOperatorEmitter : IOperatorEmitter {
        private readonly Func<TO2Type> otherTypeFactory;
        private readonly Func<TO2Type> resultTypeFactory;
        private readonly MethodInfo methodInfo;
        private readonly OpCode[] postOpCodes;
        private readonly Func<ModuleContext, IEnumerable<RealizedType>> targetTypeArguments;

        public StaticMethodOperatorEmitter(Func<TO2Type> otherTypeFactory, Func<TO2Type> resultTypeFactory,
            MethodInfo methodInfo, Func<ModuleContext, IEnumerable<RealizedType>> targetTypeArguments = null,
            params OpCode[] postOpCodes) {
            this.otherTypeFactory = otherTypeFactory;
            this.resultTypeFactory = resultTypeFactory;
            this.methodInfo = methodInfo;
            this.targetTypeArguments = targetTypeArguments;
            this.postOpCodes = postOpCodes;
        }

        public bool Accepts(ModuleContext context, TO2Type otherType) =>
            otherTypeFactory().IsAssignableFrom(context, otherType);

        public TO2Type OtherType => otherTypeFactory();

        public TO2Type ResultType => resultTypeFactory();

        public void EmitCode(IBlockContext context, Node target) {
            if (methodInfo.IsGenericMethod && targetTypeArguments != null) {
                var filled = methodInfo.MakeGenericMethod(targetTypeArguments(context.ModuleContext)
                    .Select(realizedType => realizedType.GeneratedType(context.ModuleContext)).ToArray());
                context.IL.EmitCall(OpCodes.Call, filled, filled.GetParameters().Length);
            } else {
                context.IL.EmitCall(OpCodes.Call, methodInfo, methodInfo.GetParameters().Length);
            }

            foreach (OpCode opCOde in postOpCodes) context.IL.Emit(opCOde);
        }

        public void EmitAssign(IBlockContext context, IBlockVariable variable, Node target) {
            EmitCode(context, target);
            variable.Type.AssignFrom(context.ModuleContext, ResultType).EmitConvert(context);
            variable.EmitStore(context);
        }

        public IOperatorEmitter FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) {
            if (methodInfo.IsGenericMethod) {
                Type[] arguments = methodInfo.GetGenericArguments().Select(t => {
                    if (!typeArguments.ContainsKey(t.Name))
                        throw new ArgumentException($"Generic parameter {t.Name} not found");
                    return typeArguments[t.Name].GeneratedType(context);
                }).ToArray();

                return new StaticMethodOperatorEmitter(
                    () => otherTypeFactory().UnderlyingType(context).FillGenerics(context, typeArguments),
                    () => resultTypeFactory().UnderlyingType(context).FillGenerics(context, typeArguments),
                    methodInfo.MakeGenericMethod(arguments), null, postOpCodes);
            }

            return this;
        }

        public IREPLValue Eval(Node node, IREPLValue left, IREPLValue right) {
            var result = methodInfo.Invoke(null, new object[] { left.Value, right.Value });

            return ResultType.REPLCast(result);
        }
    }
}
