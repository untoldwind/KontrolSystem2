using System;

namespace KontrolSystem.TO2.Runtime;

public class InvalidAsyncStateException(int state) : Exception($"Async function in invalid state: {state}") {
}
