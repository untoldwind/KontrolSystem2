using System;

namespace KontrolSystem.KSP.Runtime.KSPUI;

public class CallbackObserver<T>(Action<T> callback) : IObserver<T> {
    private readonly Action<T> callback = callback;

    public void OnNext(T value) {
        callback(value);
    }

    public void OnError(Exception error) {
    }

    public void OnCompleted() {
    }
}
