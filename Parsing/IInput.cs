using System;

namespace KontrolSystem.Parsing {
    /// <summary>
    /// Abstract parser input
    /// </summary>
    public interface IInput {
        /// <summary>
        /// Gets the current <see cref="System.Char" />.
        /// </summary>
        char Current { get; }

        /// <summary>
        /// Number of available characters in input.
        /// </summary>
        int Available { get; }

        /// <summary>
        /// Gets the current positon.
        /// </summary>
        Position Position { get; }

        int FindNext(Predicate<char> predicate);

        string Take(int count);

        IInput Advance(int count);
    }
}
