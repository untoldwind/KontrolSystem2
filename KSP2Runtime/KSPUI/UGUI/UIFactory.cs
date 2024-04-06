using System;
using TMPro;
using UniLinq;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

namespace KontrolSystem.KSP.Runtime.KSPUI.UGUI;

public interface UIAssetsProvider {
    Texture2D WindowsBackground { get; }

    Texture2D WindowCloseButton { get; }

    Texture2D ButtonBackground { get; }

    Texture2D SelectButtonBackground { get; }

    Texture2D PanelBackground { get; }

    Texture2D VScrollBackground { get; }

    Texture2D VScrollHandle { get; }

    Texture2D FrameBackground { get; }

    Texture2D StateInactive { get; }

    Texture2D StateActive { get; }

    Texture2D StateError { get; }

    Texture2D StartIcon { get; }

    Texture2D StopIcon { get; }

    Texture2D ToggleOn { get; }

    Texture2D ToggleOff { get; }

    Texture2D SliderBackground { get; }

    Texture2D SliderFill { get; }

    Texture2D SliderHandle { get; }

    Texture2D ConsoleBackground { get; }

    Texture2D ConsoleInactiveFrame { get; }

    Texture2D ConsoleActiveFrame { get; }

    Texture2D UpIcon { get; }

    Texture2D DownIcon { get; }

    Font GraphFont { get; }

    Font UIFont { get; }

    float UIFontSize { get; }

    Font ConsoleFont { get; }

    float ConsoleFontSize { get; }

    void OnChange(Action action);
}

public class UIFactory {
    internal static readonly int UI_LAYER = 5;
    internal static LayoutAlign LAYOUT_START = new(0, 0, 0);
    internal static LayoutAlign LAYOUT_CENTER = new(0.5f, 0.5f, 0.5f);
    internal static LayoutAlign LAYOUT_END = new(1, 1, 1);
    internal static LayoutAlign LAYOUT_STRETCH = new(0, 1, 0);
    internal readonly Sprite buttonBackground;
    internal readonly Sprite consoleBackground;
    internal readonly TMP_FontAsset consoleFont;
    internal readonly float consoleFontSize;
    internal readonly Sprite consoleInactiveFrame;
    internal readonly Sprite consoleActiveFrame;
    internal readonly Texture2D downIcon;
    internal readonly Sprite downIconSprite;
    internal readonly Sprite frameBackground;
    internal readonly TMP_FontAsset graphFont;
    internal readonly Sprite panelBackground;
    internal readonly Sprite selectButtonBackground;
    internal readonly Sprite sliderBackground;
    internal readonly Sprite sliderFill;
    internal readonly Sprite sliderHandle;
    internal readonly Texture2D startIcon;
    internal readonly Texture2D stateActive;
    internal readonly Texture2D stateError;
    internal readonly Texture2D stateInactive;
    internal readonly Texture2D stopIcon;
    internal readonly Sprite toggleOff;
    internal readonly Sprite toggleOn;
    internal readonly TMP_FontAsset uiFont;
    internal readonly float uiFontSize;
    internal readonly Texture2D upIcon;
    internal readonly Sprite vScrollBackground;
    internal readonly Sprite vScrollHandle;
    internal readonly Sprite windowBackground;
    internal readonly Sprite windowCloseButton;

    internal UIFactory(UIAssetsProvider uiAssetsProvider) {
        graphFont = TMP_FontAsset.CreateFontAsset(uiAssetsProvider.GraphFont, 90, 9, GlyphRenderMode.SDFAA_HINTED, 1024,
            1024);
        UpdateShader(graphFont, true);

        uiFont = TMP_FontAsset.CreateFontAsset(uiAssetsProvider.UIFont, 90, 9, GlyphRenderMode.SDFAA_HINTED, 1024,
            1024);
        UpdateShader(uiFont, false);
        uiFontSize = uiAssetsProvider.UIFontSize;

        consoleFont = TMP_FontAsset.CreateFontAsset(uiAssetsProvider.ConsoleFont, 90, 9, GlyphRenderMode.SDFAA_HINTED,
            1024, 1024);
        UpdateShader(consoleFont, false);
        consoleFontSize = uiAssetsProvider.ConsoleFontSize;

        windowBackground = Make9TileSprite(uiAssetsProvider.WindowsBackground, new Vector4(30, 30, 30, 30));
        windowCloseButton = Make9TileSprite(uiAssetsProvider.WindowCloseButton, new Vector4(4, 4, 4, 4));
        buttonBackground = Make9TileSprite(uiAssetsProvider.ButtonBackground, new Vector4(6, 6, 6, 6));
        selectButtonBackground = Make9TileSprite(uiAssetsProvider.SelectButtonBackground, new Vector4(6, 6, 6, 6));
        panelBackground = Make9TileSprite(uiAssetsProvider.PanelBackground, new Vector4(6, 6, 6, 6));
        vScrollBackground = Make9TileSprite(uiAssetsProvider.VScrollBackground, new Vector4(0, 6, 0, 6));
        vScrollHandle = Make9TileSprite(uiAssetsProvider.VScrollHandle, new Vector4(6, 11, 6, 11));
        frameBackground = Make9TileSprite(uiAssetsProvider.FrameBackground, new Vector4(5, 5, 5, 5));
        stateInactive = uiAssetsProvider.StateInactive;
        stateActive = uiAssetsProvider.StateActive;
        stateError = uiAssetsProvider.StateError;
        startIcon = uiAssetsProvider.StartIcon;
        stopIcon = uiAssetsProvider.StopIcon;
        upIcon = uiAssetsProvider.UpIcon;
        downIcon = uiAssetsProvider.DownIcon;
        toggleOn = Make9TileSprite(uiAssetsProvider.ToggleOn, new Vector4(9, 9, 9, 9));
        toggleOff = Make9TileSprite(uiAssetsProvider.ToggleOff, new Vector4(9, 9, 9, 9));
        sliderBackground = Make9TileSprite(uiAssetsProvider.SliderBackground, new Vector4(13, 8, 11, 8));
        sliderFill = Make9TileSprite(uiAssetsProvider.SliderFill, new Vector4(5, 5, 5, 5));
        sliderHandle = Make9TileSprite(uiAssetsProvider.SliderHandle, new Vector4(3, 5, 3, 5));
        consoleBackground = Make9TileSprite(uiAssetsProvider.ConsoleBackground, new Vector4(20, 20, 20, 20));
        consoleInactiveFrame = Make9TileSprite(uiAssetsProvider.ConsoleInactiveFrame, new Vector4(20, 20, 20, 20));
        consoleActiveFrame = Make9TileSprite(uiAssetsProvider.ConsoleActiveFrame, new Vector4(20, 20, 20, 20));
        downIconSprite = Make9TileSprite(uiAssetsProvider.DownIcon, new Vector4(1, 1, 1, 1));

        GLUIDrawer.Initialize(graphFont);
    }

    public static UIFactory? Instance { get; private set; }

    public static void Init(UIAssetsProvider uiAssetsProvider) {
        Instance = new UIFactory(uiAssetsProvider);
        uiAssetsProvider.OnChange(() => { Instance = new UIFactory(uiAssetsProvider); });
    }

    internal void UpdateShaderRaster(TMP_FontAsset fontAsset) {
        var tmp_material = new Material(Shader.Find("TextMeshPro/Bitmap"));

        tmp_material.SetTexture(ShaderUtilities.ID_MainTex, fontAsset.atlasTexture);
        tmp_material.SetFloat(ShaderUtilities.ID_TextureWidth, fontAsset.atlasWidth);
        tmp_material.SetFloat(ShaderUtilities.ID_TextureHeight, fontAsset.atlasHeight);

        fontAsset.material = tmp_material;
    }

    internal void UpdateShader(TMP_FontAsset fontAsset, bool overlay) {
        var tmp_material =
            new Material(Shader.Find(overlay ? "TextMeshPro/Distance Field Overlay" : "TextMeshPro/Distance Field"));

        tmp_material.SetTexture(ShaderUtilities.ID_MainTex, fontAsset.atlasTexture);
        tmp_material.SetFloat(ShaderUtilities.ID_TextureWidth, fontAsset.atlasWidth);
        tmp_material.SetFloat(ShaderUtilities.ID_TextureHeight, fontAsset.atlasHeight);
        tmp_material.SetFloat(ShaderUtilities.ID_GradientScale, fontAsset.atlasPadding + 1);
        tmp_material.SetFloat(ShaderUtilities.ID_WeightNormal, fontAsset.normalStyle);
        tmp_material.SetFloat(ShaderUtilities.ID_WeightBold, fontAsset.boldStyle);

        fontAsset.material = tmp_material;
    }

    internal Sprite Make9TileSprite(Texture2D texture, Vector4 border) {
        return Sprite.Create(texture,
            new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100, 0,
            SpriteMeshType.FullRect, border);
    }

    internal GameObject CreateButton(string label) {
        var buttonRoot = CreateUIObject("Button", typeof(Image), typeof(Button));
        var labelText = CreateUIObject("ButtonLabel", typeof(TextMeshProUGUI));
        var labelTextTransform = labelText.GetComponent<RectTransform>();
        labelTextTransform.SetParent(buttonRoot.transform);
        labelTextTransform.anchorMin = Vector2.zero;
        labelTextTransform.anchorMax = Vector2.one;
        labelTextTransform.sizeDelta = Vector2.zero;
        labelTextTransform.anchoredPosition = new Vector2(0, 1);

        var image = buttonRoot.GetComponent<Image>();
        image.sprite = buttonBackground;
        image.type = Image.Type.Tiled;
        image.color = Color.white;

        var bt = buttonRoot.GetComponent<Button>();
        var colors = bt.colors;
        colors.normalColor = new Color(0.4784f, 0.5216f, 0.6f);
        colors.highlightedColor = new Color(0.7059f, 0.7686f, 0.8824f);
        colors.pressedColor = new Color(0.8059f, 0.8686f, 0.9824f);
        colors.selectedColor = new Color(0.7059f, 0.7686f, 0.8824f);
        colors.disabledColor = new Color(0.2784f, 0.3216f, 0.4f);
        bt.colors = colors;

        var text = labelTextTransform.GetComponent<TextMeshProUGUI>();
        text.text = label;
        text.font = uiFont;
        text.horizontalAlignment = HorizontalAlignmentOptions.Center;
        text.verticalAlignment = VerticalAlignmentOptions.Middle;
        text.fontSize = uiFontSize;
        text.color = Color.black;
        text.enableWordWrapping = false;

        return buttonRoot;
    }

    internal GameObject CreateDeleteButton() {
        var root = CreateUIObject("CloseButton", typeof(Image), typeof(Button));
        var buttonImage = root.GetComponent<Image>();
        buttonImage.sprite = Instance!.windowCloseButton;
        buttonImage.type = Image.Type.Sliced;
        buttonImage.color = Color.white;
        var button = root.GetComponent<Button>();
        var buttonColors = button.colors;
        buttonColors.normalColor = new Color(0.5f, 0.5234f, 0.5976f);
        buttonColors.highlightedColor = new Color(0.5195f, 0.0508f, 0);
        buttonColors.pressedColor = new Color(0.7f, 0.0508f, 0);
        button.colors = buttonColors;

        return root;
    }

    internal GameObject CreateSelectButton(string label) {
        var buttonRoot = CreateUIObject("SelectButton", typeof(Image), typeof(Toggle));
        var checkmark = CreateUIObject("SelectCheckmark", typeof(Image));
        var checkmarkTransform = checkmark.GetComponent<RectTransform>();
        checkmarkTransform.SetParent(buttonRoot.transform);
        checkmarkTransform.anchorMin = Vector2.zero;
        checkmarkTransform.anchorMax = Vector2.one;
        checkmarkTransform.sizeDelta = Vector2.zero;

        var labelText = CreateUIObject("ButtonLabel", typeof(TextMeshProUGUI));
        var labelTextTransform = labelText.GetComponent<RectTransform>();
        labelTextTransform.SetParent(checkmarkTransform);
        labelTextTransform.anchorMin = Vector2.zero;
        labelTextTransform.anchorMax = Vector2.one;
        labelTextTransform.sizeDelta = Vector2.zero;
        labelTextTransform.anchoredPosition = new Vector2(0, 1);

        var image = buttonRoot.GetComponent<Image>();
        image.sprite = selectButtonBackground;
        image.type = Image.Type.Tiled;
        image.color = Color.white;

        var checkmarkImage = checkmark.GetComponent<Image>();
        checkmarkImage.sprite = selectButtonBackground;
        checkmarkImage.type = Image.Type.Tiled;
        checkmarkImage.color = new Color(0.2941f, 0.3137f, 0.6902f);

        var toggle = buttonRoot.GetComponent<Toggle>();
        toggle.isOn = false;

        toggle.graphic = checkmarkImage;
        toggle.targetGraphic = image;
        var btColors = toggle.colors;
        btColors.normalColor = new Color(0.1569f, 0.1804f, 0.2157f);
        btColors.highlightedColor = new Color(0.1804f, 0.2078f, 0.251f);
        btColors.pressedColor = new Color(0.1804f, 0.2078f, 0.251f);
        btColors.selectedColor = new Color(0.1804f, 0.2078f, 0.251f);
        toggle.colors = btColors;

        var text = labelTextTransform.GetComponent<TextMeshProUGUI>();
        text.text = label;
        text.font = uiFont;
        text.horizontalAlignment = HorizontalAlignmentOptions.Center;
        text.verticalAlignment = VerticalAlignmentOptions.Middle;
        text.fontSize = uiFontSize;
        text.color = new Color(0.7961f, 0.8706f, 1f);
        text.enableWordWrapping = false;

        return buttonRoot;
    }

    internal GameObject CreateIconButton(Texture2D icon) {
        var buttonRoot = CreateUIObject("IconToggle", typeof(Image), typeof(Button));
        var iconImage = CreateUIObject("Icon", typeof(RawImage));
        var iconTransform = iconImage.GetComponent<RectTransform>();
        iconTransform.SetParent(buttonRoot.transform);
        iconTransform.anchorMin = Vector2.one * 0.5f;
        iconTransform.anchorMax = Vector2.one * 0.5f;
        iconTransform.sizeDelta = new Vector2(icon.width, icon.height);

        var image = buttonRoot.GetComponent<Image>();
        image.sprite = selectButtonBackground;
        image.type = Image.Type.Tiled;
        image.color = Color.white;

        var rawImage = iconImage.GetComponent<RawImage>();
        rawImage.texture = icon;
        rawImage.color = Color.white;

        var bt = buttonRoot.GetComponent<Button>();
        var btColors = bt.colors;
        btColors.normalColor = new Color(0, 0, 0, 0);
        btColors.highlightedColor = new Color(0.2941f, 0.3137f, 0.6902f);
        btColors.pressedColor = new Color(0.2941f, 0.3137f, 0.6902f);
        btColors.selectedColor = new Color(0.1804f, 0.2078f, 0.251f);
        bt.colors = btColors;

        return buttonRoot;
    }

    public GameObject CreateToggle(string label) {
        var toggleRoot = CreateUIObject("Toggle", typeof(Toggle));
        var background = CreateUIObject("Background", typeof(Image));
        var checkmark = CreateUIObject("Checkmark", typeof(Image));
        var childLabel = CreateUIObject("Label", typeof(TextMeshProUGUI));

        var toggle = toggleRoot.GetComponent<Toggle>();
        toggle.isOn = false;

        var bgImage = background.GetComponent<Image>();
        bgImage.sprite = toggleOff;
        bgImage.type = Image.Type.Tiled;
        bgImage.color = Color.white;

        var checkmarkImage = checkmark.GetComponent<Image>();
        checkmarkImage.sprite = toggleOn;
        checkmarkImage.type = Image.Type.Tiled;
        checkmarkImage.color = Color.white;

        var labelText = childLabel.GetComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.font = uiFont;
        labelText.horizontalAlignment = HorizontalAlignmentOptions.Left;
        labelText.verticalAlignment = VerticalAlignmentOptions.Middle;
        labelText.fontSize = uiFontSize;
        labelText.color = new Color(0.7961f, 0.8706f, 1f);
        labelText.enableWordWrapping = false;

        toggle.graphic = checkmarkImage;
        toggle.targetGraphic = bgImage;
        var colors = toggle.colors;
        colors.normalColor = new Color(0.4784f, 0.5216f, 0.6f);
        colors.highlightedColor = new Color(0.7059f, 0.7686f, 0.8824f);
        colors.pressedColor = new Color(0.8059f, 0.8686f, 0.9824f);
        colors.selectedColor = new Color(0.7059f, 0.7686f, 0.8824f);
        toggle.colors = colors;

        var bgRect = background.GetComponent<RectTransform>();
        bgRect.SetParent(toggleRoot.transform);
        bgRect.anchorMin = new Vector2(0f, 0.5f);
        bgRect.anchorMax = new Vector2(0f, 0.5f);
        bgRect.pivot = new Vector2(0, 0.5f);
        bgRect.anchoredPosition = Vector2.zero;
        bgRect.sizeDelta = new Vector2(40f, 20f);

        var checkmarkRect = checkmark.GetComponent<RectTransform>();
        checkmarkRect.SetParent(background.transform);
        checkmarkRect.anchorMin = new Vector2(0f, 0.5f);
        checkmarkRect.anchorMax = new Vector2(0f, 0.5f);
        checkmarkRect.pivot = new Vector2(0, 0.5f);
        checkmarkRect.anchoredPosition = Vector2.zero;
        checkmarkRect.sizeDelta = new Vector2(40f, 20f);

        var labelRect = childLabel.GetComponent<RectTransform>();
        labelRect.SetParent(toggleRoot.transform);
        labelRect.anchorMin = new Vector2(0f, 0f);
        labelRect.anchorMax = new Vector2(1f, 1f);
        labelRect.pivot = new Vector2(0, 0.5f);
        labelRect.anchoredPosition = new Vector2(43f, 2f);
        labelRect.sizeDelta = new Vector2(-43f, 0f);

        return toggleRoot;
    }

    internal GameObject CreateVScrollbar() {
        var scrollbarRoot = CreateUIObject("VScrollbar", typeof(Image), typeof(Scrollbar));
        var sliderArea = CreateUIObject("Sliding Area", typeof(RectTransform));
        var handle = CreateUIObject("Handle", typeof(Image));

        var bgImage = scrollbarRoot.GetComponent<Image>();
        bgImage.sprite = vScrollBackground;
        bgImage.type = Image.Type.Tiled;
        bgImage.color = Color.white;

        var handleImage = handle.GetComponent<Image>();
        handleImage.sprite = vScrollHandle;
        handleImage.type = Image.Type.Tiled;
        handleImage.color = Color.white;

        var sliderAreaRect = sliderArea.GetComponent<RectTransform>();
        sliderAreaRect.SetParent(scrollbarRoot.transform);
        sliderAreaRect.sizeDelta = new Vector2(-20, -20);
        sliderAreaRect.anchorMin = Vector2.zero;
        sliderAreaRect.anchorMax = Vector2.one;

        var handleRect = handle.GetComponent<RectTransform>();
        handleRect.SetParent(sliderAreaRect);
        handleRect.sizeDelta = new Vector2(20, 20);

        var scrollbar = scrollbarRoot.GetComponent<Scrollbar>();
        scrollbar.handleRect = handleRect;
        scrollbar.targetGraphic = handleImage;
        scrollbar.direction = Scrollbar.Direction.TopToBottom;

        return scrollbarRoot;
    }

    internal GameObject CreateScrollView(GameObject content) {
        var root = CreateUIObject("Scroll View", typeof(Image));
        var scrollRoot = CreateUIObject("Scroll Rect", typeof(ScrollRect));
        var viewport = CreateUIObject("Viewport", typeof(Image), typeof(Mask));

        var scrollRootRT = scrollRoot.GetComponent<RectTransform>();
        scrollRootRT.SetParent(root.transform);
        scrollRootRT.anchorMin = Vector2.zero;
        scrollRootRT.anchorMax = Vector2.one;
        scrollRootRT.localPosition = Vector3.zero;
        scrollRootRT.sizeDelta = new Vector2(-10, -10);

        var vScrollbar = CreateVScrollbar();
        vScrollbar.GetComponent<Scrollbar>().SetDirection(Scrollbar.Direction.BottomToTop, true);
        var vScrollbarRT = vScrollbar.GetComponent<RectTransform>();
        vScrollbarRT.SetParent(scrollRoot.transform);
        vScrollbarRT.anchorMin = Vector2.right;
        vScrollbarRT.anchorMax = Vector2.one;
        vScrollbarRT.pivot = Vector2.one;
        vScrollbarRT.sizeDelta = new Vector2(20, 0);

        var viewportRT = viewport.GetComponent<RectTransform>();
        viewportRT.SetParent(scrollRoot.transform);
        viewportRT.anchorMin = Vector2.zero;
        viewportRT.anchorMax = Vector2.one;
        viewportRT.sizeDelta = new Vector2(-20, 0);
        viewportRT.pivot = Vector2.up;

        var contentRT = content.GetComponent<RectTransform>();
        contentRT.SetParent(viewportRT);
        contentRT.anchorMin = Vector2.up;
        contentRT.anchorMax = Vector2.one;
        contentRT.sizeDelta = new Vector2(0, 0);
        contentRT.pivot = Vector2.up;

        var scrollRect = scrollRoot.GetComponent<ScrollRect>();
        scrollRect.content = contentRT;
        scrollRect.viewport = viewportRT;
        scrollRect.verticalScrollbar = vScrollbar.GetComponent<Scrollbar>();
        scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
        scrollRect.verticalScrollbarSpacing = 3;

        var rootImage = root.GetComponent<Image>();
        rootImage.sprite = panelBackground;
        rootImage.type = Image.Type.Tiled;
        rootImage.color = Color.white;

        var viewportMask = viewport.GetComponent<Mask>();
        viewportMask.showMaskGraphic = false;

        var viewportImage = viewport.GetComponent<Image>();
        viewportImage.type = Image.Type.Tiled;

        return root;
    }

    internal GameObject CreatePanel() {
        var panel = CreateUIObject("Panel", typeof(Image));

        var image = panel.GetComponent<Image>();
        image.sprite = panelBackground;
        image.type = Image.Type.Tiled;
        image.color = Color.white;

        return panel;
    }

    internal GameObject CreateText(string text, float size = 20,
        HorizontalAlignmentOptions hAlign = HorizontalAlignmentOptions.Left,
        VerticalAlignmentOptions vAlign = VerticalAlignmentOptions.Middle) {
        var root = CreateUIObject("Text", typeof(TextMeshProUGUI));
        var textMesh = root.GetComponent<TextMeshProUGUI>();
        textMesh.SetText(text);
        textMesh.font = uiFont;
        textMesh.horizontalAlignment = hAlign;
        textMesh.verticalAlignment = vAlign;
        textMesh.fontSize = size;
        textMesh.color = new Color(0.8382f, 0.8784f, 1);
        textMesh.enableWordWrapping = false;

        return root;
    }

    internal GameObject CreateInputField() {
        var root = CreateUIObject("InputField", typeof(Image), typeof(InputFieldExtended));
        root.SetActive(false);
        var textArea = CreateUIObject("TextArea", typeof(RectMask2D));
        Layout(textArea, root.transform, LAYOUT_STRETCH, LAYOUT_STRETCH, 3, -6, -6, 0);

        var childText = CreateUIObject("Text", typeof(TextMeshProUGUI));
        Layout(childText, textArea.transform, LAYOUT_STRETCH, LAYOUT_STRETCH, 0, 0, 0, 0);

        var image = root.GetComponent<Image>();
        image.sprite = frameBackground;
        image.type = Image.Type.Tiled;
        image.color = Color.white;

        var text = childText.GetComponent<TextMeshProUGUI>();
        text.font = uiFont;
        text.color = new Color(0.8382f, 0.8784f, 1);
        text.fontSize = uiFontSize - 2;

        TMP_InputField inputField = root.GetComponent<InputFieldExtended>();
        var inputColors = inputField.colors;
        inputColors.normalColor = new Color(0.4784f, 0.5216f, 0.6f);
        inputColors.highlightedColor = new Color(0.7059f, 0.7686f, 0.8824f);
        inputColors.pressedColor = new Color(0.2941f, 0.3137f, 0.6902f);
        inputColors.selectedColor = new Color(0.2941f, 0.3137f, 0.6902f);
        inputField.colors = inputColors;
        inputField.textComponent = text;
        inputField.textViewport = textArea.GetComponent<RectTransform>();
        inputField.caretColor = new Color(0.8382f, 0.8784f, 1);
        inputField.selectionColor = new Color(0, 0.4353f, 1);
        inputField.customCaretColor = true;
        inputField.fontAsset = uiFont;
        inputField.pointSize = uiFontSize - 2;

        root.SetActive(true);
        var inputActions = root.AddComponent<InputFieldFocusLockInputActions>();
        inputActions.DisableAllInputActionMaps = true;

        return root;
    }

    internal GameObject CreateHSlider() {
        var root = CreateUIObject("Slider", typeof(Slider));
        var background = CreateUIObject("Background", typeof(Image));
        Layout(background, root.transform, LAYOUT_STRETCH, LAYOUT_STRETCH, 0, 0, 0, 0);
        var fillArea = CreateUIObject("Fill Area", typeof(RectTransform));
        Layout(fillArea, root.transform, LAYOUT_STRETCH, LAYOUT_STRETCH, 5, 0, -20, 0);
        var fill = CreateUIObject("Fill", typeof(Image));
        Layout(fill, fillArea.transform, LAYOUT_STRETCH, LAYOUT_STRETCH, -5, 0, 10, 0);
        var handleArea = CreateUIObject("Handle Slide Area", typeof(RectTransform));
        Layout(handleArea, root.transform, LAYOUT_STRETCH, LAYOUT_STRETCH, 10, -3, -20, -6);
        var handle = CreateUIObject("Handle", typeof(Image));
        Layout(handle, handleArea.transform, LAYOUT_START, LAYOUT_STRETCH, -6, 0, 12, 0);

        var backgroundImage = background.GetComponent<Image>();
        backgroundImage.sprite = sliderBackground;
        backgroundImage.type = Image.Type.Tiled;
        backgroundImage.color = Color.white;

        var fillImage = fill.GetComponent<Image>();
        fillImage.sprite = sliderFill;
        fillImage.type = Image.Type.Tiled;
        fillImage.color = Color.white;

        var handleImage = handle.GetComponent<Image>();
        handleImage.sprite = sliderHandle;
        handleImage.color = Color.white;

        var slider = root.GetComponent<Slider>();
        slider.fillRect = fill.GetComponent<RectTransform>();
        slider.handleRect = handle.GetComponent<RectTransform>();
        slider.targetGraphic = handleImage;
        slider.direction = Slider.Direction.LeftToRight;

        return root;
    }

    internal GameObject CreateDropdown(string[] options, float size = 20) {
        var dropdownRoot = CreateUIObject("Dropdown", typeof(Image), typeof(TMP_Dropdown));
        var label = CreateUIObject("Label", typeof(TextMeshProUGUI));
        var arrow = CreateUIObject("arrow", typeof(Image));
        var template = CreateUIObject("Template", typeof(Image), typeof(ScrollRect));
        var viewport = CreateUIObject("Viewport", typeof(Image), typeof(Mask));
        var content = CreateUIObject("Content", typeof(RectTransform));
        var item = CreateUIObject("Item", typeof(Toggle));
        var itemBackground = CreateUIObject("Item Background", typeof(Image));
        var itemLabel = CreateUIObject("Item Label", typeof(TextMeshProUGUI));
        var scrollbar = CreateVScrollbar();

        scrollbar.GetComponent<Scrollbar>().SetDirection(Scrollbar.Direction.BottomToTop, true);

        template.SetActive(false);

        var dropdown = dropdownRoot.GetComponent<TMP_Dropdown>();

        RectTransform dropdownRootRT = dropdownRoot.GetComponent<RectTransform>();

        var backgroundImage = dropdownRoot.GetComponent<Image>();
        backgroundImage.sprite = frameBackground;
        backgroundImage.type = Image.Type.Tiled;
        backgroundImage.color = Color.white;

        var textMesh = label.GetComponent<TextMeshProUGUI>();
        textMesh.SetText("text");
        textMesh.font = uiFont;
        textMesh.horizontalAlignment = HorizontalAlignmentOptions.Left;
        textMesh.verticalAlignment = VerticalAlignmentOptions.Middle;
        textMesh.fontSize = size;
        textMesh.color = new Color(0.8382f, 0.8784f, 1);
        textMesh.enableWordWrapping = false;
        dropdown.captionText = textMesh;

        var labelRT = label.GetComponent<RectTransform>();
        labelRT.SetParent(dropdownRootRT);
        labelRT.anchorMin = Vector2.zero;
        labelRT.anchorMax = Vector2.one;
        labelRT.pivot = new Vector2(0, 0);
        labelRT.sizeDelta = new Vector2(25, 0);
        labelRT.anchoredPosition = new Vector3(7, 2, 0);

        var arrowImage = arrow.GetComponent<Image>();
        arrowImage.sprite = downIconSprite;
        arrowImage.type = Image.Type.Tiled;
        arrowImage.color = Color.white;

        var arrowRT = arrow.GetComponent<RectTransform>();
        arrowRT.SetParent(dropdownRootRT);
        arrowRT.anchorMin = new Vector2(1, 0.5f);
        arrowRT.anchorMax = new Vector2(1, 0.5f);
        arrowRT.pivot = new Vector2(1, 0);
        arrowRT.sizeDelta = new Vector2(16, 16);
        arrowRT.anchoredPosition = new Vector3(-4, -8, 0);

        var templateRT = template.GetComponent<RectTransform>();
        templateRT.SetParent(dropdownRootRT);
        templateRT.anchorMin = new Vector2(0, 0);
        templateRT.anchorMax = new Vector2(1, 0);
        templateRT.pivot = new Vector2(0.5f, 1);
        templateRT.anchoredPosition = new Vector2(0, 2);
        templateRT.sizeDelta = new Vector2(0, 150);
        dropdown.template = templateRT;

        var templateBackground = template.GetComponent<Image>();
        templateBackground.sprite = panelBackground;
        templateBackground.type = Image.Type.Tiled;
        templateBackground.color = Color.white;

        var viewportRT = viewport.GetComponent<RectTransform>();
        viewportRT.SetParent(templateRT);
        viewportRT.anchorMin = new Vector2(0, 0);
        viewportRT.anchorMax = new Vector2(1, 1);
        viewportRT.sizeDelta = new Vector2(-10, 0);
        viewportRT.pivot = new Vector2(0, 1);
        viewportRT.anchoredPosition = Vector3.zero;

        Mask viewportMask = viewport.GetComponent<Mask>();
        viewportMask.showMaskGraphic = false;

        Image viewportImage = viewport.GetComponent<Image>();
        viewportImage.type = Image.Type.Tiled;

        var contentRT = content.GetComponent<RectTransform>();
        contentRT.SetParent(viewportRT);
        contentRT.anchorMin = new Vector2(0f, 1);
        contentRT.anchorMax = new Vector2(1f, 1);
        contentRT.pivot = new Vector2(0.5f, 1);
        contentRT.anchoredPosition = new Vector2(0, 0);
        contentRT.sizeDelta = new Vector2(-10, 28);

        var itemRT = item.GetComponent<RectTransform>();
        itemRT.SetParent(contentRT);
        itemRT.anchorMin = new Vector2(0, 0.5f);
        itemRT.anchorMax = new Vector2(1, 0.5f);
        itemRT.sizeDelta = new Vector2(0, uiFontSize + 10);
        itemRT.anchoredPosition = Vector3.zero;

        var itemToggle = item.GetComponent<Toggle>();
        var btColors = itemToggle.colors;
        btColors.normalColor = new Color(0.1569f, 0.1804f, 0.2157f);
        btColors.highlightedColor = new Color(0.1804f, 0.2078f, 0.251f);
        btColors.pressedColor = new Color(0.1804f, 0.2078f, 0.251f);
        btColors.selectedColor = new Color(0.1804f, 0.2078f, 0.251f);
        itemToggle.colors = btColors;

        var itemBackgroundRT = itemBackground.GetComponent<RectTransform>();
        itemBackgroundRT.SetParent(itemRT);
        itemBackgroundRT.anchorMin = Vector2.zero;
        itemBackgroundRT.anchorMax = Vector2.one;
        itemBackgroundRT.sizeDelta = Vector2.zero;
        itemBackgroundRT.anchoredPosition = Vector3.zero;

        var itemBackgroundBackground = itemBackground.GetComponent<Image>();
        itemBackgroundBackground.color = Color.white;

        var itemLabelRT = itemLabel.GetComponent<RectTransform>();
        itemLabelRT.SetParent(itemRT);
        itemLabelRT.anchorMin = Vector2.zero;
        itemLabelRT.anchorMax = Vector2.one;
        itemLabelRT.offsetMin = new Vector2(20, 1);
        itemLabelRT.offsetMax = new Vector2(-10, 1);

        var itemLabelText = itemLabel.GetComponent<TextMeshProUGUI>();
        itemLabelText.font = uiFont;
        itemLabelText.horizontalAlignment = HorizontalAlignmentOptions.Left;
        itemLabelText.verticalAlignment = VerticalAlignmentOptions.Middle;
        itemLabelText.fontSize = size;
        itemLabelText.color = new Color(0.8382f, 0.8784f, 1);
        itemLabelText.enableWordWrapping = false;

        dropdown.itemText = itemLabelText;

        item.GetComponent<Toggle>().targetGraphic = itemBackgroundBackground;

        RectTransform vScrollbarRT = scrollbar.GetComponent<RectTransform>();
        vScrollbarRT.SetParent(templateRT);
        vScrollbarRT.anchorMin = Vector2.right;
        vScrollbarRT.anchorMax = Vector2.one;
        vScrollbarRT.pivot = Vector2.one;
        vScrollbarRT.sizeDelta = new Vector2(20, 0);
        vScrollbarRT.anchoredPosition = Vector3.zero;

        ScrollRect templateScrollRect = template.GetComponent<ScrollRect>();
        templateScrollRect.content = content.GetComponent<RectTransform>();
        templateScrollRect.viewport = viewport.GetComponent<RectTransform>();
        templateScrollRect.horizontal = false;
        templateScrollRect.verticalScrollbar = scrollbar.GetComponent<Scrollbar>();
        templateScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
        templateScrollRect.verticalScrollbarSpacing = -3;

        dropdown.options = options.Select(option => new TMP_Dropdown.OptionData(option)).ToList();
        dropdown.RefreshShownValue();

        return dropdownRoot;
    }

    internal static RectTransform Layout(GameObject gameObject, Transform parent, LayoutAlign horizontal,
        LayoutAlign vertical,
        float x, float y, float width, float height) {
        var transform = gameObject.GetComponent<RectTransform>();
        transform.SetParent(parent);
        transform.anchorMin = new Vector2(horizontal.min, 1 - vertical.max);
        transform.anchorMax = new Vector2(horizontal.max, 1 - vertical.min);
        transform.pivot = new Vector2(horizontal.pivot, 1 - vertical.pivot);
        transform.localPosition = Vector3.zero;
        transform.anchoredPosition = new Vector3(x, y);
        transform.sizeDelta = new Vector2(width, height);

        return transform;
    }

    internal struct LayoutAlign(float min, float max, float pivot) {
        internal float min = min;
        internal float max = max;
        internal float pivot = pivot;
    }

    private static GameObject CreateUIObject(string name, params Type[] components) => new(name, components) {
        layer = UI_LAYER
    };
}
