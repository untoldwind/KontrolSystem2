using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KontrolSystem.KSP.Runtime.Core;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPConsole;

public class ConsolePrompt {
    private readonly object promptLock = new();

    private readonly CommandShell commandShell;
    private readonly int maxLineLength;

    internal int caretRow = 0;
    internal int caretCol = 0;
    internal bool focus = false;

    private List<ConsoleLine> promptLines = [];

    private List<List<ConsoleLine>> history = [];

    private int commandHistoryIndex = 0;

    internal ConsolePrompt(CommandShell commandShell, int maxLineLength) {
        this.commandShell = commandShell;
        this.maxLineLength = maxLineLength;
    }

    internal void SetFocus(bool focus) {
        lock (promptLock) {
            this.focus = focus;
            if (focus && promptLines.Count == 0) {
                promptLines.Add(new ConsoleLine(0, new char[maxLineLength]));
            } else if (!focus && promptLines.Count == 1 && promptLines.First().IsEmpty()) {
                caretRow = 0;
                caretCol = 0;
                promptLines.Clear();
            }
        }
    }

    internal void Clear() {
        lock (promptLock) {
            if (focus) return;
            caretRow = 0;
            caretCol = 0;
            promptLines.Clear();
        }
    }

    internal void HandleKey(KeyCode keyCode, char character, EventModifiers modifiers) {
        string? commandText = null;

        lock (promptLock) {
            if (caretRow > promptLines.Count) return;

            var line = promptLines[caretRow];

            switch (keyCode) {
            case KeyCode.Backspace:
                if (caretCol == 0) return;
                Array.Copy(line.line, caretCol, line.line, caretCol - 1, maxLineLength - caretCol);
                caretCol--;
                break;
            case KeyCode.Delete:
                Array.Copy(line.line, caretCol + 1, line.line, caretCol, maxLineLength - caretCol - 1);
                break;
            case KeyCode.LeftArrow:
                if (caretCol > 0) caretCol--;
                break;
            case KeyCode.RightArrow:
                var endIdx = line.line.Count(ch => ch != 0);
                if (caretCol < endIdx) caretCol++;
                break;
            case KeyCode.Home:
                caretCol = 0;
                break;
            case KeyCode.End:
                caretCol = line.line.Count(ch => ch != 0);
                break;
            case KeyCode.UpArrow:
                if (commandHistoryIndex == 0) return;
                commandHistoryIndex--;
                promptLines = history[commandHistoryIndex];
                caretCol = 0;
                caretRow = 0;
                break;
            case KeyCode.DownArrow:
                if (commandHistoryIndex == history.Count) return;
                commandHistoryIndex++;
                if (commandHistoryIndex == history.Count)
                    promptLines = new List<ConsoleLine> { new(0, new char[maxLineLength]) };
                else
                    promptLines = history[commandHistoryIndex];
                caretCol = 0;
                caretRow = 0;
                break;
            case KeyCode.Return:
                if (promptLines.Count == 0 || (promptLines.Count == 1 && promptLines.First().IsEmpty())) return;
                commandText = string.Join("\n", promptLines.Select(l => l.ContentAsString()));
                history.Add(promptLines);
                commandHistoryIndex = history.Count;
                promptLines = new List<ConsoleLine> { new(0, new char[maxLineLength]) };
                caretCol = 0;
                caretRow = 0;
                break;
            default:
                if (modifiers is EventModifiers.Control or EventModifiers.Command) {
                    switch (keyCode) {
                    case KeyCode.C:
                        Mainframe.Instance!.AbortCommands();
                        break;
                    }
                } else {
                    if (character < 32 || character == 127 || caretCol >= maxLineLength - 1) return;
                    Array.Copy(line.line, caretCol, line.line, caretCol + 1, maxLineLength - caretCol - 1);
                    line.line[caretCol++] = character;
                }

                break;
            }
        }

        if (commandText != null) {
            commandShell.RunCommand(commandText);
        }
    }

    internal string RenderLine(ConsoleLine line) {
        var endIdx = line.line.Count(ch => ch != 0);
        if (!focus || line.lineNumber != caretRow || line.line.Length == 0) return $"$> {new string(line.line, 0, endIdx)}";

        if (caretCol > endIdx) return $"$> {line.ContentAsString()}<mark=green>\u2588</mark>";

        var sb = new StringBuilder();
        sb.Append("$> ");
        sb.Append(line.line, 0, caretCol);
        sb.Append("<mark=green>");
        var caretChar = line.line[caretCol];
        sb.Append(caretChar > 32 ? caretChar : '\u2588');
        sb.Append("</mark>");
        if (endIdx > caretCol) sb.Append(line.line, caretCol + 1, endIdx - caretCol);

        return sb.ToString();
    }

    internal List<String> DisplayTexts() {
        lock (promptLock) {
            return promptLines.Select(RenderLine).ToList();
        }
    }
}
