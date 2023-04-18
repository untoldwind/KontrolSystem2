using System;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPTelemetry;
using SpaceWarp.API.Assets;
using KontrolSystem.TO2.Runtime;
using SpaceWarp.API.UI;
using UnityEngine;
using static KontrolSystem.KSP.Runtime.KSPOrbit.KSPOrbitModule;

namespace KontrolSystem.SpaceWarpMod.UI {
    public class ConsoleWindow : ResizableWindow {
        private static readonly Color Color = new Color(1f, 1f, 1f, 1.1f); // opaque window color when focused
        private static readonly Color BgColor = new Color(0.0f, 0.0f, 0.0f, 1.0f); // black background of terminal
        private static readonly Color TextColor = new Color(0.5f, 1.0f, 0.5f, 1.0f); // font color on terminal

        private static readonly GUIStyle tinyButtonStyle = new GUIStyle(Skins.ConsoleSkin.button) {
            padding = new RectOffset(0, 0, 0, 0)
        };

        private GUIStyle terminalImageStyle;
        private GUIStyle terminalFrameStyle;
        private GUIStyle terminalFrameActiveStyle;
        private GUISkin terminalLetterSkin;
        private KSPConsoleBuffer consoleBuffer;
        private TimeSeriesCollection timeSeriesCollection;
        private int fontCharWidth;
        private int fontCharHeight;

        private string replText = "";

        public event Action OnCloseClicked;

        public List<string> commandHistory = new List<string>();
        public int commandHistoryIndex = 0; // a value of commandHistory.Count indicates that we're not reading from the command history

        public void Toggle() {
            if (!isOpen) Open();
            else Close();
        }

        public void AttachTo(KSPConsoleBuffer consoleBuffer, TimeSeriesCollection timeSeriesCollection) {
            this.consoleBuffer = consoleBuffer;
            this.timeSeriesCollection = timeSeriesCollection;
            if (this.consoleBuffer == null) return;

            windowRect = new Rect(windowRect.xMin, windowRect.yMin, this.consoleBuffer.VisibleCols * fontCharWidth + 65,
                this.consoleBuffer.VisibleRows * fontCharHeight + 140);
        }

        // --------------------- MonoBehaviour callbacks ------------------------

        public void Awake() {
            Initialize("KontrolSystem: Console", new Rect(60, 60, 100, 100), 120, 120, true);

            Texture2D terminalImage = GFXAdapter.GetTexture("monitor_minimal");
            Texture2D terminalFrameImage = GFXAdapter.GetTexture("monitor_minimal_frame");
            Texture2D terminalFrameActiveImage = GFXAdapter.GetTexture("monitor_minimal_frame_active");

            terminalImageStyle = Create9SliceStyle(terminalImage);
            terminalFrameStyle = Create9SliceStyle(terminalFrameImage);
            terminalFrameActiveStyle = Create9SliceStyle(terminalFrameActiveImage);

            terminalLetterSkin = BuildPanelSkin();
            terminalLetterSkin.label.font = ConfigAdapter.Instance.ConsoleFont;
            terminalLetterSkin.label.fontSize = ConfigAdapter.Instance.ConsoleFontSize;

            LoggerAdapter.Instance.Debug($"Console font: {terminalLetterSkin.label.font}");

            CharacterInfo chInfo;
            terminalLetterSkin.label.font
                .RequestCharactersInTexture("X"); // Make sure the char in the font is lazy-loaded by Unity.
            terminalLetterSkin.label.font.GetCharacterInfo('X', out chInfo);
            fontCharWidth = chInfo.advance - 3;
            fontCharHeight = terminalLetterSkin.label.fontSize + 2;
            LoggerAdapter.Instance.Debug($"Font metrics: {fontCharWidth} x {fontCharHeight}");
        }

        public Result<object, Exception> Submit(string expression) {
            LoggerAdapter.Instance.Debug($"Submitted: {expression}");
            consoleBuffer?.PrintLine($"$> {expression}");
            try {
                var result = Utils.Expression.Run(expression);
                if (result != null) {
                    consoleBuffer?.PrintLine($"{result}");
                }
                return Result.Ok<object, Exception>(result);
            } catch (Exception e) {
                consoleBuffer?.PrintLine($"{e}");
                return Result.Err<object, Exception>(e);
            }
        }

        private void UpdateCommandHistory() {
            if (commandHistoryIndex >= 0 && commandHistoryIndex < commandHistory.Count) {
                replText = commandHistory[commandHistoryIndex];
            } else {
                replText = "";
            }
            LoggerAdapter.Instance.Info($"Command history: {string.Join(", ", commandHistory)} ({commandHistory.Count} command(s)); Command history index: {commandHistoryIndex}; REPL text: {replText}");
        }

        protected override void DrawWindow(int windowId) {
            GUI.color = Color;

            replText = GUI.TextField(new Rect(15, windowRect.height - 70, windowRect.width - 75, 25), replText);
            if (GUI.Button(new Rect(windowRect.width - 45, windowRect.height - 70, 30, 25), CommonStyles.Instance.startButtonTexture)) {
                if (!string.IsNullOrWhiteSpace(replText)) {
                    commandHistory.Add(replText);
                }
                Submit(replText);
                commandHistoryIndex = commandHistory.Count;
                UpdateCommandHistory();
            }

            GUI.enabled = commandHistoryIndex > 0;
            if (GUI.Button(new Rect(windowRect.width - 58, windowRect.height - 70, 12, 12), CommonStyles.Instance.upButtonTexture, tinyButtonStyle)) {
                commandHistoryIndex--;
                UpdateCommandHistory();
            }

            GUI.enabled = commandHistoryIndex < commandHistory.Count;
            if (GUI.Button(new Rect(windowRect.width - 58, windowRect.height - 57, 12, 12), CommonStyles.Instance.downButtonTexture, tinyButtonStyle)) {
                commandHistoryIndex++;
                UpdateCommandHistory();
            }

            GUI.enabled = true;

            if (GUI.Button(new Rect(windowRect.width - 75, windowRect.height - 30, 50, 25), "Close")) Close();
            if (GUI.Button(new Rect(15, windowRect.height - 30, 50, 25), "Clear")) {
                consoleBuffer?.Clear();
            }

            if (windowRect.width > 200 && consoleBuffer != null) {
                if (GUI.Button(new Rect(80, windowRect.height - 30, 50, 25), "Copy")) {
                    GUIUtility.systemCopyBuffer = consoleBuffer.ContentAsString();
                }
            }

            if (windowRect.width > 400) {
                string label = Game.Input.asset.enabled ? "Disable Game Input" : "Enable Game Input";
                if (GUI.Button(new Rect(145, windowRect.height - 30, 175, 25), label)) {
                    if (Game.Input.asset.enabled) {
                        Game.Input.Disable();
                    } else {
                        Game.Input.Enable();
                    }
                }
            }

            GUI.Label(new Rect(15, 28, windowRect.width - 30, windowRect.height - 105), "", terminalImageStyle);

            GUI.BeginGroup(new Rect(28, 48, (consoleBuffer?.VisibleCols ?? 1) * fontCharWidth + 2,
                (consoleBuffer?.VisibleRows ?? 1) * fontCharHeight +
                4)); // +4 so descenders and underscores visible on bottom row.

            List<ConsoleLine> visibleLines = consoleBuffer?.VisibleLines ?? new List<ConsoleLine>();

            terminalLetterSkin.label.normal.textColor = TextColor;

            for (int row = 0; row < visibleLines.Count; row++) {
                string lineString = visibleLines[row].ToString().Replace('\0', ' ').Substring(0, consoleBuffer?.VisibleCols ?? 0);

                GUI.Label(new Rect(0, (row * fontCharHeight), windowRect.width - 10, fontCharHeight), lineString,
                    terminalLetterSkin.label);
            }

            GUI.EndGroup();

            
            GUI.Label(new Rect(15, 28, windowRect.width - 30, windowRect.height - 105), "", terminalFrameStyle);

            GUI.Label(new Rect(windowRect.width / 2 - 40, windowRect.height - 16, 100, 10),
                (consoleBuffer?.VisibleCols ?? 1) + "x" + (consoleBuffer?.VisibleRows ?? 1));
        }

        protected override void OnResize(Rect newWindowRect) {
            consoleBuffer?.Resize((int)((newWindowRect.height - 140) / fontCharHeight),
                (int)((newWindowRect.width - 65) / fontCharWidth));
        }


        public static Color AdjustColor(Color baseColor, double brightness) {
            Color newColor = baseColor;
            newColor.a = Convert.ToSingle(brightness); // represent dimness by making it fade into the backround.
            return newColor;
        }

        /// <summary>
        /// Unity lacks gui styles for GUI.DrawTexture(), so to make it do
        /// 9-slice stretching, we have to draw the 9slice image as a GUI.Label.
        /// But GUI.Labels that render a Texture2D instead of text, won't stretch
        /// larger than the size of the image file no matter what you do (only smaller).
        /// So to make it stretch the image in a label, the image has to be implemented
        /// as part of the label's background defined in the GUIStyle instead of as a
        /// normal image element.  This sets up that style, which you can then render
        /// by making a GUILabel use this style and have dummy empty string content.
        /// </summary>
        /// <returns>The slice style.</returns>
        /// <param name="fromTexture">From texture.</param>
        private GUIStyle Create9SliceStyle(Texture2D fromTexture) {
            GUIStyle style = new GUIStyle();
            style.normal.background = fromTexture;
            style.border = new RectOffset(20, 20, 20, 20);
            return style;
        }

        private GUISkin BuildPanelSkin() {
            GUISkin theSkin = Instantiate(_spaceWarpUISkin); // Use Instantiate to make a copy of the Skin Object

            theSkin.label.fontSize = 10;
            theSkin.label.normal.textColor = Color.white;
            theSkin.label.padding = new RectOffset(0, 0, 0, 0);
            theSkin.label.margin = new RectOffset(1, 1, 1, 1);

            theSkin.button.fontSize = 10;
            theSkin.button.padding = new RectOffset(0, 0, 0, 0);
            theSkin.button.margin = new RectOffset(0, 0, 0, 0);

            return theSkin;
        }

        protected override void OnClose() {
            OnCloseClicked();
        }
    }
}
