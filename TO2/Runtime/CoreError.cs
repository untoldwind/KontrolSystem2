using System.Linq;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.TO2.Runtime;

[KSModule("core::error",
    Description = "Error reporting and error handling."
)]
public class CoreError {
    [KSFunction(Description = "Get current stack.")]
    public static StackEntry[] CurrentStack() {
        return ContextHolder.CurrentContext.Value?.CurrentStack()?.ToArray() ?? [];
    }

    [KSClass("Error", Description = "Error information of a failed Result.")]
    public class Error(string message, StackEntry[] stackTrace) {
        public string message = message;

        [KSField]
        public string Message => message;

        [KSField] public StackEntry[] StackTrace => stackTrace;

        [KSMethod]
        public override string ToString() => message;
    }

    [KSClass("StackEntry", Description = "Stacktrace entry.")]
    public class StackEntry(string name, object[] arguments, string sourceName, int line) {
        [KSField]
        public string Name => name;

        [KSField]
        public string[] Arguments => arguments.Select(arg => arg.ToString()).ToArray();

        [KSField]
        public string SourceName => sourceName;

        [KSField]
        public long Line => line;

        [KSMethod]
        public override string ToString() => $"[{sourceName}:{line}] {name}({string.Join(",", arguments.Select(arg => StringMethods.Ellipsis(arg.ToString(), 12)))}) ";
    }
}
