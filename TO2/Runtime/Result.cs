namespace KontrolSystem.TO2.Runtime;

public interface IAnyResult {
    bool Success { get; }

    object? ValueObject { get; }

    string? ErrorString { get; }

    Error? ErrorObject { get; }
}

public readonly struct Result<T>(bool success, T? value, Error? error) : IAnyResult {
    public readonly bool success = success;
    public readonly T? value = value;
    public readonly Error? error = error;
    
    public bool Success => success;

    public object? ValueObject => value;

    public string? ErrorString => error?.ToString();

    public Error? ErrorObject => error;
}

public static class Result {
    public static Result<T> Ok<T>(T value) {
        return new Result<T>(true, value, default);
    }

    public static Result<T> Err<T>(string error) {
        return new Result<T>(false, default, new Error(error));
    }

    public static Result<T> Err<T>(Error error) {
        return new Result<T>(false, default, error);
    }
}
