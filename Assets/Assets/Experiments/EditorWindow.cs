using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Experiments {
    public class EditorWindow : UGUIResizableWindow {
        private GameObject deleteConfirm;
        
        public void OnEnable() {
            Initialize("KontrolSystem: Editor", new Rect(700, 500, 600, 500));
            
            var root = RootVerticalLayout();
            
            var editPanel = new GameObject("Editor", typeof(Image), typeof(TMP_InputField));
            editPanel.SetActive(false);
            root.Add(editPanel, UGUILayout.Align.STRETCH, new Vector2(200, 200),1);
            var textArea = new GameObject("TextArea", typeof(RectMask2D));
            UIFactory.Layout(textArea, editPanel.transform, UIFactory.LAYOUT_STRECH, UIFactory.LAYOUT_STRECH, 5, -5, -30, -10);
            
            var childText = new GameObject("Text", typeof(TextMeshProUGUI));
            UIFactory.Layout(childText, textArea.transform, UIFactory.LAYOUT_STRECH, UIFactory.LAYOUT_STRECH, 0, 0, 0, 0);
            
            var vSlider = UIFactory.Instance.CreateVScrollbar();
            UIFactory.Layout(vSlider, editPanel.transform, UIFactory.LAYOUT_END, UIFactory.LAYOUT_STRECH,
                -5, -5, 20, -10);
            
            Image image = editPanel.GetComponent<Image>();
            image.sprite = UIFactory.Instance.frameBackground;
            image.type = Image.Type.Tiled;
            image.color = Color.white;

            TextMeshProUGUI text = childText.GetComponent<TextMeshProUGUI>();
            text.font = UIFactory.Instance.consoleFont;
            text.color = new Color(0.8382f, 0.8784f, 1);
            text.fontSize = 16;

            TMP_InputField inputField = editPanel.GetComponent<TMP_InputField>();
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
            inputField.fontAsset = UIFactory.Instance.consoleFont;
            inputField.verticalScrollbar = vSlider.GetComponent<Scrollbar>();
            inputField.lineType = TMP_InputField.LineType.MultiLineNewline;
            inputField.onFocusSelectAll = false;
            inputField.lineLimit = 0;
            inputField.pointSize = 16;

            editPanel.SetActive(true);
            
            var fileNameInput = UGUIInputField.Create("", 120);
            root.Add(fileNameInput);

            var buttonContainer = root.Add(UGUILayoutContainer.Horizontal(10, new Padding(0, 0, 20, 20)));
            
            var delete = UIFactory.Instance.CreateButton("Delete");
            buttonContainer.Add(delete, UGUILayout.Align.STRETCH, new Vector2(120, 30));
            var deleteButton = delete.GetComponent<Button>();
            var deleteButtonColors = deleteButton.colors;
            deleteButtonColors.normalColor = new Color(0.7195f, 0.2508f, 0);
            deleteButtonColors.highlightedColor = new Color(0.6195f, 0.1508f, 0);
            deleteButtonColors.pressedColor = new Color(0.8f, 0.2508f, 0);
            deleteButton.colors = deleteButtonColors;
            deleteButton.onClick.AddListener(() => {
                if (deleteConfirm != null) {
                    Destroy(deleteConfirm);
                    deleteConfirm = null;
                } else {
                    deleteConfirm = UIFactory.Instance.CreatePanel();
                    UIFactory.Layout(deleteConfirm, windowTransform, UIFactory.LAYOUT_START, UIFactory.LAYOUT_END,
                        30, -40, 180, 50);

                    var confirmDelete = UIFactory.Instance.CreateButton("Confirm delete");
                    UIFactory.Layout(confirmDelete, deleteConfirm.transform, UIFactory.LAYOUT_STRECH, UIFactory.LAYOUT_STRECH, 10, -10, -20, -20);
                    confirmDelete.GetComponent<Button>().colors = deleteButtonColors;
                    confirmDelete.GetComponent<Button>().onClick.AddListener(() => {
                        
                    });
                }
            });

            buttonContainer.AddSpace(0, 1);
            
            var saveAndReboot = UGUIButton.Create("Save and Reboot", () => { });
            buttonContainer.Add(saveAndReboot);

            MinSize = root.MinSize;
            root.Layout();
        }
    }
}
