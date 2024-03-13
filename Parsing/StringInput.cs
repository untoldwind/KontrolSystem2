using System;

namespace KontrolSystem.Parsing;

/// <summary>
///     Entire input for parsing is available as string.
/// </summary>
public readonly ref struct StringInput(ReadOnlySpan<char> source, string sourceName, int position = 0, int line = 1, int column = 1) {
    private readonly ReadOnlySpan<char> source = source;
    
    public char Current => source[0];

    public int Available => source.Length;

    public Position Position => new(sourceName, position, line, column);

    public int FindNext(Predicate<char> predicate) {
        for (var p = 0; p < source.Length; p++)
            if (predicate(source[p]))
                return p;

        return -1;
    }

    public bool Match(ReadOnlySpan<char> tag) => source.StartsWith(tag);

    public string Take(int count) {
        if (count == 0) return "";
        if (count > source.Length) throw new InvalidOperationException("Advance beyond eof");

        return source.Slice(0, count).ToString();
    }

    public StringInput Advance(int count) {
        if (count == 0) return this;
        if (count > source.Length) throw new InvalidOperationException("Advance beyond eof");

        var nextLine = line;
        var nextColumn = column;

        for (var p = 0; p < count; p++) {
            nextColumn++;
            if (source[p] == '\n') {
                nextLine++;
                nextColumn = 1;
            }
        }

        return new StringInput(source.Slice(count), sourceName, position + count, nextLine, nextColumn);
    }

    public override string ToString() {
        return source.ToString();
    }
}
