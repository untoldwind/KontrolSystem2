using System;

namespace KontrolSystem.TO2.Runtime {
    public interface IAnyOption {
        bool Defined { get; }
    }

    public struct Option<T> : IAnyOption {
        public readonly bool defined;
        public readonly T value;

        public Option(T value) {
            defined = true;
            this.value = value;
        }

        public bool Defined => defined;

        public Option<U> Map<U>(Func<T, U> mapper) => defined ? new Option<U>(mapper(value)) : new Option<U>();

        public Option<U> Then<U>(Func<T, Option<U>> mapper) => defined ? mapper(value) : new Option<U>();

        public Result<T, E> OkOr<E>(E error) =>
            defined ? new Result<T, E>(true, value, default) : new Result<T, E>(false, default, error);

        public T GetValueOrDefault(T defaultValue) => defined ? value : defaultValue;
    }

    public static class Option {
        public static Option<T> Some<T>(T value) => new Option<T>(value);

        public static Option<T> None<T>() => new Option<T>();
    }
}
