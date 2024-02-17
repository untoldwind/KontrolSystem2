using System;

namespace KontrolSystem.Parsing;

public interface IOption<out T> {
    bool IsEmpty { get; }

    bool IsDefined { get; }

    T Value { get; }

    IOption<U> Map<U>(Func<T, U> convert);
}

public static class Option {
    public static IOption<T> Some<T>(T value) {
        return new SomeOption<T>(value);
    }

    public static IOption<T> None<T>() {
        return new NoneOption<T>();
    }

    public static T GetOrElse<T>(this IOption<T> option, T defaultValue) {
        return option.IsEmpty ? defaultValue : option.Value;
    }

    private readonly struct SomeOption<T> : IOption<T> {
        internal SomeOption(T value) {
            this.Value = value;
        }

        public bool IsEmpty => false;

        public bool IsDefined => true;

        public T Value { get; }

        public IOption<U> Map<U>(Func<T, U> convert) {
            return new SomeOption<U>(convert(Value));
        }
    }

    private struct NoneOption<T> : IOption<T> {
        public bool IsEmpty => true;

        public bool IsDefined => false;

        public T Value => throw new InvalidOperationException("None has no value");

        public IOption<U> Map<U>(Func<T, U> convert) {
            return new NoneOption<U>();
        }
    }
}
