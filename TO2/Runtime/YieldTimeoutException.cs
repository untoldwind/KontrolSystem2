using System;

namespace KontrolSystem.TO2.Runtime;

public class YieldTimeoutException(long millis) : Exception($"Timeout: Module did no yield after {millis} ms") {
}
