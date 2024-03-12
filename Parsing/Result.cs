using System;
using System.Collections.Generic;
using System.Linq;

namespace KontrolSystem.Parsing;

public delegate Result<U> SelectConvert<T, U>(Result<T> result);

public readonly ref struct Result<T>( bool success, IInput remaining, T value, List<string> expected) {
    
    public readonly T value = value;

    public readonly IInput remaining = remaining;

    public readonly bool success = success;

    public readonly List<string> expected = expected;

    public Position Position {
        get => remaining.Position;
    }

    public Result<U> To<U>(U fixedValue) => new(success, remaining, fixedValue, expected);
    
    public Result<U> Map<U>(Func<T, U> f) => new(success, remaining, success ? f(value) : default!, expected);

    public Result<U> Select<U>(SelectConvert<T, U> next) => success ? next(this) : new(success, remaining, default!, expected);
}

public static class Result {
    public static Result<T> Success<T>(IInput remaining, T value) {
        return new Result<T>(true, remaining, value, []);
    }

    public static Result<T> Failure<T>(IInput input, string expected) {
        return new Result<T>(false, input,default!, [expected]);
    }

    public static Result<T> Failure<T>(IInput input, IEnumerable<string> expected) {
        return new Result<T>(false, input, default!, expected.ToList());
    }
}
