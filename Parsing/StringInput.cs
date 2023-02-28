using System;

namespace KontrolSystem.Parsing {
    /// <summary>
    /// Entire input for parsing is available as string.
    /// </summary>
    public readonly struct StringInput : IInput {
        private readonly string source;
        private readonly Position position;

        public StringInput(string source, string sourceName = "<inline>") {
            this.source = source;
            position = new Position(sourceName);
        }

        private StringInput(string source, Position position) {
            this.source = source;
            this.position = position;
        }

        public char Current => source[position.position];

        public int Available => source.Length - position.position;

        public Position Position => position;

        public int FindNext(Predicate<char> predicate) {
            for (int p = position.position; p < source.Length; p++) {
                if (predicate(source[p])) return p - position.position;
            }

            return -1;
        }

        public string Take(int count) {
            if (count == 0) return "";
            if (count + position.position > source.Length) throw new InvalidOperationException("Advance beyond eof");

            return source.Substring(position.position, count);
        }

        public IInput Advance(int count) {
            if (count == 0) return this;
            if (count + position.position > source.Length) throw new InvalidOperationException("Advance beyond eof");

            int line = position.line;
            int column = position.column;

            for (int p = position.position; p < position.position + count; p++) {
                column++;
                if (source[p] == '\n') {
                    line++;
                    column = 1;
                }
            }

            return new StringInput(source, new Position(position.sourceName, position.position + count, line, column));
        }

        public override string ToString() => source.Substring(position.position);
    }
}
