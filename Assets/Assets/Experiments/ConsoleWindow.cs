using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Experiments {
    public class ConsoleWindow : UGUIResizableWindow {
        private RawImage replStartStopIcon;
        
        public void OnEnable() {
            Initialize("KontrolSystem: Console", new Rect(200, 500, 400, 500));

            var root = RootVerticalLayout();
            
            GameObject consoleBackground = new GameObject("ConsoleBackground", typeof(Image), typeof(TextMeshProUGUI));
            root.Add(consoleBackground, UGUILayout.Align.STRETCH, new Vector2(200, 200), 1);
            Image backgroundImage = consoleBackground.GetComponent<Image>();
            backgroundImage.sprite = UIFactory.Instance.consoleBackground;
            backgroundImage.type = Image.Type.Sliced;
            backgroundImage.color = Color.white;

            GameObject consoleFrame = new GameObject("ConsoleFrame", typeof(Image));
            UIFactory.Layout(consoleFrame, consoleBackground.transform, UIFactory.LAYOUT_STRECH,
                UIFactory.LAYOUT_STRECH, 0, 0, 0, 0);
            Image frameImage = consoleFrame.GetComponent<Image>();
            frameImage.sprite = UIFactory.Instance.consoleInactiveFrame;
            frameImage.type = Image.Type.Sliced;
            frameImage.color = Color.white;
            
            GameObject console = new GameObject("Console", typeof(TextMeshProUGUI));
            UIFactory.Layout(console, consoleBackground.transform, UIFactory.LAYOUT_STRECH, UIFactory.LAYOUT_STRECH, 
                10, -10, -20, -20);
            var textMesh = console.GetComponent<TextMeshProUGUI>();
            textMesh.font = UIFactory.Instance.consoleFont;
            textMesh.horizontalAlignment = HorizontalAlignmentOptions.Left;
            textMesh.verticalAlignment = VerticalAlignmentOptions.Top;
            textMesh.fontSize = 12;
            textMesh.color = new Color(0.5f, 1.0f, 0.5f, 1.0f);

            var replContainer = root.Add(UGUILayoutContainer.Horizontal(2));
            
            var commandInputField = UGUIInputField.Create("", 120);
            replContainer.Add(commandInputField, UGUILayout.Align.STRETCH, 1);

            var replHistoryContainer = replContainer.Add(UGUILayoutContainer.Vertical(0));

            GameObject replHistoryUp = UIFactory.Instance.CreateIconButton(UIFactory.Instance.upIcon);
            replHistoryContainer.Add(replHistoryUp, UGUILayout.Align.STRETCH, new Vector2(15, 15));

            GameObject replHistoryDown = UIFactory.Instance.CreateIconButton(UIFactory.Instance.downIcon);
            replHistoryContainer.Add(replHistoryDown, UGUILayout.Align.STRETCH, new Vector2(15, 15));

            GameObject replStartStop = UIFactory.Instance.CreateIconButton(false ? UIFactory.Instance.stopIcon : UIFactory.Instance.startIcon);
            replContainer.Add(replStartStop, UGUILayout.Align.STRETCH, new Vector2( 30, 30));
            replStartStopIcon = replStartStop.GetComponentInChildren<RawImage>();

            var buttonContainer = root.Add(UGUILayoutContainer.Horizontal(20));

            buttonContainer.Add(UGUIButton.Create("Clear", () => {}));
            buttonContainer.Add(UGUIButton.Create("Copy", () => { }));

            textMesh.SetText("Bla\nBlaBla\nBlaBla\nBlaBla\nBlaBla\nBlaBla\nBlaBla\nBlaBla\nBlaBla\nBlaBla\nBla");

            MinSize = root.MinSize;
            root.Layout();
        }
    }
}
