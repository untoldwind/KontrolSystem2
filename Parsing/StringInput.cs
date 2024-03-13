using System;

namespace KontrolSystem.Parsing;

/// <summary>
///     Entire input for parsing is available as string.
/// </summary>
public readonly ref struct StringInput(string source, string sourceName, int position = 0, int line = 1, int column = 1) {
    public char Current => source[position];

    public int Available => source.Length - position;

    public Position Position => new (sourceName, position, line, column);

    public int FindNext(Predicate<char> predicate) {
        for (var p = position; p < source.Length; p++)
            if (predicate(source[p]))
                return p - position;

        return -1;
    }

    public string Take(int count) {
        if (count == 0) return "";
        if (count + position > source.Length) throw new InvalidOperationException("Advance beyond eof");

        return source.Substring(position, count);
    }

    public StringInput Advance(int count) {
        if (count == 0) return this;
        if (count + position > source.Length) throw new InvalidOperationException("Advance beyond eof");

        var nextLine = line;
        var nextColumn = column;

        for (var p = position; p < position + count; p++) {
            nextColumn++;
            if (source[p] == '\n') {
                nextLine++;
                nextColumn = 1;
            }
        }

        return new StringInput(source, sourceName, position + count, nextLine, nextColumn);
    }

    public override string ToString() {
        return source.Substring(position);
    }
}
