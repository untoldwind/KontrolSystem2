using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public struct ILCount {
        public int opCodes;
        public int stack;
    }

    public delegate RealizedType TypeHint(IBlockContext context);

    /// <summary>
    /// Base class of all expressions.
    /// </summary>
    public abstract class Expression : Node, IBlockItem {
        public bool IsComment => false;

        protected Expression(Position start, Position end) : base(start, end) {
        }

        public abstract IVariableContainer VariableContainer { set; }

        public virtual TypeHint TypeHint {
            set { }
        }

        public abstract TO2Type ResultType(IBlockContext context);

        /// <summary>
        /// Emit any IL code as preparation for the actual expression.
        /// A preparation should happen on an empty stack and leave an empty stack.
        /// For the most part this is necessary when calling async functions as the underlying state machine
        /// can only restore local variables but not intermediate values on the stack.
        /// </summary>
        public abstract void Prepare(IBlockContext context);

        /// <summary>
        /// Emit the IL code of the expression.
        /// <param name="context">The context of the block containing the expression</param>
        /// <param name="dropResult">Toggles if the result of the expression should be put on the stack or just dropped.</param>
        /// </summary>
        public abstract void EmitCode(IBlockContext context, bool dropResult);

        /// <summary>
        /// Variant of the Emit when a pointer to the result of the expression is required, rather
        /// then the value itself.
        /// I.e. EmitPtr will leave a pointer on the stack where the result is stored.
        /// </summary>
        public virtual void EmitPtr(IBlockContext context) {
            using ITempBlockVariable tempLocal =
                context.MakeTempVariable(ResultType(context).UnderlyingType(context.ModuleContext));
            EmitStore(context, tempLocal, true);

            if (context.HasErrors) return;

            tempLocal.EmitLoadPtr(context);
        }

        /// <summary>
        /// Variant of Emit when the result of the expression should be directly stored to a local variable.
        /// </summary>
        public virtual void EmitStore(IBlockContext context, IBlockVariable variable, bool dropResult) {
            EmitCode(context, false);

            if (context.HasErrors) return;

            if (!dropResult) context.IL.Emit(OpCodes.Dup);

            variable.EmitStore(context);
        }

        /// <summary>
        /// Get the size of the IL opcodes for this expression.
        /// <param name="context">The context of the block containing the expression</param>
        /// <param name="dropResult">Toggles if the result of the expression should be also put on the stack or just dropped.</param>
        /// </summary>
        public ILCount GetILCount(IBlockContext context, bool dropResult) {
            IBlockContext countingContext = context.CloneCountingContext();

            EmitCode(countingContext, dropResult);

            return new ILCount {
                opCodes = countingContext.IL.ILSize,
                stack = countingContext.IL.StackCount
            };
        }

        /// <summary>
        /// List of variables the expression would like to introduce to the current scope.
        /// Right now this is only honor by "if" for the "then" scope and by "while" for the loop-scope.  
        /// </summary>
        public virtual Dictionary<string, TO2Type> GetScopeVariables(IBlockContext context) => null;
    }

    /// <summary>
    /// A bracket expression.
    /// I.e. just an other expression surrounded by round brackets '(', ')'
    /// </summary>
    public class Bracket : Expression {
        private readonly Expression expression;

        public Bracket(Expression expression, Position start = new Position(), Position end = new Position()) :
            base(start, end) => this.expression = expression;

        public override IVariableContainer VariableContainer {
            set => expression.VariableContainer = value;
        }

        public override TypeHint TypeHint {
            set => expression.TypeHint = value;
        }

        public override TO2Type ResultType(IBlockContext context) => expression.ResultType(context);

        public override void Prepare(IBlockContext context) => expression.Prepare(context);

        public override void EmitCode(IBlockContext context, bool dropResult) =>
            expression.EmitCode(context, dropResult);

        public override void EmitPtr(IBlockContext context) => expression.EmitPtr(context);

        public override void EmitStore(IBlockContext context, IBlockVariable variable, bool dropResult) =>
            expression.EmitStore(context, variable, dropResult);
    }
}
