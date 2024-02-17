using System;

namespace KontrolSystem.TO2.Runtime;

public interface IAnyOption {
    bool Defined { get; }

    object? ValueObject { get; }
}

public struct Option<T> : IAnyOption {
    public readonly bool defined;
    public readonly T value;

    public Option(T value) {
        defined = true;
        this.value = value;
    }

    public bool Defined => defined;

    public object? ValueObject => value;

    public Option<U> Map<U>(Func<T, U> mapper) {
        return defined ? new Option<U>(mapper(value)) : new Option<U>();
    }

    public Option<U> Then<U>(Func<T, Option<U>> mapper) {
        return defined ? mapper(value) : new Option<U>();
    }

    public Result<T, E> OkOr<E>(E error) {
        return defined ? new Result<T, E>(true, value, default) : new Result<T, E>(false, default, error);
    }

    public T GetValueOrDefault(T defaultValue) {
        return defined ? value : defaultValue;
    }
}

public static class Option {
    public static Option<T> Some<T>(T value) {
        return new Option<T>(value);
    }

    public static Option<T> None<T>() {
        return new Option<T>();
    }

    public static Option<T> OfNullable<T>(T? value) {
        return value != null ? new Option<T>(value) : new Option<T>();
    }
}
