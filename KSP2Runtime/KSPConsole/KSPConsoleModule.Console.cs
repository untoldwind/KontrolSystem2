using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPConsole {
    public partial class KSPConsoleModule {
        [KSClass("Console", Description = "Representation of a console")]
        public class Console {
            [KSMethod(
                Description = "Clear the console of all its content and move cursor to (0, 0)."
            )]
            public void Clear() => KSPContext.CurrentContext.ConsoleBuffer?.Clear();

            [KSMethod(
                Description = "Print a message at the current cursor position (and move cursor forward)"
            )]
            public void Print(string message) => KSPContext.CurrentContext.ConsoleBuffer?.Print(message);

            [KSMethod(
                Description =
                    "Print a message at the current cursor position and move cursor to the beginning of the next line."
            )]
            public void PrintLine(string message) => KSPContext.CurrentContext.ConsoleBuffer?.PrintLine(message);

            [KSMethod(
                Description =
                    "Moves the cursor to the specified position, prints the message and restores the previous cursor position"
            )]
            public void PrintAt(long row, long column, string message) {
                var consoleBuffer = KSPContext.CurrentContext.ConsoleBuffer;
                if (consoleBuffer == null) return;
                var origRow = consoleBuffer.CursorRow;
                var origCol = consoleBuffer.CursorCol;
                consoleBuffer.MoveCursor((int)row, (int)column);
                consoleBuffer.Print(message);
                consoleBuffer.MoveCursor(origRow, origCol);
            }

            [KSMethod(
                Description = "Move the cursor to a give `row` and `column`."
            )]
            public void MoveCursor(long row, long column) =>
                KSPContext.CurrentContext.ConsoleBuffer?.MoveCursor((int)row, (int)column);

            [KSMethod(
                Description = "Clear a line"
            )]
            public void ClearLine(long row) => KSPContext.CurrentContext.ConsoleBuffer?.ClearLine((int)row);

            [KSField]
            public long CursorRow {
                get => KSPContext.CurrentContext.ConsoleBuffer?.CursorRow ?? 0;
            }

            [KSField]
            public long CursorCol {
                get => KSPContext.CurrentContext.ConsoleBuffer?.CursorCol ?? 0;
            }
        }
    }
}
