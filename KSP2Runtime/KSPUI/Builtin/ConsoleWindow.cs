using System;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.KSP.Runtime.Core;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KontrolSystem.KSP.Runtime.KSPUI.Builtin {
    public class ConsoleWindow : UGUIResizableWindow {
        private KSPConsoleBuffer consoleBuffer;
        private TextMeshProUGUI consoleText;
        private UGUIInputField commandInputField;
        private Button commandHistoryUpButton;
        private Button commandHistoryDownButton;
        private float charWidth;
        private float charHeight;

        public List<string> commandHistory = new List<string>();
        public int commandHistoryIndex; // a value of commandHistory.Count indicates that we're not reading from the command history

        public void OnEnable() {
            Initialize("KontrolSystem: Console", new Rect(200, Screen.height - 200, 400, 500));

            var root = RootVerticalLayout();

            GameObject consoleBackground = new GameObject("ConsoleBackground", typeof(Image));
            root.Add(consoleBackground, UGUILayout.Align.Stretch, new Vector2(200, 200), 1);
            Image image = consoleBackground.GetComponent<Image>();
            image.sprite = UIFactory.Instance.consoleBackground;
            image.type = Image.Type.Sliced;
            image.color = Color.white;

            GameObject consoleFrame = new GameObject("ConsoleFrame", typeof(Image));
            UIFactory.Layout(consoleFrame, consoleBackground.transform, UIFactory.LAYOUT_STRETCH,
                UIFactory.LAYOUT_STRETCH, 0, 0, 0, 0);
            Image frameImage = consoleFrame.GetComponent<Image>();
            frameImage.sprite = UIFactory.Instance.consoleInactiveFrame;
            frameImage.type = Image.Type.Sliced;
            frameImage.color = Color.white;

            GameObject console = new GameObject("Console", typeof(TextMeshProUGUI));
            UIFactory.Layout(console, consoleBackground.transform, UIFactory.LAYOUT_STRETCH, UIFactory.LAYOUT_STRETCH,
                10, -10, -20, -20);
            consoleText = console.GetComponent<TextMeshProUGUI>();
            consoleText.font = UIFactory.Instance.consoleFont;
            consoleText.horizontalAlignment = HorizontalAlignmentOptions.Left;
            consoleText.verticalAlignment = VerticalAlignmentOptions.Top;
            consoleText.fontSize = UIFactory.Instance.consoleFontSize;
            consoleText.color = new Color(0.5f, 1.0f, 0.5f, 1.0f);

            var fontScale = consoleText.fontSize / consoleText.font.faceInfo.pointSize;
            charHeight = consoleText.font.faceInfo.lineHeight * fontScale;
            charWidth = consoleText.font.glyphTable[0].metrics.horizontalAdvance * fontScale;

            var replContainer = root.Add(UGUILayoutContainer.Horizontal(2));

            commandInputField = UGUIInputField.Create("", 120);
            replContainer.Add(commandInputField, UGUILayout.Align.Stretch, 1);

            var replHistoryContainer = replContainer.Add(UGUILayoutContainer.Vertical(0));

            GameObject replHistoryUp = UIFactory.Instance.CreateIconButton(UIFactory.Instance.upIcon);
            replHistoryContainer.Add(replHistoryUp, UGUILayout.Align.Stretch, new Vector2(15, 15));
            commandHistoryUpButton = replHistoryUp.GetComponent<Button>();
            commandHistoryUpButton.onClick.AddListener(OnCommandHistoryUp);
            commandHistoryUpButton.interactable = false;

            GameObject replHistoryDown = UIFactory.Instance.CreateIconButton(UIFactory.Instance.downIcon);
            replHistoryContainer.Add(replHistoryDown, UGUILayout.Align.Stretch, new Vector2(15, 15));
            commandHistoryDownButton = replHistoryDown.GetComponent<Button>();
            commandHistoryDownButton.onClick.AddListener(OnCommandHistoryDown);
            commandHistoryDownButton.interactable = false;

            GameObject replStartStop = UIFactory.Instance.CreateIconButton(UIFactory.Instance.startIcon);
            replContainer.Add(replStartStop, UGUILayout.Align.Stretch, new Vector2(30, 30));
            replStartStop.GetComponent<Button>().onClick.AddListener(OnRunCommand);

            var buttonContainer = root.Add(UGUILayoutContainer.Horizontal(20));

            buttonContainer.Add(UGUIButton.Create("Clear", () => consoleBuffer.Clear()));
            buttonContainer.Add(UGUIButton.Create("Copy", () => {
                GUIUtility.systemCopyBuffer = consoleBuffer.ContentAsString();
            }));

            MinSize = root.Layout();

            consoleBuffer = Mainframe.Instance.ConsoleBuffer;
            consoleBuffer.changed.AddListener(OnConsoleBufferChanged);

            OnConsoleBufferChanged();
        }

        public override void OnDisable() {
            base.OnDisable();

            consoleBuffer.changed.RemoveListener(OnConsoleBufferChanged);
        }

        protected override void OnResize(Vector2 delta) {
            base.OnResize(delta);

            OnConsoleBufferChanged();
        }

        private void OnConsoleBufferChanged() {
            var textRect = consoleText.GetComponent<RectTransform>().rect;

            consoleBuffer.Resize((int)(textRect.height / charHeight), (int)(textRect.width / charWidth));

            consoleText.SetText(String.Join("\n", consoleBuffer.VisibleLines.Select(line => line.ToString().Replace('\0', ' ').Substring(0, consoleBuffer?.VisibleCols ?? 0))));
        }

        private void OnRunCommand() {
            var replText = commandInputField.Value;

            if (!string.IsNullOrWhiteSpace(replText)) {
                commandHistory.Add(replText);
                Mainframe.Instance.Logger.Debug($"Submitted: {replText}");
                consoleBuffer?.PrintLine($"$> {replText}");
                try {
                    var result = REPLExpression.Run(replText);
                    if (result != null) {
                        consoleBuffer?.PrintLine($"{result}");
                    }
                } catch (Exception e) {
                    consoleBuffer?.PrintLine($"{e}");
                }
                commandHistoryIndex = commandHistory.Count;
                UpdateCommandHistory();

            }
        }

        private void OnCommandHistoryUp() {
            if (commandHistoryIndex > 0) {
                commandHistoryIndex--;
                UpdateCommandHistory();
            }
        }

        private void OnCommandHistoryDown() {
            if (commandHistoryIndex < commandHistory.Count) {
                commandHistoryIndex++;
                UpdateCommandHistory();
            }
        }

        private void UpdateCommandHistory() {
            if (commandHistoryIndex >= 0 && commandHistoryIndex < commandHistory.Count) {
                commandInputField.Value = commandHistory[commandHistoryIndex];
            } else {
                commandInputField.Value = "";
            }

            commandHistoryUpButton.interactable = commandHistoryIndex > 0;
            commandHistoryDownButton.interactable = commandHistoryIndex < commandHistory.Count;

            Mainframe.Instance.Logger.Info($"Command history: {string.Join(", ", commandHistory)} ({commandHistory.Count} command(s)); Command history index: {commandHistoryIndex}; REPL text: {commandInputField.Value}");
        }
    }
}
