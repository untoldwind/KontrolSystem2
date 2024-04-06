using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Experiments {
    public class ConsoleWindow : UGUIResizableWindow {
        private RawImage replStartStopIcon;
        private KSPConsoleBuffer consoleBuffer = new KSPConsoleBuffer(25, 40);
        private TextMeshProUGUI consoleText;
        private float charHeight;
        private float charWidth;

        public void OnEnable() {
            consoleBuffer.PrintLine("Test Line 1 <color=red>bla</color>    dfökdfölksdf ölsdkf ölskdf ösdlkfölsdfk ölsdfk ösdlfksdöl fkösldf");
            consoleBuffer.PrintLine("Test <mark=green>\u2588</mark> line 2 lkjsdflkjsdflk jsdflk jsdflkjs dflkjsdf lksdjflksdjf lksdjfm");
            for (int i = 1; i < 20; i++) {
                consoleBuffer.PrintLine($"Line {i}");
            }
            
            Initialize("KontrolSystem: Console", new Rect(200, 500, 400, 500));

            var root = RootVerticalLayout();
            
            GameObject consoleBackground = new GameObject("ConsoleBackground", typeof(Image), typeof(TextMeshProUGUI));
            root.Add(consoleBackground, UGUILayout.Align.STRETCH, new Vector2(200, 200), 1);
            Image backgroundImage = consoleBackground.GetComponent<Image>();
            backgroundImage.sprite = UIFactory.Instance.consoleBackground;
            backgroundImage.type = Image.Type.Sliced;
            backgroundImage.color = Color.white;

            GameObject consoleFrame = new GameObject("ConsoleFrame", typeof(Image), typeof(RectMask2D));
            UIFactory.Layout(consoleFrame, consoleBackground.transform, UIFactory.LAYOUT_STRECH,
                UIFactory.LAYOUT_STRECH, 0, 0, 0, 0);
            Image frameImage = consoleFrame.GetComponent<Image>();
            frameImage.sprite = UIFactory.Instance.consoleInactiveFrame;
            frameImage.type = Image.Type.Sliced;
            frameImage.color = Color.white;
            RectMask2D mask = consoleFrame.GetComponent<RectMask2D>();
            mask.padding = new(10, 10, 10, 10);
            
            GameObject console = new GameObject("Console", typeof(TextMeshProUGUI), typeof(ConsoleWindowInput));
            UIFactory.Layout(console, consoleFrame.transform, UIFactory.LAYOUT_STRECH, UIFactory.LAYOUT_STRECH, 
                10, -10, -20, -20);
            consoleText = console.GetComponent<TextMeshProUGUI>();
            consoleText.font = UIFactory.Instance.consoleFont;
            consoleText.horizontalAlignment = HorizontalAlignmentOptions.Left;
            consoleText.verticalAlignment = VerticalAlignmentOptions.Top;
            consoleText.fontSize = 12;
            consoleText.richText = true;
            consoleText.enableWordWrapping = false;
            consoleText.overflowMode = TextOverflowModes.Masking;
            consoleText.color = new Color(0.5f, 1.0f, 0.5f, 1.0f);
            
            var fontScale = consoleText.fontSize / consoleText.font.faceInfo.pointSize;
            charHeight = consoleText.font.faceInfo.lineHeight * fontScale;
            charWidth = consoleText.font.glyphTable[0].metrics.horizontalAdvance * fontScale;

            console.GetComponent<ConsoleWindowInput>().Init(frameImage, consoleBuffer);
            
            var buttonContainer = root.Add(UGUILayoutContainer.Horizontal(20));

            buttonContainer.Add(UGUIButton.Create("Clear", () => {}));
            buttonContainer.Add(UGUIButton.Create("Copy", () => { }));
            
            MinSize = root.MinSize;
            root.Layout();

            consoleBuffer.changed.AddListener(OnConsoleBufferChanged);

            OnConsoleBufferChanged();
        }
        
        
        protected override void OnResize(Vector2 delta) {
            base.OnResize(delta);

            OnConsoleBufferChanged();
        }

        private void OnConsoleBufferChanged() {
            var textRect = consoleText!.GetComponent<RectTransform>().rect;

            consoleBuffer!.Resize((int)(textRect.height / charHeight), (int)(textRect.width / charWidth));

            consoleText.SetText(consoleBuffer.DisplayText());
        }
    }
}
