using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.Core;

public static class MainframeEvents {
    public class ProcessStarted(string name, string[] arguments) {
        public string Name => name;

        public string[] Arguments => arguments;
    }

    public class ProcessStopped(string name, Option<string> error) {
        public string Name => name;

        public Option<string> Error => error;
    }
}
