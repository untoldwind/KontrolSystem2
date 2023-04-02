using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

namespace KontrolSystem.KSP.Runtime.KSPConsole {
    public struct ConsoleLine {
        public readonly int lineNumber;
        public char[] line;

        internal ConsoleLine(int lineNumber, char[] line) {
            this.lineNumber = lineNumber;
            this.line = line;
        }

        internal void Clear() {
            if (line == null) return;
            for (int i = 0; i < line.Length; i++) line[i] = ' ';
        }

        internal bool IsEmpty() {
            if (line != null) {
                for (int i = 0; i < line.Length; i++)
                    if (line[i] != ' ' && line[i] != 0)
                        return false;
            }

            return true;
        }

        public string ContentAsString() {
            if (line.Length == 0) return "";

            int endIdx = line.Length - 1;

            for (; endIdx >= 0 && (line[endIdx] == ' ' || line[endIdx] == 0); endIdx--) ;

            return new String(line, 0, endIdx + 1);
        }

        public override string ToString() => new string(line);

    }

    public class KSPConsoleBuffer {
        private static readonly String[] LineSeparators = new[] { "\r\n", "\n" };

        private readonly LinkedList<ConsoleLine> bufferLines;

        private LinkedListNode<ConsoleLine> topLine;

        private LinkedListNode<ConsoleLine> cursorLine;

        private readonly int maxLines;

        private readonly int maxLineLength;

        private readonly object consoleLock = new object();

        public UnityEvent changed = new UnityEvent();

        public KSPConsoleBuffer(int visibleRows, int visibleCols, int maxLineLength = 1000, int maxLines = 2000) {
            bufferLines = new LinkedList<ConsoleLine>();
            this.VisibleRows = Math.Max(visibleRows, 1);
            this.VisibleCols = Math.Max(visibleCols, 1);
            this.maxLines = maxLines;
            this.maxLineLength = maxLineLength;

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
            changed.Invoke();
        }

        public void Print(string message) => PrintLines(message.Split(LineSeparators, StringSplitOptions.None));

        public void PrintLine(string message) => Print(message + "\n");

        public void ClearLine(int row) {
            lock (consoleLock) {
                bufferLines.FirstOrDefault(line => line.lineNumber == row).Clear();
            }
            changed.Invoke();
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
                    for (int j = 0; CursorCol < maxLineLength && j < line.Length; j++) {
                        cursorLine.Value.line[CursorCol++] = line[j];
                    }
                }
            }
            changed.Invoke();
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
                while (topLine.Previous != null && topLine.Value.IsEmpty())
                    topLine = topLine.Previous;
                int lastNonEmpty = topLine.Value.lineNumber;
                int count = 1;
                while (topLine.Previous != null && count < VisibleRows) {
                    topLine = topLine.Previous;
                    count++;
                }

                CursorRow = Math.Min(CursorRow, lastNonEmpty + 1);
                CursorCol = Math.Min(CursorCol, VisibleCols);

                cursorLine = topLine;
                for (int i = 0; i < CursorRow && cursorLine.Next != null; i++) cursorLine = cursorLine.Next;
            }
        }

        public string ContentAsString() {
            lock (consoleLock) {
                StringBuilder sb = new StringBuilder();

                foreach (var line in bufferLines) {
                    sb.Append(line.ContentAsString());
                    sb.Append("\n");
                }

                return sb.ToString();
            }
        }

        private void AddLines(int count) {
            for (int i = 0; i < count; i++)
                bufferLines.AddLast(new ConsoleLine(bufferLines.Count, new char[maxLineLength]));
            topLine ??= bufferLines.First;
            while (topLine != null && topLine.Value.lineNumber < bufferLines.Count - VisibleRows)
                topLine = topLine.Next;
            while (bufferLines.Count > maxLines) bufferLines.RemoveFirst();
        }
    }
}
