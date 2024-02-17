using System;

namespace KontrolSystem.TO2.Runtime;

public class InvalidAsyncStateException : Exception {
    public InvalidAsyncStateException(int state) : base($"Async function in invalid state: {state}") {
    }
}
