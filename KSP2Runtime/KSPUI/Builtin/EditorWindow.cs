using System.IO;
using KontrolSystem.KSP.Runtime.Core;
using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using LibNoise;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Math = System.Math;

namespace KontrolSystem.KSP.Runtime.KSPUI.Builtin {
    public class EditorWindow : UGUIResizableWindow {
        private UGUIInputField fileNameInputField;
        private InputFieldExtended editorInputField;
        private UGUIButton sourceAndRebootButton;
        private GameObject deleteConfirm;

        public void OnEnable() {
            Initialize("KontrolSystem: Editor", new Rect(Screen.width - 700, Screen.height - 200, 600, 500));

            var root = RootVerticalLayout();

            var editPanel = new GameObject("Editor", typeof(Image), typeof(InputFieldExtended));
            editPanel.SetActive(false);
            root.Add(editPanel, UGUILayout.Align.Stretch, new Vector2(200, 200), 1);
            var textArea = new GameObject("TextArea", typeof(RectMask2D));
            UIFactory.Layout(textArea, editPanel.transform, UIFactory.LAYOUT_STRETCH, UIFactory.LAYOUT_STRETCH, 5, -5,
                -30, -10);

            var childText = new GameObject("Text", typeof(TextMeshProUGUI));
            UIFactory.Layout(childText, textArea.transform, UIFactory.LAYOUT_STRETCH, UIFactory.LAYOUT_STRETCH, 0, 0, 0,
                0);

            var vSlider = UIFactory.Instance.CreateVScrollbar();
            UIFactory.Layout(vSlider, editPanel.transform, UIFactory.LAYOUT_END, UIFactory.LAYOUT_STRETCH,
                -5, -5, 20, -10);

            Image image = editPanel.GetComponent<Image>();
            image.sprite = UIFactory.Instance.frameBackground;
            image.type = Image.Type.Tiled;
            image.color = Color.white;

            TextMeshProUGUI text = childText.GetComponent<TextMeshProUGUI>();
            text.font = UIFactory.Instance.consoleFont;
            text.color = new Color(0.8382f, 0.8784f, 1);
            text.fontSize = Math.Max(10, UIFactory.Instance.uiFontSize - 6);

            editorInputField = editPanel.GetComponent<InputFieldExtended>();
            var inputColors = editorInputField.colors;
            inputColors.normalColor = new Color(0.4784f, 0.5216f, 0.6f);
            inputColors.highlightedColor = new Color(0.7059f, 0.7686f, 0.8824f);
            inputColors.pressedColor = new Color(0.2941f, 0.3137f, 0.6902f);
            inputColors.selectedColor = new Color(0.2941f, 0.3137f, 0.6902f);
            editorInputField.colors = inputColors;
            editorInputField.textComponent = text;
            editorInputField.textViewport = textArea.GetComponent<RectTransform>();
            editorInputField.caretColor = new Color(0.8382f, 0.8784f, 1);
            editorInputField.selectionColor = new Color(0, 0.4353f, 1);
            editorInputField.customCaretColor = true;
            editorInputField.text = "";
            editorInputField.fontAsset = UIFactory.Instance.consoleFont;
            editorInputField.verticalScrollbar = vSlider.GetComponent<Scrollbar>();
            editorInputField.lineType = TMP_InputField.LineType.MultiLineNewline;
            editorInputField.onFocusSelectAll = false;
            editorInputField.lineLimit = 0;
            editorInputField.pointSize = Math.Max(10, UIFactory.Instance.uiFontSize - 6);
            editorInputField.onValueChanged.AddListener(_ => {
                sourceAndRebootButton.Interactable = true;
            });

            editPanel.SetActive(true);
            var inputActions = editPanel.AddComponent<InputFieldFocusLockInputActions>();
            inputActions.DisableAllInputActionMaps = true;

            fileNameInputField = UGUIInputField.Create("", 120);
            root.Add(fileNameInputField);

            var buttonContainer = root.Add(UGUILayoutContainer.Horizontal(10, new UGUILayout.Padding(0, 0, 20, 20)));

            var delete = UIFactory.Instance.CreateButton("Delete");
            buttonContainer.Add(delete, UGUILayout.Align.Stretch, new Vector2(120, 30));
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
                    UIFactory.Layout(confirmDelete, deleteConfirm.transform, UIFactory.LAYOUT_STRETCH, UIFactory.LAYOUT_STRETCH, 10, -10, -20, -20);
                    confirmDelete.GetComponent<Button>().colors = deleteButtonColors;
                    confirmDelete.GetComponent<Button>().onClick.AddListener(OnDelete);
                }
            });

            buttonContainer.AddSpace(0, 1);

            sourceAndRebootButton = UGUIButton.Create("Save and Reboot", OnSave);
            buttonContainer.Add(sourceAndRebootButton);

            MinSize = root.Layout();
        }

        public void NewFile() {
            var filepath = Path.Combine(Mainframe.Instance.LocalLibPath, "unnamed.to2");

            int i = 2;
            while (File.Exists(filepath)) {
                filepath = Path.Combine(Mainframe.Instance.LocalLibPath, $"unnamed ({i++}).to2");
            }
            editorInputField.text = "use { Vessel } from ksp::vessel\r\nuse { CONSOLE } from ksp::console\r\n\r\npub fn main_flight(vessel: Vessel) -> Result<Unit, string> = {\r\n    CONSOLE.clear()\r\n    CONSOLE.print_line(\"Hello world\")\r\n}";
            fileNameInputField.Value = filepath;
            sourceAndRebootButton.Interactable = true;
        }

        public void Load(string sourceFile) {
            fileNameInputField.Value = sourceFile;
            editorInputField.text = File.ReadAllText(sourceFile);
            sourceAndRebootButton.Interactable = false;
        }

        public void OnSave() {
            var filepath = fileNameInputField.Value;
            var source = editorInputField.text;

            File.WriteAllText(filepath, source);

            Mainframe.Instance.Reboot();
            sourceAndRebootButton.Interactable = false;
        }

        public void OnDelete() {
            var filepath = fileNameInputField.Value;

            File.Delete(filepath);
            Mainframe.Instance.Reboot();

            Close();
        }
    }
}
