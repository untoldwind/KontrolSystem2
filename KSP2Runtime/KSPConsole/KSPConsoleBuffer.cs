using System;
using System.Collections.Generic;
using System.Linq;

namespace KontrolSystem.KSP.Runtime.KSPConsole {
    public struct ConsoleLine {
        public readonly int lineNumber;
        public char[] line;

        internal ConsoleLine(int lineNumber, char[] line) {
            this.lineNumber = lineNumber;
            this.line = line;
        }

        internal void AdjustCols(int cols) {
            if (line.Length < cols) return;

            char[] newLine = new char[cols];

            Array.Copy(line, newLine, Math.Min(cols, line.Length));

            for (int i = line.Length; i < cols; i++) newLine[i] = ' ';

            line = newLine;
        }

        internal void Clear() {
            if (line == null) return;
            for (int i = 0; i < line.Length; i++) line[i] = ' ';
        }

        public override string ToString() => new string(line);
    }

    public class KSPConsoleBuffer {
        private static readonly String[] LineSeparators = new[] { "\r\n", "\n" };

        private readonly LinkedList<ConsoleLine> bufferLines;

        private LinkedListNode<ConsoleLine> topLine;

        private LinkedListNode<ConsoleLine> cursorLine;

        private readonly int maxLines;

        private readonly object consoleLock = new object();

        public KSPConsoleBuffer(int visibleRows, int visibleCols, int maxLines = 2000) {
            bufferLines = new LinkedList<ConsoleLine>();
            this.VisibleRows = Math.Max(visibleRows, 1);
            this.VisibleCols = Math.Max(visibleCols, 1);
            this.maxLines = maxLines;

            Clear();
        }

        public int CursorCol { get; private set; }

        public int CursorRow { get; private set; }

        public int VisibleCols { get; private set; }

        public int VisibleRows { get; private set; }

        public List<ConsoleLine> VisibleLines {
            get {
                lock (consoleLock) {
                    List<ConsoleLine> lines = new List<ConsoleLine>();
                    LinkedListNode<ConsoleLine> current = topLine;

                    while (current != null) {
                        lines.Add(current.Value);
                        current = current.Next;
                    }

                    return lines;
                }
            }
        }

        public void Clear() {
            lock (consoleLock) {
                bufferLines.Clear();
                topLine = null;
                AddLines(VisibleRows);

                CursorCol = CursorRow = 0;
                cursorLine = topLine;
            }
        }

        public void Print(string message) => PrintLines(message.Split(LineSeparators, StringSplitOptions.None));

        public void PrintLine(string message) => Print(message + "\n");

        public void ClearLine(int row) {
            lock (consoleLock) {
                bufferLines.FirstOrDefault(line => line.lineNumber == row).Clear();
            }
        }

        private void PrintLines(string[] lines) {
            lock (consoleLock) {
                for (int i = 0; i < lines.Length; i++) {
                    if (i > 0) {
                        CursorCol = 0;
                        if (cursorLine.Next == null) {
                            AddLines(1);
                            cursorLine = bufferLines.Last;
                        } else {
                            cursorLine = cursorLine.Next;
                            CursorRow++;
                        }
                    }

                    string line = lines[i];
                    cursorLine.Value.AdjustCols(VisibleCols);
                    for (int j = 0; CursorCol < VisibleCols && j < line.Length; j++)
                        cursorLine.Value.line[CursorCol++] = line[j];
                }
            }
        }

        public void MoveCursor(int row, int col) {
            lock (consoleLock) {
                CursorRow = Math.Max(Math.Min(row, VisibleRows), 0);
                CursorCol = Math.Max(Math.Min(col, VisibleCols), 0);

                cursorLine = topLine;
                for (int i = 0; i < CursorRow && cursorLine.Next != null; i++) cursorLine = cursorLine.Next;
            }
        }

        public void Resize(int rows, int cols) {
            lock (consoleLock) {
                VisibleRows = rows;
                VisibleCols = cols;

                if (bufferLines.Count < VisibleRows)
                    AddLines(VisibleRows - bufferLines.Count);

                topLine = bufferLines.Last;
                while (topLine.Previous != null && topLine.Value.lineNumber >= bufferLines.Count - VisibleRows)
                    topLine = topLine.Previous;

                CursorRow = Math.Min(CursorRow, VisibleRows - 1);
                CursorCol = Math.Min(CursorCol, VisibleCols);

                cursorLine = topLine;
                for (int i = 0; i < CursorRow && cursorLine.Next != null; i++) cursorLine = cursorLine.Next;
            }
        }

        private void AddLines(int count) {
            for (int i = 0; i < count; i++)
                bufferLines.AddLast(new ConsoleLine(bufferLines.Count, new char[VisibleCols]));
            topLine ??= bufferLines.First;
            while (topLine != null && topLine.Value.lineNumber < bufferLines.Count - VisibleRows)
                topLine = topLine.Next;
            while (bufferLines.Count > maxLines) bufferLines.RemoveFirst();
        }
    }
}
