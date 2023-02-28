using System;

namespace KontrolSystem.Parsing {
    public readonly struct Position : IEquatable<Position> {
        public readonly string sourceName;

        public readonly int position;

        public readonly int line;

        public readonly int column;

        public Position(string sourceName, int position = 0, int line = 1, int column = 1) {
            this.sourceName = sourceName;
            this.position = position;
            this.line = line;
            this.column = column;
        }

        public override bool Equals(object obj) =>
            !ReferenceEquals(null, obj) && Equals((Position)obj);

        public bool Equals(Position other) =>
            position == other.position && line == other.line && column == other.column;

        public static bool operator ==(Position left, Position right) => Equals(left, right);

        public static bool operator !=(Position left, Position right) => !Equals(left, right);

        public override int GetHashCode() {
            var h = 31;
            h = h * 13 + position;
            h = h * 13 + line;
            h = h * 13 + column;
            return h;
        }

        public override string ToString() => $"{sourceName}({line}, {column})";
    }
}
