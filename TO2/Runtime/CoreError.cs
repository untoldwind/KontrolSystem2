using System.Linq;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.TO2.Runtime;

[KSModule("core::error",
    Description = "Error reporting and error handling."
)]
public class CoreError {
    [KSFunction(Description = "Get current stack.")]
    public static StackEntry[] CurrentStack() {
        return ContextHolder.CurrentContext.Value?.CurrentStack() ?? [];
    }

    [KSClass("Error", Description = "Error information of a failed Result.")]
    public class Error(string message) {
        public string message = message;

        [KSField]
        public string Message => message;

        [KSMethod]
        public override string ToString() => message;
    }

    [KSClass("StackEntry", Description = "Stacktrace entry.")]
    public class StackEntry(string functionName, object[] arguments, string sourceName, int line) {
        [KSField]
        public string FunctionName => functionName;

        [KSField]
        public string[] Arguments => arguments.Select(arg => arg.ToString()).ToArray();

        [KSField]
        public string SourceName => sourceName;

        [KSField]
        public long Line => line;

        [KSMethod]
        public override string ToString() => $"{FunctionName}({string.Join(",", Arguments)}) [{SourceName}:{Line}]";
    }
}
