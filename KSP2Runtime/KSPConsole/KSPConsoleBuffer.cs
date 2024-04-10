using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KontrolSystem.KSP.Runtime.Core;
using UnityEngine;
using UnityEngine.Events;

namespace KontrolSystem.KSP.Runtime.KSPConsole;

public struct ConsoleLine {
    public readonly int lineNumber;
    public char[] line;

    internal ConsoleLine(int lineNumber, char[] line) {
        this.lineNumber = lineNumber;
        this.line = line;
    }

    internal readonly void Clear() {
        if (line == null) return;
        for (var i = 0; i < line.Length; i++) line[i] = ' ';
    }

    internal readonly bool IsEmpty() {
        if (line != null)
            for (var i = 0; i < line.Length; i++)
                if (line[i] != ' ' && line[i] != 0)
                    return false;

        return true;
    }

    internal void EnsureColumnVisible(int col) {
        for(var i = 0; i < col; i++)
            if (line[i] == '\0')
                line[i] = ' ';
    }
    
    public readonly string ContentAsString() {
        if (line.Length == 0) return "";

        var endIdx = line.Length - 1;

        for (; endIdx >= 0 && (line[endIdx] == ' ' || line[endIdx] == 0); endIdx--) ;

        return new string(line, 0, endIdx + 1);
    }

    public readonly override string ToString() {
        return new string(line);
    }

    internal readonly string DisplayText() =>
        "<noparse>" + ContentAsString() + "</noparse>";
}

public class KSPConsoleBuffer {
    private static readonly string[] LineSeparators = ["\r\n", "\n"];

    private readonly LinkedList<ConsoleLine> bufferLines;

    private ConsolePrompt prompt;

    private volatile bool promptLocked;

    private readonly object consoleLock = new();

    private readonly int maxLineLength;

    private readonly int maxLines;

    public UnityEvent changed = new();

    private LinkedListNode<ConsoleLine>? cursorLine;

    private LinkedListNode<ConsoleLine>? topLine;

    public KSPConsoleBuffer(int visibleRows, int visibleCols, int maxLineLength = 1000, int maxLines = 2000) {
        var commandShell = new CommandShell(this);
        prompt = new ConsolePrompt(commandShell, maxLineLength);
        bufferLines = new LinkedList<ConsoleLine>();
        VisibleRows = Math.Max(visibleRows, 1);
        VisibleCols = Math.Max(visibleCols, 1);
        this.maxLines = maxLines;
        this.maxLineLength = maxLineLength;

        Clear();
    }

    public int CursorCol { get; private set; }

    public int CursorRow { get; private set; }

    public int VisibleCols { get; private set; }

    public int VisibleRows { get; private set; }

    public List<ConsoleLine> VisibleLines(int maxVisibleLines) {
        lock (consoleLock) {
            return bufferLines.Reverse().SkipWhile(line => line.IsEmpty()).Take(maxVisibleLines).Reverse().ToList();
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
        prompt.Clear();

        changed.Invoke();
    }

    public void Print(string message) {
        PrintLines(message.Split(LineSeparators, StringSplitOptions.None));
    }

    public void PrintLine(string message) {
        Print(message + "\n");
    }

    public void ClearLine(int row) {
        lock (consoleLock) {
            bufferLines.FirstOrDefault(line => line.lineNumber == row).Clear();
        }

        changed.Invoke();
    }

    private void PrintLines(string[] lines) {
        lock (consoleLock) {
            for (var i = 0; i < lines.Length; i++) {
                if (i > 0) {
                    CursorCol = 0;
                    if (cursorLine?.Next == null) {
                        AddLines(1);
                        cursorLine = bufferLines.Last;
                    } else {
                        cursorLine = cursorLine.Next;
                        CursorRow++;
                    }
                }

                var line = lines[i];
                for (var j = 0; CursorCol < maxLineLength && j < line.Length; j++)
                    cursorLine!.Value.line[CursorCol++] = line[j];
            }
        }

        changed.Invoke();
    }

    public void MoveCursor(int row, int col) {
        lock (consoleLock) {
            CursorRow = Math.Max(Math.Min(row, VisibleRows), 0);
            CursorCol = Math.Max(Math.Min(col, VisibleCols), 0);

            cursorLine = topLine;
            for (var i = 0; i < CursorRow && cursorLine?.Next != null; i++) cursorLine = cursorLine.Next;
            cursorLine?.Value.EnsureColumnVisible(CursorCol);
        }
    }

    public void Resize(int rows, int cols) {
        lock (consoleLock) {
            if (VisibleRows == rows && VisibleCols == cols) return;

            VisibleRows = rows;
            VisibleCols = cols;

            if (bufferLines.Count < VisibleRows)
                AddLines(VisibleRows - bufferLines.Count);

            topLine = bufferLines.Last;
            while (topLine.Previous != null && topLine.Value.IsEmpty())
                topLine = topLine.Previous;
            var lastNonEmpty = topLine.Value.lineNumber;
            var count = 1;
            while (topLine.Previous != null && count < VisibleRows) {
                topLine = topLine.Previous;
                count++;
            }

            CursorRow = Math.Min(CursorRow, lastNonEmpty + 1);
            CursorCol = Math.Min(CursorCol, VisibleCols);

            cursorLine = topLine;
            for (var i = 0; i < CursorRow && cursorLine.Next != null; i++) cursorLine = cursorLine.Next;
        }
    }

    public string ContentAsString() {
        lock (consoleLock) {
            var sb = new StringBuilder();

            foreach (var line in bufferLines) {
                sb.Append(line.ContentAsString());
                sb.Append("\n");
            }

            return sb.ToString();
        }
    }

    private void AddLines(int count) {
        for (var i = 0; i < count; i++)
            bufferLines.AddLast(new ConsoleLine(bufferLines.Count, new char[maxLineLength]));
        topLine ??= bufferLines.First;
        while (topLine != null && topLine.Value.lineNumber < bufferLines.Count - VisibleRows)
            topLine = topLine.Next;
        while (bufferLines.Count > maxLines) bufferLines.RemoveFirst();
    }

    internal void SetFocus(bool focus) {
        prompt.SetFocus(focus);
        changed.Invoke();
    }

    internal void SetPromptLock(bool locked) {
        if (promptLocked != locked) {
            promptLocked = locked;
            changed.Invoke();
        }
    }

    internal bool HandleKey(KeyCode keyCode, char character, EventModifiers modifiers) {
        switch (keyCode) {
        case KeyCode.Escape:
            Mainframe.Instance!.AbortCommands();
            return false;
        default:
            if (promptLocked) return true;
            prompt.HandleKey(keyCode, character, modifiers);
            changed.Invoke();
            return true;
        }
    }

    internal string DisplayText() {
        var promptLines = promptLocked ? [] : prompt.DisplayTexts();

        return string.Join("\n",
            VisibleLines(VisibleRows - promptLines.Count).Select(line => line.DisplayText())
                .Concat(promptLines));
    }
}
