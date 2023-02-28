namespace KontrolSystem.TO2.Runtime {
    public class InvalidAsyncStateException : System.Exception {
        public InvalidAsyncStateException(int state) : base($"Async function in invalid state: {state}") {
        }
    }
}
