using TMPro;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

namespace Experiments {
    public interface UIAssetsProvider {
        Texture2D WindowsBackground { get; }
        
        Texture2D CloseButton { get; }
        
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
        
        Texture2D UpIcon { get; }
        
        Texture2D DownIcon { get; }
        
        Font GraphFont { get; }

        Font UIFont { get; }

        Font ConsoleFont { get; }
    }
    
    public class UIFactory {
        internal readonly Sprite windowBackground;
        internal readonly Sprite windowCloseButton;
        internal readonly Sprite buttonBackground;
        internal readonly Sprite selectButtonBackground;
        internal readonly Sprite panelBackground;
        internal readonly Sprite vScrollBackground;
        internal readonly Sprite vScrollHandle;
        internal readonly Sprite frameBackground;
        internal readonly TMP_FontAsset graphFont;
        internal readonly TMP_FontAsset uiFont;
        internal readonly TMP_FontAsset consoleFont;
        internal readonly Texture2D stateInactive;
        internal readonly Texture2D stateActive;
        internal readonly Texture2D stateError;
        internal readonly Texture2D startIcon;
        internal readonly Texture2D stopIcon;
        internal readonly Texture2D upIcon;
        internal readonly Texture2D downIcon;
        internal readonly Sprite toggleOn;
        internal readonly Sprite toggleOff;
        internal readonly Sprite sliderBackground;
        internal readonly Sprite sliderFill;
        internal readonly Sprite sliderHandle;
        internal readonly Sprite consoleBackground;
        internal readonly Sprite consoleInactiveFrame;

        public static UIFactory Instance { get; private set; }
        
        public static void Init(UIAssetsProvider uiAssetsProvider) {
            Instance = new UIFactory(uiAssetsProvider);
        }
        
        internal UIFactory(UIAssetsProvider uiAssetsProvider) {
            graphFont = TMP_FontAsset.CreateFontAsset(uiAssetsProvider.GraphFont, 90, 9, GlyphRenderMode.SDFAA_HINTED, 1024, 1024, AtlasPopulationMode.Dynamic);
            UpdateShader(graphFont, true);

            uiFont = TMP_FontAsset.CreateFontAsset(uiAssetsProvider.UIFont, 90, 9, GlyphRenderMode.SDFAA_HINTED, 1024, 1024, AtlasPopulationMode.Dynamic);
            UpdateShader(uiFont, false);

            consoleFont = TMP_FontAsset.CreateFontAsset(uiAssetsProvider.ConsoleFont, 90, 9, GlyphRenderMode.SDFAA_HINTED, 1024, 1024, AtlasPopulationMode.Dynamic);
            UpdateShader(consoleFont, false);

            windowBackground = Make9TileSprite(uiAssetsProvider.WindowsBackground, new Vector4(30, 30, 30, 30));
            windowCloseButton = Make9TileSprite(uiAssetsProvider.CloseButton, new Vector4(4, 4, 4, 4));
            buttonBackground = Make9TileSprite(uiAssetsProvider.ButtonBackground, new Vector4(6, 6, 6, 6));
            selectButtonBackground = Make9TileSprite(uiAssetsProvider.SelectButtonBackground, new Vector4(6, 6, 6, 6));
            panelBackground = Make9TileSprite(uiAssetsProvider.PanelBackground, new Vector4(6, 6, 6, 6));
            vScrollBackground = Make9TileSprite(uiAssetsProvider.VScrollBackground, new Vector4(0, 6, 0, 6));
            vScrollHandle = Make9TileSprite(uiAssetsProvider.VScrollHandle, new Vector4(6, 11, 6, 11));
            frameBackground = Make9TileSprite(uiAssetsProvider.FrameBackground, new Vector4(4, 4, 4, 4));
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

            GLUIDrawer.Initialize(graphFont);
        }

        internal void  UpdateShaderRaster(TMP_FontAsset fontAsset) {
            Material tmp_material = new Material( Shader.Find("TextMeshPro/Bitmap"));
            
            tmp_material.SetTexture(ShaderUtilities.ID_MainTex, fontAsset.atlasTexture);
            tmp_material.SetFloat(ShaderUtilities.ID_TextureWidth, fontAsset.atlasWidth);
            tmp_material.SetFloat(ShaderUtilities.ID_TextureHeight, fontAsset.atlasHeight);

            fontAsset.material = tmp_material;
        }

        internal void  UpdateShader(TMP_FontAsset fontAsset, bool overlay) {
            Material tmp_material = new Material( Shader.Find(overlay ? "TextMeshPro/Distance Field Overlay" : "TextMeshPro/Distance Field"));
            
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
            GameObject buttonRoot = new GameObject("Button", typeof(Image), typeof(Button));
            GameObject labelText = new GameObject("ButtonLabel", typeof(TextMeshProUGUI));
            RectTransform labelTextTransform = labelText.GetComponent<RectTransform>();
            labelTextTransform.SetParent(buttonRoot.transform);
            labelTextTransform.anchorMin = Vector2.zero;
            labelTextTransform.anchorMax = Vector2.one;
            labelTextTransform.sizeDelta = Vector2.zero;
            labelTextTransform.anchoredPosition = new Vector2(0, 1);
            
            Image image = buttonRoot.GetComponent<Image>();
            image.sprite = buttonBackground;
            image.type = Image.Type.Tiled;
            image.color = Color.white;

            Button bt = buttonRoot.GetComponent<Button>();
            ColorBlock colors = bt.colors;
            colors.normalColor = new Color(0.4784f, 0.5216f, 0.6f);
            colors.highlightedColor = new Color(0.7059f, 0.7686f, 0.8824f);
            colors.pressedColor = new Color(0.8059f, 0.8686f, 0.9824f);
            colors.selectedColor = new Color(0.7059f, 0.7686f, 0.8824f);
            colors.disabledColor = new Color(0.2784f, 0.3216f, 0.4f);
            bt.colors = colors;

            TextMeshProUGUI text = labelTextTransform.GetComponent<TextMeshProUGUI>();
            text.text = label;
            text.font = uiFont;
            text.horizontalAlignment = HorizontalAlignmentOptions.Center;
            text.verticalAlignment = VerticalAlignmentOptions.Middle;
            text.fontSize = 20;
            text.color = Color.black;
            
            return buttonRoot;            
        }

        internal GameObject CreateDeleteButton() {
            var root = new GameObject("CloseButton", typeof(Image), typeof(Button));
            var buttonImage = root.GetComponent<Image>();
            buttonImage.sprite = UIFactory.Instance.windowCloseButton;
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
            GameObject buttonRoot = new GameObject("SelectButton", typeof(Image), typeof(Toggle));
            GameObject checkmark = new GameObject("SelectCheckmark", typeof(Image));
            RectTransform checkmarkTransform = checkmark.GetComponent<RectTransform>();
            checkmarkTransform.SetParent(buttonRoot.transform);
            checkmarkTransform.anchorMin = Vector2.zero;
            checkmarkTransform.anchorMax = Vector2.one;
            checkmarkTransform.sizeDelta = Vector2.zero;
            
            GameObject labelText = new GameObject("ButtonLabel", typeof(TextMeshProUGUI));
            RectTransform labelTextTransform = labelText.GetComponent<RectTransform>();
            labelTextTransform.SetParent(checkmarkTransform);
            labelTextTransform.anchorMin = Vector2.zero;
            labelTextTransform.anchorMax = Vector2.one;
            labelTextTransform.sizeDelta = Vector2.zero;
            labelTextTransform.anchoredPosition = new Vector2(0, 1);

            Image image = buttonRoot.GetComponent<Image>();
            image.sprite = selectButtonBackground;
            image.type = Image.Type.Tiled;
            image.color = Color.white;

            Image checkmarkImage = checkmark.GetComponent<Image>();
            checkmarkImage.sprite = selectButtonBackground;
            checkmarkImage.type = Image.Type.Tiled;
            checkmarkImage.color = new Color(0.2941f, 0.3137f, 0.6902f);
            
            Toggle toggle = buttonRoot.GetComponent<Toggle>();
            toggle.isOn = false;
            
            toggle.graphic = checkmarkImage;
            toggle.targetGraphic = image;
            var btColors = toggle.colors;
            btColors.normalColor = new Color(0.1569f, 0.1804f, 0.2157f);
            btColors.highlightedColor = new Color(0.1804f, 0.2078f, 0.251f);
            btColors.pressedColor = new Color(0.1804f, 0.2078f, 0.251f);
            btColors.selectedColor = new Color(0.1804f, 0.2078f, 0.251f);
            toggle.colors = btColors;
            
            TextMeshProUGUI text = labelTextTransform.GetComponent<TextMeshProUGUI>();
            text.text = label;
            text.font = uiFont;
            text.horizontalAlignment = HorizontalAlignmentOptions.Center;
            text.verticalAlignment = VerticalAlignmentOptions.Middle;
            text.fontSize = 20;
            text.color = new Color(0.7961f, 0.8706f, 1f);
            
            return buttonRoot;          
        }

        internal GameObject CreateIconButton(Texture2D icon) {
            GameObject buttonRoot = new GameObject("IconToggle", typeof(Image), typeof(Button));
            GameObject iconImage = new GameObject("Icon", typeof(RawImage));
            RectTransform iconTransform = iconImage.GetComponent<RectTransform>();
            iconTransform.SetParent(buttonRoot.transform);
            iconTransform.anchorMin = Vector2.one * 0.5f;
            iconTransform.anchorMax = Vector2.one * 0.5f;
            iconTransform.sizeDelta = new Vector2(icon.width, icon.height);

            Image image = buttonRoot.GetComponent<Image>();
            image.sprite = selectButtonBackground;
            image.type = Image.Type.Tiled;
            image.color = Color.white;

            RawImage rawImage = iconImage.GetComponent<RawImage>();
            rawImage.texture = icon;
            rawImage.color = Color.white;
            
            Button bt = buttonRoot.GetComponent<Button>();
            ColorBlock btColors = bt.colors;
            btColors.normalColor = new Color(0,0,0, 0);
            btColors.highlightedColor = new Color(0.2941f, 0.3137f, 0.6902f);
            btColors.pressedColor = new Color(0.2941f, 0.3137f, 0.6902f);
            btColors.selectedColor = new Color(0.1804f, 0.2078f, 0.251f);
            bt.colors = btColors;

            return buttonRoot;
        }

        public GameObject CreateToggle(string label) {
            GameObject toggleRoot = new GameObject("Toggle", typeof(Toggle));

            GameObject background = new GameObject("Background", typeof(Image));
            GameObject checkmark = new GameObject("Checkmark", typeof(Image));
            GameObject childLabel = new GameObject("Label", typeof(TextMeshProUGUI));
            
            Toggle toggle = toggleRoot.GetComponent<Toggle>();
            toggle.isOn = false;

            Image bgImage = background.GetComponent<Image>();
            bgImage.sprite = toggleOff;
            bgImage.type = Image.Type.Tiled;
            bgImage.color = Color.white;

            Image checkmarkImage = checkmark.GetComponent<Image>();
            checkmarkImage.sprite = toggleOn;
            checkmarkImage.type = Image.Type.Tiled;
            checkmarkImage.color = Color.white;

            TextMeshProUGUI labelText = childLabel.GetComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.font = uiFont;
            labelText.horizontalAlignment = HorizontalAlignmentOptions.Left;
            labelText.verticalAlignment = VerticalAlignmentOptions.Middle;
            labelText.fontSize = 20;
            labelText.color = new Color(0.7961f, 0.8706f, 1f);

            toggle.graphic = checkmarkImage;
            toggle.targetGraphic = bgImage;
            ColorBlock colors = toggle.colors;
            colors.normalColor = new Color(0.4784f, 0.5216f, 0.6f);
            colors.highlightedColor = new Color(0.7059f, 0.7686f, 0.8824f);
            colors.pressedColor = new Color(0.8059f, 0.8686f, 0.9824f);
            colors.selectedColor = new Color(0.7059f, 0.7686f, 0.8824f);
            toggle.colors = colors;

            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.SetParent(toggleRoot.transform);
            bgRect.anchorMin        = new Vector2(0f, 0.5f);
            bgRect.anchorMax        = new Vector2(0f, 0.5f);
            bgRect.pivot = new Vector2(0, 0.5f);
            bgRect.anchoredPosition = Vector2.zero;
            bgRect.sizeDelta        = new Vector2(40f, 20f);

            RectTransform checkmarkRect = checkmark.GetComponent<RectTransform>();
            checkmarkRect.SetParent(background.transform);
            checkmarkRect.anchorMin = new Vector2(0f, 0.5f);
            checkmarkRect.anchorMax = new Vector2(0f, 0.5f);
            checkmarkRect.pivot = new Vector2(0, 0.5f);
            checkmarkRect.anchoredPosition = Vector2.zero;
            checkmarkRect.sizeDelta = new Vector2(40f, 20f);

            RectTransform labelRect = childLabel.GetComponent<RectTransform>();
            labelRect.SetParent(toggleRoot.transform);
            labelRect.anchorMin     = new Vector2(0f, 0f);
            labelRect.anchorMax     = new Vector2(1f, 1f);
            labelRect.pivot = new Vector2(0, 0.5f);
            labelRect.anchoredPosition     = new Vector2(43f, 2f);
            labelRect.sizeDelta     = new Vector2(-43f, 0f);
            
            return toggleRoot;
        }
        
        internal GameObject CreateVScrollbar() {
            GameObject scrollbarRoot = new GameObject("VScrollbar", typeof(Image), typeof(Scrollbar));
            GameObject sliderArea = new GameObject("Sliding Area", typeof(RectTransform));
            GameObject handle = new GameObject("Handle", typeof(Image));
            
            Image bgImage = scrollbarRoot.GetComponent<Image>();
            bgImage.sprite = vScrollBackground;
            bgImage.type = Image.Type.Tiled;
            bgImage.color = Color.white;

            Image handleImage = handle.GetComponent<Image>();
            handleImage.sprite = vScrollHandle;
            handleImage.type = Image.Type.Tiled;
            handleImage.color = Color.white;

            RectTransform sliderAreaRect = sliderArea.GetComponent<RectTransform>();
            sliderAreaRect.SetParent(scrollbarRoot.transform);
            sliderAreaRect.sizeDelta = new Vector2(-20, -20);
            sliderAreaRect.anchorMin = Vector2.zero;
            sliderAreaRect.anchorMax = Vector2.one;

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.SetParent(sliderAreaRect);
            handleRect.sizeDelta = new Vector2(20, 20);

            Scrollbar scrollbar = scrollbarRoot.GetComponent<Scrollbar>();
            scrollbar.handleRect = handleRect;
            scrollbar.targetGraphic = handleImage;
            scrollbar.direction = Scrollbar.Direction.TopToBottom;
            
            return scrollbarRoot;
        }
        
        internal GameObject CreateScrollView(GameObject content) {
            GameObject root = new GameObject("Scroll View", typeof(Image));
            GameObject scrollRoot = new GameObject("Scroll Rect", typeof(ScrollRect));
            GameObject viewport = new GameObject("Viewport", typeof(Image), typeof(Mask));

            RectTransform scrollRootRT = scrollRoot.GetComponent<RectTransform>();
            scrollRootRT.SetParent(root.transform);
            scrollRootRT.anchorMin = Vector2.zero;
            scrollRootRT.anchorMax = Vector2.one;
            scrollRootRT.localPosition = Vector3.zero;
            scrollRootRT.sizeDelta = new Vector2(-10, -10);

            GameObject vScrollbar = CreateVScrollbar();
            vScrollbar.GetComponent<Scrollbar>().SetDirection(Scrollbar.Direction.BottomToTop, true);
            RectTransform vScrollbarRT = vScrollbar.GetComponent<RectTransform>();
            vScrollbarRT.SetParent(scrollRoot.transform);
            vScrollbarRT.anchorMin = Vector2.right;
            vScrollbarRT.anchorMax = Vector2.one;
            vScrollbarRT.pivot = Vector2.one;
            vScrollbarRT.sizeDelta = new Vector2(20, 0);

            RectTransform viewportRT = viewport.GetComponent<RectTransform>();
            viewportRT.SetParent(scrollRoot.transform);
            viewportRT.anchorMin = Vector2.zero;
            viewportRT.anchorMax = Vector2.one;
            viewportRT.sizeDelta = new Vector2(-20, 0);
            viewportRT.pivot = Vector2.up;

            RectTransform contentRT = content.GetComponent<RectTransform>();
            contentRT.SetParent(viewportRT);
            contentRT.anchorMin = Vector2.up;
            contentRT.anchorMax = Vector2.one;
            contentRT.sizeDelta = new Vector2(0, 0);
            contentRT.pivot = Vector2.up;

            ScrollRect scrollRect = scrollRoot.GetComponent<ScrollRect>();
            scrollRect.content = contentRT;
            scrollRect.viewport = viewportRT;
            scrollRect.verticalScrollbar = vScrollbar.GetComponent<Scrollbar>();
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
            scrollRect.verticalScrollbarSpacing = 3;

            Image rootImage = root.GetComponent<Image>();
            rootImage.sprite = panelBackground;
            rootImage.type = Image.Type.Tiled;
            rootImage.color = Color.white;

            Mask viewportMask = viewport.GetComponent<Mask>();
            viewportMask.showMaskGraphic = false;

            Image viewportImage = viewport.GetComponent<Image>();
            viewportImage.type = Image.Type.Tiled;

            return root;
        }        
        internal GameObject CreatePanel() {
            GameObject panel = new GameObject("Panel", typeof(Image));

            Image image = panel.GetComponent<Image>();
            image.sprite = panelBackground;
            image.type = Image.Type.Tiled;
            image.color = Color.white;

            return panel;
        }

        internal GameObject CreateText(string text, float size = 20, HorizontalAlignmentOptions hAlign = HorizontalAlignmentOptions.Left, VerticalAlignmentOptions vAlign = VerticalAlignmentOptions.Middle) {
            var root = new GameObject("Text", typeof(TextMeshProUGUI));
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
            var root = new GameObject("InputField", typeof(Image), typeof(TMP_InputField));
            root.SetActive(false);
            var textArea = new GameObject("TextArea", typeof(RectMask2D));
            Layout(textArea, root.transform, LAYOUT_STRECH, LAYOUT_STRECH, 3, -3, -6, 0);
            
            var childText = new GameObject("Text", typeof(TextMeshProUGUI));
            Layout(childText, textArea.transform, LAYOUT_STRECH, LAYOUT_STRECH, 0, 0, 0, 0);
            
            Image image = root.GetComponent<Image>();
            image.sprite = frameBackground;
            image.type = Image.Type.Tiled;
            image.color = Color.white;

            TextMeshProUGUI text = childText.GetComponent<TextMeshProUGUI>();
            text.font = uiFont;
            text.color = new Color(0.8382f, 0.8784f, 1);
            text.fontSize = 20;

            TMP_InputField inputField = root.GetComponent<TMP_InputField>();
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
            inputField.text = "bla";
            inputField.fontAsset = uiFont;
            inputField.pointSize = 20;

            root.SetActive(true);
            
            return root;
        }

        internal GameObject CreateHSlider() {
            var root = new GameObject("Slider", typeof(Slider));

            var background = new GameObject("Background", typeof(Image));
            Layout(background, root.transform, LAYOUT_STRECH, LAYOUT_STRECH, 0, 0, 0, 0);
            var fillArea = new GameObject("Fill Area",  typeof(RectTransform));
            Layout(fillArea, root.transform, LAYOUT_STRECH, LAYOUT_STRECH, 5, 0, -20, 0);
            var fill = new GameObject("Fill",  typeof(Image));
            Layout(fill, fillArea.transform, LAYOUT_STRECH, LAYOUT_STRECH, -5, 0, 10, 0);
            var handleArea = new GameObject("Handle Slide Area",  typeof(RectTransform));
            Layout(handleArea, root.transform, LAYOUT_STRECH, LAYOUT_STRECH, 10, -3, -20, -6);
            var handle = new GameObject("Handle", typeof(Image));
            Layout(handle, handleArea.transform, LAYOUT_START, LAYOUT_STRECH, -6, 0, 12, 0);

            Image backgroundImage = background.GetComponent<Image>();
            backgroundImage.sprite = sliderBackground;
            backgroundImage.type = Image.Type.Tiled;
            backgroundImage.color = Color.white;
            
            Image fillImage = fill.GetComponent<Image>();
            fillImage.sprite = sliderFill;
            fillImage.type = Image.Type.Tiled;
            fillImage.color = Color.white;
            
            Image handleImage = handle.GetComponent<Image>();
            handleImage.sprite = sliderHandle;
            handleImage.color = Color.white;
            
            Slider slider = root.GetComponent<Slider>();
            slider.fillRect = fill.GetComponent<RectTransform>();
            slider.handleRect = handle.GetComponent<RectTransform>();
            slider.targetGraphic = handleImage;
            slider.direction = Slider.Direction.LeftToRight;

            return root;
        }
        
        internal static RectTransform Layout(GameObject gameObject, Transform parent, LayoutAlign horizontal, LayoutAlign vertical,
            float x, float y, float width, float height) {
            RectTransform transform = gameObject.GetComponent<RectTransform>();
            transform.SetParent(parent);
            transform.anchorMin = new Vector2(horizontal.min, 1 - vertical.max);
            transform.anchorMax = new Vector2(horizontal.max, 1 - vertical.min);
            transform.pivot = new Vector2(horizontal.pivot, 1 - vertical.pivot);
            transform.localPosition = Vector3.zero;
            transform.anchoredPosition = new Vector3(x, y);
            transform.sizeDelta = new Vector2(width, height);

            return transform;
        }
        
        internal struct LayoutAlign {
            internal float min;
            internal float max;
            internal float pivot;

            public LayoutAlign(float min, float max, float pivot) {
                this.min = min;
                this.max = max;
                this.pivot = pivot;
            }
        }

        internal static LayoutAlign LAYOUT_START = new LayoutAlign(0, 0, 0);
        internal static LayoutAlign LAYOUT_CENTER = new LayoutAlign(0.5f, 0.5f, 0.5f);
        internal static LayoutAlign LAYOUT_END = new LayoutAlign(1, 1, 1);
        internal static LayoutAlign LAYOUT_STRECH = new LayoutAlign(0, 1, 0);
    }
}
