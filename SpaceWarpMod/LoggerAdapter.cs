using System;
using BepInEx.Logging;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.SpaceWarpMod {
    public class LoggerAdapter : ITO2Logger {
        private ManualLogSource backend = new ManualLogSource("KontrolSystem");

        private static LoggerAdapter _instance;

        public static LoggerAdapter Instance => _instance ??= new LoggerAdapter();

        public static bool debugEnabled = true;

        internal ManualLogSource Backend {
            set {
                backend = value;
            }
        }

        public void Debug(string message) {
            if (debugEnabled) backend.LogDebug(message);
        }

        public void Info(string message) => backend.LogInfo(message);

        public void Warning(string message) => backend.LogWarning(message);

        public void Error(string message) => backend.LogError(message);

        public void LogException(Exception exception) => backend.LogError(exception);
    }
}
