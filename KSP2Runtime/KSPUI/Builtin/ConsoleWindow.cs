using System;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.KSP.Runtime.Core;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KontrolSystem.KSP.Runtime.KSPUI.Builtin;

public class ConsoleWindow : UGUIResizableWindow {
    private float charHeight;
    private float charWidth;
    private KSPConsoleBuffer? consoleBuffer;
    private TextMeshProUGUI? consoleText;

    public void OnEnable() {
        Initialize("KontrolSystem: Console", new Rect(200, Screen.height - 200, 400, 500));

        var root = RootVerticalLayout();

        var consoleBackground = new GameObject("ConsoleBackground", typeof(Image));
        root.Add(consoleBackground, UGUILayout.Align.Stretch, new Vector2(200, 200), 1);
        var image = consoleBackground.GetComponent<Image>();
        image.sprite = UIFactory.Instance!.consoleBackground;
        image.type = Image.Type.Sliced;
        image.color = Color.white;

        var consoleFrame = new GameObject("ConsoleFrame", typeof(Image), typeof(RectMask2D));
        UIFactory.Layout(consoleFrame, consoleBackground.transform, UIFactory.LAYOUT_STRETCH,
            UIFactory.LAYOUT_STRETCH, 0, 0, 0, 0);
        var frameImage = consoleFrame.GetComponent<Image>();
        frameImage.sprite = UIFactory.Instance.consoleInactiveFrame;
        frameImage.type = Image.Type.Sliced;
        frameImage.color = Color.white;
        var frameMask = consoleFrame.GetComponent<RectMask2D>();
        frameMask.padding = new(8, 8, 8, 8);

        var console = new GameObject("Console", typeof(TextMeshProUGUI), typeof(ConsoleWindowInput));
        UIFactory.Layout(console, consoleFrame.transform, UIFactory.LAYOUT_STRETCH, UIFactory.LAYOUT_STRETCH,
            10, -10, -20, -20);
        consoleText = console.GetComponent<TextMeshProUGUI>();
        consoleText.font = UIFactory.Instance.consoleFont;
        consoleText.horizontalAlignment = HorizontalAlignmentOptions.Left;
        consoleText.verticalAlignment = VerticalAlignmentOptions.Top;
        consoleText.fontSize = UIFactory.Instance.consoleFontSize;
        consoleText.richText = true;
        consoleText.enableWordWrapping = false;
        consoleText.overflowMode = TextOverflowModes.Masking;
        consoleText.color = new Color(0.5f, 1.0f, 0.5f, 1.0f);

        var fontScale = consoleText.fontSize / consoleText.font.faceInfo.pointSize;
        charHeight = consoleText.font.faceInfo.lineHeight * fontScale;
        charWidth = consoleText.font.glyphTable[0].metrics.horizontalAdvance * fontScale;

        var buttonContainer = root.Add(UGUILayoutContainer.Horizontal(20)).Item1;

        buttonContainer.Add(UGUIButton.Create("Clear", () => consoleBuffer!.Clear()));
        buttonContainer.Add(UGUIButton.Create("Copy",
            () => { GUIUtility.systemCopyBuffer = consoleBuffer!.ContentAsString(); }));

        MinSize = root.Layout();

        consoleBuffer = Mainframe.Instance!.ConsoleBuffer;
        console.GetComponent<ConsoleWindowInput>().Init(frameImage, consoleBuffer);
        consoleBuffer.changed.AddListener(OnConsoleBufferChanged);

        OnConsoleBufferChanged();
    }

    public override void OnDisable() {
        base.OnDisable();

        consoleBuffer?.changed.RemoveListener(OnConsoleBufferChanged);
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
