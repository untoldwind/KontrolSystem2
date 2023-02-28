using System;

namespace KontrolSystem.TO2.Runtime {
    public interface ITO2Logger {
        void Debug(string message);

        void Info(string message);

        void Warning(string message);

        void Error(string message);

        void LogException(Exception exception);
    }

    public class ConsoleLogger : ITO2Logger {
        public void Debug(string message) => Console.Out.WriteLine("DEBUG: " + message);

        public void Info(string message) => Console.Out.WriteLine("INFO: " + message);

        public void Warning(string message) => Console.Out.WriteLine("WARNING: " + message);

        public void Error(string message) => Console.Out.WriteLine("ERROR: " + message);

        public void LogException(Exception exception) {
            Console.Out.WriteLine(exception.Message);
            Console.Out.WriteLine(exception.StackTrace);
        }
    }
}
