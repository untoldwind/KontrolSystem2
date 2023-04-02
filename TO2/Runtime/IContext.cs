using System;
using System.Threading;

namespace KontrolSystem.TO2.Runtime {
    public interface IContext {
        ITO2Logger Logger { get; }

        void CheckTimeout();

        void ResetTimeout();

        void FunctionEnter(string name, object[] arguments);

        void FunctionLeave();

        bool IsBackground { get; }

        IContext CloneBackground(CancellationTokenSource token);
    }

    public class EmptyContext : IContext {
        private readonly bool background;
        private ConsoleLogger logger = new ConsoleLogger();

        public EmptyContext(bool background) => this.background = background;

        public ITO2Logger Logger => logger;

        public void CheckTimeout() {
        }

        public void ResetTimeout() {
        }

        public void FunctionEnter(string name, object[] arguments) {
        }

        public void FunctionLeave() {
        }

        public bool IsBackground => background;

        public IContext CloneBackground(CancellationTokenSource token) => new EmptyContext(true);
    }

    public static class ContextHolder {
        public static readonly ThreadLocal<IContext> CurrentContext = new ThreadLocal<IContext>();

        public static void CheckTimeout() {
            IContext context = CurrentContext.Value;
            if (context != null) context.CheckTimeout();
            else throw new ArgumentException("Running out of context");
        }

        public static void FunctionEnter(string name, object[] arguments) {
            IContext context = CurrentContext.Value;
            if (context != null) context.FunctionEnter(name, arguments);
            else throw new ArgumentException("Running out of context");
        }

        public static void FunctionLeave() {
            IContext context = CurrentContext.Value;
            if (context != null) context.FunctionLeave();
            else throw new ArgumentException("Running out of context");
        }
    }
}
