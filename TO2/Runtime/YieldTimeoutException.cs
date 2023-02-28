using System;

namespace KontrolSystem.TO2.Runtime {
    public class YieldTimeoutException : Exception {
        public YieldTimeoutException(long millis) : base($"Timeout: Module did no yield after {millis} ms") {
        }
    }
}
