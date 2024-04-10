using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Experiments {
    public class ConsolePrompt {
        private readonly object promptLock = new();

        private readonly CommandShell commandShell;
        private readonly int maxLineLength;

        internal int caretRow = 0;
        internal int caretCol = 0;
        internal bool focus = false;
        private List<ConsoleLine> promptLines = new();

        private List<List<ConsoleLine>> history = new();

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
                    if (caretCol > 0) {
                        Array.Copy(line.line, caretCol, line.line, caretCol - 1, maxLineLength - caretCol);
                        caretCol--;
                    } else if (caretRow > 0) {
                        var prev = promptLines[caretRow-1];
                        var prevLast = prev.line.Count(ch => ch != 0);
                        Array.Copy(line.line, 0, prev.line, prevLast, maxLineLength - prevLast);
                        caretRow--;
                        caretCol = prevLast;
                        promptLines.Remove(line);
                        for (var i = 0; i < promptLines.Count; i++) {
                            promptLines[i] = new ConsoleLine(i, promptLines[i].line);
                        }
                    }
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
                    caretRow = 0;
                    caretCol = 0;
                    break;
                case KeyCode.End:
                    caretRow = promptLines.Last().lineNumber;
                    caretCol = promptLines.Last().line.Count(ch => ch != 0);
                    break;
                case KeyCode.UpArrow:
                    if (caretRow > 0) {
                        caretRow--;
                        caretCol = Math.Min(caretCol, promptLines[caretRow].line.Count(ch => ch != 0));
                        return;
                    }
                    if (commandHistoryIndex == 0) return;
                    commandHistoryIndex--;
                    promptLines = history[commandHistoryIndex].Select(l => l.Clone()).ToList();
                    caretCol = promptLines.Last().ContentAsString().Length;
                    caretRow = promptLines.Last().lineNumber;
                    break;
                case KeyCode.DownArrow:
                    if (caretRow < promptLines.Count - 1) {
                        caretRow++;
                        caretCol = Math.Min(caretCol, promptLines[caretRow].line.Count(ch => ch != 0));
                        return;
                    }
                    if (commandHistoryIndex == history.Count) return;
                    commandHistoryIndex++;
                    if (commandHistoryIndex == history.Count)
                        promptLines = new List<ConsoleLine> { new(0, new char[maxLineLength]) };
                    else
                        promptLines = history[commandHistoryIndex].Select(l => l.Clone()).ToList();
                    caretCol = promptLines.Last().ContentAsString().Length;
                    caretRow = promptLines.Last().lineNumber;
                    break;
                case KeyCode.Return:
                    if (modifiers == EventModifiers.Shift) {
                        var newLine = new char[maxLineLength];
                        Array.Copy(line.line, caretCol, newLine, 0, maxLineLength - caretCol);
                        for (int i = caretCol; i < maxLineLength; i++) line.line[i] = '\0';
                        promptLines.Insert(line.lineNumber + 1, new(promptLines.Count, newLine) );
                        for (var i = 0; i < promptLines.Count; i++) {
                            promptLines[i] = new ConsoleLine(i, promptLines[i].line);
                        }
                        caretCol = 0;
                        caretRow++;
                        return;
                    } 
                    
                    if (promptLines.Count == 0 || (promptLines.Count == 1 && promptLines.First().IsEmpty())) return;
                    commandText = string.Join("\n", promptLines.Select(l => l.ContentAsString()));
                    history.Add(promptLines);
                    commandHistoryIndex = history.Count;
                    promptLines = new List<ConsoleLine> { new(0, new char[maxLineLength]) };
                    caretCol = 0;
                    caretRow = 0;

                    break;
                default:
                    if (character < 32 || character == 127 || caretCol >= maxLineLength - 1) return;
                    Array.Copy(line.line, caretCol, line.line, caretCol + 1, maxLineLength - caretCol - 1);
                    line.line[caretCol++] = character;
                    break;
                }
            }

            if (commandText != null) {
                commandShell.RunCommand(commandText);
            }
        }

        internal string RenderLine(ConsoleLine line) {
            var endIdx = line.line.Count(ch => ch != 0);
            var prefix = line.lineNumber == 0 ? "$> " : " | ";
            if (!focus || line.lineNumber != caretRow || line.line.Length == 0) {
                return $"{prefix}{new string(line.line, 0, endIdx)}";
            }

            if (caretCol > endIdx) return $"{prefix}{line.ContentAsString()}<mark=green>\u2588</mark>";

            var sb = new StringBuilder();
            sb.Append(prefix);
            sb.Append(line.line, 0, caretCol);
            sb.Append("<mark=green>");
            var caretChar = line.line[caretCol];
            sb.Append(caretChar > 32 ? caretChar : '\u2588');
            sb.Append("</mark>");
            if (endIdx > caretCol + 1) sb.Append(line.line, caretCol + 1, endIdx - caretCol - 1);

            return sb.ToString();
        }

        internal List<String> DisplayTexts() {
            lock (promptLock) {
                return promptLines.Select(RenderLine).ToList();
            }
        }
    }
}
