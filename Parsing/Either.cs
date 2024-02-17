using System;

namespace KontrolSystem.Parsing;

public interface IEither<out L, out R> {
    bool IsLeft { get; }

    bool IsRight { get; }

    L Left { get; }

    R Right { get; }
}

public static class Either {
    public static IEither<L, R> Left<L, R>(L value) {
        return new LeftEither<L, R>(value);
    }

    public static IEither<L, R> Right<L, R>(R value) {
        return new RightEither<L, R>(value);
    }

    private struct LeftEither<L, R> : IEither<L, R> {
        internal LeftEither(L value) {
            this.Left = value;
        }

        public bool IsLeft => true;

        public bool IsRight => false;

        public L Left { get; }

        public R Right => throw new InvalidOperationException("Either has no right");
    }

    private struct RightEither<L, R> : IEither<L, R> {
        internal RightEither(R value) {
            this.Right = value;
        }

        public bool IsLeft => false;

        public bool IsRight => true;

        public L Left => throw new InvalidOperationException("Either has no left");

        public R Right { get; }
    }
}
