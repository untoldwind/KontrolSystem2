using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace KontrolSystem.TO2.Runtime;

public interface IContext {
    ITO2Logger Logger { get; }

    bool IsBackground { get; }

    void CheckTimeout();

    void ResetTimeout();

    void FunctionEnter(string name, object[] arguments, string sourceName, int line);

    void FunctionLeave();

    CoreError.StackEntry? CurrentCallSite { get; set; }

    IEnumerable<CoreError.StackEntry> CurrentStack();

    IContext CloneBackground(CancellationTokenSource token);
}

public class EmptyContext : IContext {
    private readonly ConsoleLogger logger = new();

    public EmptyContext(bool background) {
        IsBackground = background;
    }

    public ITO2Logger Logger => logger;

    public void CheckTimeout() {
    }

    public void ResetTimeout() {
    }

    public void FunctionEnter(string name, object[] arguments, string sourceName, int line) {
    }

    public void FunctionLeave() {
    }

    public CoreError.StackEntry? CurrentCallSite { get; set; }

    public IEnumerable<CoreError.StackEntry> CurrentStack() => Enumerable.Empty<CoreError.StackEntry>();

    public bool IsBackground { get; }

    public IContext CloneBackground(CancellationTokenSource token) {
        return new EmptyContext(true);
    }
}

public static class ContextHolder {
    public static readonly ThreadLocal<IContext?> CurrentContext = new();

    public static void CheckTimeout() {
        var context = CurrentContext.Value;
        if (context != null) context.CheckTimeout();
        else throw new ArgumentException("Running out of context");
    }

    public static void FunctionEnter(string name, object[] arguments, string sourceName, int line) {
        var context = CurrentContext.Value;
        if (context != null) {
            context.CurrentCallSite = null;
            context.FunctionEnter(name, arguments, sourceName, line);
        } else throw new ArgumentException("Running out of context");
    }

    public static void FunctionLeave() {
        var context = CurrentContext.Value;
        if (context != null) {
            context.CurrentCallSite = null;
            context.FunctionLeave();
        } else throw new ArgumentException("Running out of context");
    }

    public static void SetCallSite(string name, string sourceName, int line) {
        var context = CurrentContext.Value;
        if (context != null) context.CurrentCallSite = new CoreError.StackEntry(name, [], sourceName, line);
        else throw new ArgumentException("Running out of context");
    }
}
