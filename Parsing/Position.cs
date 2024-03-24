using System;

namespace KontrolSystem.Parsing;

public readonly struct Position(string sourceName, int position = 0, int line = 1, int column = 1) : IEquatable<Position> {
    public readonly string sourceName = sourceName;

    public readonly int position = position;

    public readonly int line = line;

    public readonly int column = column;

    public override bool Equals(object obj) {
        return !ReferenceEquals(null, obj) && Equals((Position)obj);
    }

    public bool Equals(Position other) {
        return position == other.position && line == other.line && column == other.column;
    }

    public static bool operator ==(Position left, Position right) {
        return Equals(left, right);
    }

    public static bool operator !=(Position left, Position right) {
        return !Equals(left, right);
    }

    public override int GetHashCode() {
        return HashCode.Combine(position, line, column);
    }

    public override string ToString() {
        return $"{sourceName}({line}, {column})";
    }
}
