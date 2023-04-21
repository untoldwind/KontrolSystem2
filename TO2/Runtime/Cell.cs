using System;

namespace KontrolSystem.TO2.Runtime {
    public class Cell<T> {
        private readonly object cellLock;
        private T element;
        private Action<T> observers;

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

                observers?.Invoke(element);
            }
        }

        public Cell<T> Update(Func<T, T> updater) {
            lock (cellLock) {
                element = updater(element);
            }
            observers?.Invoke(element);
            return this;
        }

        public void AddObserver(Action<T> observer) {
            observers += observer;
        }

        public void RemoveObserver(Action<T> observer) {
            observers -= observer;
        }
    }

    public static class Cell {
        public static Cell<T> Create<T>(T value) => new Cell<T>(value);
    }
}
