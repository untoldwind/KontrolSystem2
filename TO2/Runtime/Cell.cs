using System;
using System.Collections.Generic;

namespace KontrolSystem.TO2.Runtime {
    public class Cell<T> : IObservable<T> {
        private readonly object cellLock;
        private T element;
        private List<IObserver<T>> observers;

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

        public Cell<T> Update(Func<T, T> updater) {
            lock (cellLock) {
                element = updater(element);
            }
            Notify();
            return this;
        }

        public IDisposable Subscribe(IObserver<T> observer) {
            observers ??= new List<IObserver<T>>();
            observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        private void Notify() {
            if (observers == null) return;
            var value = element;
            foreach (var observer in observers) {
                observer.OnNext(value);
            }
        }

        private class Unsubscriber : IDisposable {
            private List<IObserver<T>> observers;
            private IObserver<T> observer;

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
        public static Cell<T> Create<T>(T value) => new Cell<T>(value);
    }
}
