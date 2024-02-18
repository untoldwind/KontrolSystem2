using System;
using System.Globalization;
using System.Text;
using KontrolSystem.KSP.Runtime.Core;
using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using TMPro;
using UniLinq;
using UnityEngine;
using UnityEngine.UI;

namespace KontrolSystem.KSP.Runtime.KSPUI.Builtin;

public class ModuleManagerWindow : UGUIResizableWindow {
    private RawImage? mainframeStateIcon;

    public void OnEnable() {
        Initialize($"KontrolSystem {Mainframe.Instance!.Version}", new Rect(Screen.width - 750, 650, 550, 450));

        var root = RootVerticalLayout();

        var mainframeState = new GameObject("MainframeState", typeof(RawImage));
        UIFactory.Layout(mainframeState, windowTransform!, UIFactory.LAYOUT_START, UIFactory.LAYOUT_START,
            5, -5, UIFactory.Instance!.uiFontSize + 10, UIFactory.Instance.uiFontSize + 10);
        mainframeStateIcon = mainframeState.GetComponent<RawImage>();
        mainframeStateIcon.texture = UIFactory.Instance.stateActive;

        var tabbedPanels = new UITabbedPanels(new UITab[]
            { new EntrypointsUITab(), new StateUITab(), new ModulesUITab() }, new Vector2(300, 300));
        root.Add(tabbedPanels, UGUILayout.Align.Stretch, 1);

        MinSize = root.Layout();

        if (!Mainframe.Instance.Initialized) {
            Mainframe.Instance.Logger.Debug("Lazy Initialize KontrolSystemMod");
            mainframeStateIcon.texture = UIFactory.Instance.stateInactive;
            Mainframe.Instance.Reboot();
        }

        Mainframe.Instance.availableProcessesChanged.AddListener(OnProcessesChanged);
    }

    public override void OnDisable() {
        base.OnDisable();
        Mainframe.Instance!.availableProcessesChanged.RemoveListener(OnProcessesChanged);
    }

    internal void OnProcessesChanged() {
        if (Mainframe.Instance!.Rebooting)
            mainframeStateIcon!.texture = UIFactory.Instance!.stateInactive;
        else if (Mainframe.Instance.LastErrors.Any())
            mainframeStateIcon!.texture = UIFactory.Instance!.stateError;
        else
            mainframeStateIcon!.texture = UIFactory.Instance!.stateActive;
    }

    internal class EntrypointsUITab : UITab {
        private Action? onParameterPopupClose;
        private UGUILayoutContainer? parameterPopup;
        private Transform? parent;
        private UIList<KontrolSystemProcessWithArguments, UIProcessElement>? processList;
        private UGUIButton? rebootButton;

        public EntrypointsUITab() : base("Entrypoints") {
        }

        public override void Create(RectTransform parent) {
            this.parent = parent;

            var horizonal = new UGUIHorizontalLayout(parent);

            processList = new UIList<KontrolSystemProcessWithArguments, UIProcessElement>(
                UIFactory.Instance!.uiFontSize + 10,
                element => new UIProcessElement(element, ShowParameterPopup, CloseParameterPopup));

            horizonal.Add(UGUIElement.VScrollView(processList, new Vector2(200, 200)), UGUILayout.Align.Stretch, 1);

            var vertical = horizonal.Add(UGUILayoutContainer.Vertical(20, new UGUILayout.Padding(0, 10, 0, 0))).Item1;

            rebootButton = vertical.Add(UGUIButton.Create("Rebooting...", Mainframe.Instance!.Reboot)).Item1;
            vertical.AddSpace(0, 1);
            vertical.Add(UGUIButton.Create("New Module", UIWindows.Instance!.OpenEditorWindowNew));
            vertical.Add(UGUIButton.Create("Telemetry", UIWindows.Instance.OpenTelemetryWindow));
            vertical.Add(UGUIButton.Create("Console", UIWindows.Instance.OpenConsoleWindow));

            horizonal.Layout();

            OnProcessesChanged();
            Mainframe.Instance.availableProcessesChanged.AddListener(OnProcessesChanged);
        }

        internal void ShowParameterPopup(KontrolSystemProcessWithArguments element, Action onClose) {
            CloseParameterPopup();

            parameterPopup = UGUILayoutContainer.VerticalPanel();
            onParameterPopupClose = onClose;

            parameterPopup.Add(UGUILabel.Create(element.process.Name), UGUILayout.Align.Center).Item1.HorizontalAlignment =
                HorizontalAlignmentOptions.Center;

            var horizontal = parameterPopup.Add(UGUILayoutContainer.Horizontal(5)).Item1;
            var labelVertical = horizontal.Add(UGUILayoutContainer.Vertical(5)).Item1;
            var inputVertical = horizontal.Add(UGUILayoutContainer.Vertical(5), UGUILayout.Align.Stretch, 1).Item1;

            for (var i = 0; i < element.argumentDescriptors.Length; i++) {
                var updateArgument = element.UpdateArgument(i);
                labelVertical.Add(UGUILabel.Create(element.argumentDescriptors[i].Name));

                if (element.argumentDescriptors[i].Type == BuiltinType.Bool) {
                    var parameterInput = inputVertical.Add(UGUIToggle.Create("", value => updateArgument(value))).Item1;
                    parameterInput.IsOn = (bool)element.arguments[i];
                } else {
                    if (element.argumentDescriptors[i].Type == BuiltinType.Int) {
                        var parameterInput =
                            inputVertical.Add(
                                UGUIInputField.Create("", 200, value => updateArgument(long.Parse(value)))).Item1;
                        parameterInput.CharacterValidation = TMP_InputField.CharacterValidation.Integer;
                        parameterInput.Value = ((long)element.arguments[i]).ToString();
                    } else if (element.argumentDescriptors[i].Type == BuiltinType.Float) {
                        var parameterInput =
                            inputVertical.Add(UGUIInputField.Create("", 200,
                                value => updateArgument(double.Parse(value)))).Item1;
                        parameterInput.CharacterValidation = TMP_InputField.CharacterValidation.Decimal;
                        parameterInput.Value = ((double)element.arguments[i]).ToString(CultureInfo.InvariantCulture);
                    } else if (element.argumentDescriptors[i].Type == BuiltinType.String) {
                        var parameterInput =
                            inputVertical.Add(UGUIInputField.Create("", 200, value => updateArgument(value))).Item1;
                        parameterInput.Value = (string)element.arguments[i];
                    } else {
                        var parameterInput = inputVertical.Add(UGUIInputField.Create("", 200)).Item1;
                        parameterInput.Interactable = false;
                        parameterInput.Value = $"Invalid type ${element.argumentDescriptors[i].Type}";
                    }
                }
            }

            var buttonHorizontal =
                parameterPopup.Add(UGUILayoutContainer.Horizontal(10, new UGUILayout.Padding(0, 0, 10, 10))).Item1;

            buttonHorizontal.Add(UGUIButton.Create("Start", element.OnStartStop));
            buttonHorizontal.AddSpace(0, 1);
            buttonHorizontal.Add(UGUIButton.Create("Close", CloseParameterPopup));

            var minSize = parameterPopup.Layout();
            UIFactory.Layout(parameterPopup.GameObject, parent!, UIFactory.LAYOUT_START, UIFactory.LAYOUT_CENTER,
                -0.5f * minSize.x, 0, minSize.x, minSize.y);
        }

        internal void CloseParameterPopup() {
            if (parameterPopup != null) {
                parameterPopup.Destroy();
                onParameterPopupClose?.Invoke();
                parameterPopup = null;
                onParameterPopupClose = null;
            }
        }

        public override void OnDestroy() {
            Mainframe.Instance!.availableProcessesChanged.RemoveListener(OnProcessesChanged);
        }

        internal void OnProcessesChanged() {
            var gameMode = Mainframe.Instance!.GameMode;
            processList!.Elements = Mainframe.Instance.AvailableProcesses.Select(process =>
                    new KontrolSystemProcessWithArguments(process, process.EntrypointArgumentDescriptors(gameMode)))
                .ToArray();
            rebootButton!.Interactable = !Mainframe.Instance.Rebooting;
            rebootButton.Label = Mainframe.Instance.Rebooting ? "Rebooting..." : "Reboot";
        }
    }

    internal class StateUITab : UITab {
        private UGUIButton? rebootButton;
        private TextMeshProUGUI? rebootMessages;

        internal StateUITab() : base("Reboot Errors") {
        }

        public override void Create(RectTransform parent) {
            var horizonal = new UGUIHorizontalLayout(parent);

            var rebootErrors = UIFactory.Instance!.CreateText("", 20, HorizontalAlignmentOptions.Left,
                VerticalAlignmentOptions.Top);
            rebootMessages = rebootErrors.GetComponent<TextMeshProUGUI>();

            var rebootErrorsScroll = UIFactory.Instance.CreateScrollView(rebootErrors);
            horizonal.Add(rebootErrorsScroll, UGUILayout.Align.Stretch, new Vector2(200, 200), 1);
            rebootMessages.enableWordWrapping = true;
            rebootMessages.verticalAlignment = VerticalAlignmentOptions.Top;

            var vertical = horizonal.Add(UGUILayoutContainer.Vertical(20, new UGUILayout.Padding(0, 10, 0, 0))).Item1;

            rebootButton = vertical.Add(UGUIButton.Create("Rebooting...", Mainframe.Instance!.Reboot)).Item1;
            vertical.Add(UGUIButton.Create("Copy", OnCopyErrors));

            horizonal.Layout();

            OnProcessesChanged();
            Mainframe.Instance.availableProcessesChanged.AddListener(OnProcessesChanged);
        }

        public override void OnDestroy() {
            Mainframe.Instance!.availableProcessesChanged.RemoveListener(OnProcessesChanged);
        }

        internal void OnProcessesChanged() {
            rebootButton!.Interactable = !Mainframe.Instance!.Rebooting;
            rebootButton.Label = Mainframe.Instance.Rebooting ? "Rebooting..." : "Reboot";

            var sb = new StringBuilder();
            if (Mainframe.Instance.Rebooting)
                sb.Append("Rebooting...\n");
            else
                sb.Append($"Rebooted in {Mainframe.Instance.LastRebootTime}\n");
            if (!Mainframe.Instance.LastErrors.Any())
                sb.Append("<color=green>No errors</color>\n");
            else
                foreach (var error in Mainframe.Instance.LastErrors) {
                    sb.Append($"<color=red>ERROR: </color> [{error.position}] {error.errorType}\n");
                    sb.Append("    <color=white>" + error.message + "</color>\n");
                }

            rebootMessages!.text = sb.ToString();
        }

        private void OnCopyErrors() {
            var sb = new StringBuilder();

            sb.Append($"Rebooted in {Mainframe.Instance!.LastRebootTime}\n");
            if (!Mainframe.Instance.LastErrors.Any())
                sb.Append("\nNo errors\n");
            else
                foreach (var error in Mainframe.Instance.LastErrors)
                    sb.Append($"\nERROR: [{error.position}] {error.errorType}\n{error.message}\n");

            GUIUtility.systemCopyBuffer = sb.ToString();
        }
    }

    internal class ModulesUITab : UITab {
        private UGUIButton? editButton;
        private TextMeshProUGUI? moduleDescriptionText;
        private UIList<ModuleListElement, UIModuleElement>? moduleList;
        private IKontrolModule? selectedModule;

        internal ModulesUITab() : base("Modules") {
        }

        public override void Create(RectTransform parent) {
            var horizonal = new UGUIHorizontalLayout(parent);

            moduleList =
                new UIList<ModuleListElement, UIModuleElement>(30,
                    element => new UIModuleElement(element, OnSelectModule));

            horizonal.Add(UGUIElement.VScrollView(moduleList, new Vector2(200, 200)), UGUILayout.Align.Stretch, 0.6f);

            var vertical = horizonal.Add(UGUILayoutContainer.Vertical(), UGUILayout.Align.Stretch, 0.4f).Item1;

            var panel = UGUILayoutContainer.VerticalPanel();
            vertical.Add(panel, UGUILayout.Align.Stretch, 1);

            var moduleDescription = UIFactory.Instance!.CreateText("");
            panel.Add(moduleDescription, UGUILayout.Align.Stretch, new Vector2(150, 100), 1);
            moduleDescriptionText = moduleDescription.GetComponent<TextMeshProUGUI>();
            moduleDescriptionText.fontSize = 16;
            moduleDescriptionText.enableWordWrapping = true;
            moduleDescriptionText.verticalAlignment = VerticalAlignmentOptions.Top;


            editButton = vertical.Add(UGUIButton.Create("Edit", () => {
                if (selectedModule != null && !selectedModule.IsBuiltin)
                    UIWindows.Instance!.OpenEditorWindow(selectedModule.SourceFile!);
            }), UGUILayout.Align.Start).Item1;

            horizonal.Layout();

            OnProcessesChanged();
            Mainframe.Instance!.availableProcessesChanged.AddListener(OnProcessesChanged);
        }

        public override void OnDestroy() {
            Mainframe.Instance!.availableProcessesChanged.RemoveListener(OnProcessesChanged);
        }

        internal void OnSelectModule(IKontrolModule module) {
            if (selectedModule != module) {
                selectedModule = module;
                OnProcessesChanged();
            }
        }

        internal void OnProcessesChanged() {
            var modules = Mainframe.Instance!.LastRegistry?.modules.Values.ToArray() ??
                          Array.Empty<IKontrolModule>();

            moduleList!.Elements = modules.Select(module => new ModuleListElement(module, module == selectedModule))
                .ToArray();

            if (selectedModule != null) {
                var sb = new StringBuilder();
                sb.Append($"Name: {selectedModule.Name}\n");
                sb.Append($"Description: {selectedModule.Description}\n");
                sb.Append($"Builtin: {selectedModule.IsBuiltin}\n");
                if (!selectedModule.IsBuiltin) sb.Append($"Source: {selectedModule.SourceFile}");
                moduleDescriptionText!.text = sb.ToString();
                editButton!.Interactable = !selectedModule.IsBuiltin;
            } else {
                moduleDescriptionText!.text = "";
                editButton!.Interactable = false;
            }
        }
    }

    public class KontrolSystemProcessWithArguments {
        public EntrypointArgumentDescriptor[] argumentDescriptors;
        public object[] arguments;
        public KontrolSystemProcess process;

        public KontrolSystemProcessWithArguments(KontrolSystemProcess process,
            EntrypointArgumentDescriptor[] argumentDescriptors) {
            this.process = process;
            this.argumentDescriptors = argumentDescriptors;
            arguments = this.argumentDescriptors.Select(arg => arg.DefaultValue).ToArray();
        }

        public Action<object> UpdateArgument(int idx) {
            return value => { arguments[idx] = value; };
        }

        public void OnStartStop() {
            switch (process.State) {
            case KontrolSystemProcessState.Running:
            case KontrolSystemProcessState.Outdated:
                Mainframe.Instance!.StopProcess(process);
                break;
            case KontrolSystemProcessState.Available:
                Mainframe.Instance!.StartProcess(process, null, arguments);
                break;
            }
        }
    }

    internal class UIProcessElement : UIListElement<KontrolSystemProcessWithArguments> {
        private readonly TextMeshProUGUI label;
        private readonly GameObject parameterToggle;
        private readonly UGUILayoutContainer root;
        private readonly RawImage startStopIcon;

        internal UIProcessElement(KontrolSystemProcessWithArguments element,
            Action<KontrolSystemProcessWithArguments, Action> showParameters,
            Action closeParameters) {
            var uiFontSize = UIFactory.Instance!.uiFontSize;
            this.Element = element;

            root = UGUILayoutContainer.Horizontal(5);

            var label = UIFactory.Instance.CreateText($"{element.process.Name} ({element.process.State})",
                uiFontSize - 2);
            root.Add(label, UGUILayout.Align.Stretch, new Vector2(150, uiFontSize + 10), 1);
            this.label = label.GetComponent<TextMeshProUGUI>();

            var parameters = element.process.EntrypointArgumentDescriptors(Mainframe.Instance!.GameMode);
            parameterToggle = UIFactory.Instance.CreateSelectButton(parameters.Length.ToString());
            root.Add(parameterToggle, UGUILayout.Align.Center, new Vector2(uiFontSize + 4, uiFontSize + 4));
            parameterToggle.SetActive(parameters.Length > 0);
            parameterToggle.GetComponent<Toggle>().onValueChanged.AddListener(isOn => {
                if (isOn)
                    showParameters(this.Element, () => { parameterToggle.GetComponent<Toggle>().isOn = false; });
                else
                    closeParameters();
            });

            var startStop = UIFactory.Instance.CreateIconButton(
                element.process.State == KontrolSystemProcessState.Running ||
                element.process.State == KontrolSystemProcessState.Outdated
                    ? UIFactory.Instance.stopIcon
                    : UIFactory.Instance.startIcon);
            root.Add(startStop, UGUILayout.Align.Center, new Vector2(uiFontSize + 4, uiFontSize + 4));
            startStopIcon = startStop.GetComponentInChildren<RawImage>();
            startStop.GetComponent<Button>().onClick.AddListener(() => this.Element.OnStartStop());

            root.Layout();
        }

        public KontrolSystemProcessWithArguments Element { get; private set; }

        public GameObject Root => root.GameObject;

        public void Update(KontrolSystemProcessWithArguments element) {
            this.Element = element;
            label.SetText($"{element.process.Name} ({element.process.State})");
            startStopIcon.texture = element.process.State == KontrolSystemProcessState.Running ||
                                    element.process.State == KontrolSystemProcessState.Outdated
                ? UIFactory.Instance!.stopIcon
                : UIFactory.Instance!.startIcon;
            parameterToggle.GetComponentInChildren<TextMeshProUGUI>().text =
                element.argumentDescriptors.Length.ToString();
            parameterToggle.SetActive(element.argumentDescriptors.Length > 0);
        }
    }

    internal struct ModuleListElement {
        internal IKontrolModule kontrolModule;
        internal bool selected;

        public ModuleListElement(IKontrolModule kontrolModule, bool selected) {
            this.kontrolModule = kontrolModule;
            this.selected = selected;
        }
    }

    internal class UIModuleElement : UIListElement<ModuleListElement> {
        private readonly UGUILayoutContainer root;
        private readonly UGUIToggle selectButton;

        public UIModuleElement(ModuleListElement element, Action<IKontrolModule> onSelect) {
            this.Element = element;

            root = UGUILayoutContainer.Horizontal();
            selectButton = root.Add(UGUIToggle.CreateSelectButton(element.kontrolModule.Name, isOn => {
                if (isOn) onSelect(element.kontrolModule);
            }), UGUILayout.Align.Stretch, 1).Item1;
            selectButton.FontSize = 16;
            selectButton.IsOn = element.selected;

            root.Layout();
        }

        public ModuleListElement Element { get; private set; }

        public GameObject Root => root.GameObject;

        public void Update(ModuleListElement element) {
            this.Element = element;

            selectButton.Label = element.kontrolModule.Name;
            selectButton.IsOn = element.selected;
        }
    }
}
