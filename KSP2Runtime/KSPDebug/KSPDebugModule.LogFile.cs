using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPDebug;

public partial class KSPDebugModule {
    [KSClass("LogFile",
        Description = "Represents a log file."
    )]
    public interface ILogFile {
        [KSMethod(Description = "Write a log message to the file.")]
        public Future<object?> Log(string message);

        [KSMethod(Description = "Truncate/clear the log file.")]
        public void Truncate();
    }
}
