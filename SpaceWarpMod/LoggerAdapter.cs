using System;
using KontrolSystem.TO2.Runtime;
using SpaceWarp.API.Logging;

namespace KontrolSystem.SpaceWarpMod {
    public class LoggerAdapter : ITO2Logger {
        private BaseModLogger backend = new ModLogger("KontrolSystem");

        private static LoggerAdapter _instance;

        public static LoggerAdapter Instance => _instance ??= new LoggerAdapter();

        public static bool debugEnabled = true;

        internal BaseModLogger Backend {
            set {
                backend = value;
            }
        }
        
        public void Debug(string message) {
            if (debugEnabled) backend.Debug(message);
        }

        public void Info(string message) => backend.Info(message);

        public void Warning(string message) => backend.Warn(message);

        public void Error(string message) => backend.Error(message);

        public void LogException(Exception exception) => UnityEngine.Debug.LogException(exception);
    }
}
