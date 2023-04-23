using System;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public class CallbackObserver<T> : IObserver<T> {
        private readonly Action<T> callback;

        public CallbackObserver(Action<T> callback) {
            this.callback = callback;
        }

        public void OnNext(T value) => callback(value);

        public void OnError(Exception error) {
        }

        public void OnCompleted() {
        }
    }
}
