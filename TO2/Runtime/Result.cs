namespace KontrolSystem.TO2.Runtime;

public interface IAnyResult {
    bool Success { get; }

    object? ValueObject { get; }

    string? ErrorString { get; }

    object? ErrorObject { get; }
}

public readonly struct Result<T, E>(bool success, T? value, E? error) : IAnyResult {
    public readonly bool success = success;
    public readonly T? value = value;
    public readonly E? error = error;

    public bool Success => success;

    public object? ValueObject => value;

    public string? ErrorString => error?.ToString();

    public object? ErrorObject => error;
}

public static class Result {
    public static Result<T, E> Ok<T, E>(T value) {
        return new Result<T, E>(true, value, default);
    }

    public static Result<T, E> Err<T, E>(E error) {
        return new Result<T, E>(false, default, error);
    }
}
