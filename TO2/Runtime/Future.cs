namespace KontrolSystem.TO2.Runtime;

public interface IAnyFutureResult {
    bool IsReady { get; }

    object? ValueObject { get; }
}

public readonly struct FutureResult<T>(T value) : IAnyFutureResult {
    public readonly bool ready = true;
    public readonly T value = value;

    public bool IsReady => ready;

    public object? ValueObject => value;
}

public interface IAnyFuture {
    IAnyFutureResult Poll();
}

public abstract class Future<T> : IAnyFuture {
    public IAnyFutureResult Poll() {
        return PollValue();
    }

    public abstract FutureResult<T> PollValue();
}

public static class Future {
    public class Success<T>(T value) : Future<T> {
        private readonly T value = value;

        public override FutureResult<T> PollValue() {
            return new FutureResult<T>(value);
        }
    }
}
