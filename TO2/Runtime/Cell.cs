using System;
using System.Collections.Generic;

namespace KontrolSystem.TO2.Runtime;

public class Cell<T> : IObservable<T> {
    private readonly object cellLock;
    private T element;
    private List<IObserver<T>>? observers;

    public Cell(T value) {
        element = value;
        cellLock = new object();
    }

    public T Value {
        get {
            lock (cellLock) {
                return element;
            }
        }
        set {
            lock (cellLock) {
                element = value;
            }

            Notify();
        }
    }

    public IDisposable Subscribe(IObserver<T> observer) {
        observers ??= new List<IObserver<T>>();
        observers.Add(observer);
        return new Unsubscriber(observers, observer);
    }

    public Cell<T> Update(Func<T, T> updater) {
        lock (cellLock) {
            element = updater(element);
        }

        Notify();
        return this;
    }

    private void Notify() {
        if (observers == null) return;
        var value = element;
        foreach (var observer in observers) observer.OnNext(value);
    }

    private class Unsubscriber : IDisposable {
        private readonly IObserver<T>? observer;
        private readonly List<IObserver<T>> observers;

        public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer) {
            this.observers = observers;
            this.observer = observer;
        }

        public void Dispose() {
            if (observer != null && observers.Contains(observer))
                observers.Remove(observer);
        }
    }
}

public static class Cell {
    public static Cell<T> Create<T>(T value) {
        return new Cell<T>(value);
    }
}
