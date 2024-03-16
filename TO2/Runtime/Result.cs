namespace KontrolSystem.TO2.Runtime;

public interface IAnyResult {
    bool Success { get; }

    object? ValueObject { get; }

    string? ErrorString { get; }

    CoreError.Error? ErrorObject { get; }
}

public readonly struct Result<T>(bool success, T? value, CoreError.Error? error) : IAnyResult {
    public readonly bool success = success;
    public readonly T? value = value;
    public readonly CoreError.Error? error = error;

    public bool Success => success;

    public object? ValueObject => value;

    public string? ErrorString => error?.ToString();

    public CoreError.Error? ErrorObject => error;
}

public static class Result {
    public static Result<T> Ok<T>(T value) {
        return new Result<T>(true, value, default);
    }

    public static Result<T> Err<T>(string error) {
        return new Result<T>(false, default, new CoreError.Error(error, ContextHolder.CurrentContext.Value?.CurrentStack() ?? []));
    }

    public static Result<T> Err<T>(CoreError.Error error) {
        return new Result<T>(false, default, error);
    }
}
