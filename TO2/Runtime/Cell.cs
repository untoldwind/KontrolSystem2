using System;

namespace KontrolSystem.TO2.Runtime {
    public class Cell<T> {
        private readonly object cellLock;
        private T element;

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
            }
        }

        public Cell<T> Update(Func<T, T> updater) {
            lock (cellLock) {
                element = updater(element);
                return this;
            }
        }
    }

    public static class Cell {
        public static Cell<T> Create<T>(T value) => new Cell<T>(value);
    }
}
