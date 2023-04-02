using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Experiments {
    public class ModuleManagerWindow : UGUIResizableWindow {
        private RawImage mainframeStateIcon;

        public void OnEnable() {
            Initialize("MainframeState", new Rect(100, 500, 550, 400));

            var root = RootVerticalLayout();
            
            var mainframeState = new GameObject("MainframeState", typeof(RawImage));
            UIFactory.Layout(mainframeState, windowTransform, UIFactory.LAYOUT_START, UIFactory.LAYOUT_START, 5, -5, 30, 30); 
            mainframeStateIcon = mainframeState.GetComponent<RawImage>();
            mainframeStateIcon.texture = UIFactory.Instance.stateActive;
            
            var tabbedPanels = new UITabbedPanels(new UITab[] { new EntrypointsUITab(), new StateUITab(), new ModulesUITab() }, new Vector2(300, 300));
            root.Add(tabbedPanels, UGUILayout.Align.STRETCH, 1);
            
            MinSize = root.MinSize;
            root.Layout();
        }

        internal class EntrypointsUITab : UITab {
            private Transform parent;
            private UIList<FakeProcess, UIProcessElement> processList;
            private UGUILayoutContainer parameterPopup;
            private Action onParameterPopupClose;

            internal EntrypointsUITab() : base("Entrypoints") {
            }
            
            public override void Create(RectTransform parent) {
                this.parent = parent;

                var horizonal = new UGUIHorizontalLayout(parent);
                
                processList = new UIList<FakeProcess, UIProcessElement>(30, element => new UIProcessElement(element, ShowParameterPopup, CloseParameterPopup));
                horizonal.Add(UGUIElement.VScrollView(processList, new Vector2(200, 200)), UGUILayout.Align.STRETCH, 1);

                var processes = new FakeProcess[100];

                for (int i = 0; i < 100; i++) {
                    processes[i] = new FakeProcess($"Process {i}", i % 3 == 0, i % 4);
                }

                processList.Elements = processes;

                var vertical = horizonal.Add(UGUILayoutContainer.Vertical(20, new Padding(0,10,0,0)));

                var rebootButton = UGUIButton.Create("Reboot", OnReboot);
                vertical.Add(rebootButton);

                vertical.AddSpace(0, 1);
                
                var newModuleButton = UGUIButton.Create("New Module", () => { });
                vertical.Add(newModuleButton);
            
                var telemetryButton = UGUIButton.Create("Telemetry", () => { });
                vertical.Add(telemetryButton);
            
                var consoleButton = UGUIButton.Create("Console", () => { });
                vertical.Add(consoleButton);
                
                horizonal.Layout();
            }

            internal void ShowParameterPopup(FakeProcess process, Action onClose) {
                CloseParameterPopup();

                parameterPopup = UGUILayoutContainer.VerticalPanel();
                onParameterPopupClose = onClose;

                parameterPopup.Add(UGUILabel.Create(process.name), UGUILayout.Align.CENTER).HorizontalAlignment = HorizontalAlignmentOptions.Center;

                var buttonHorizontal = parameterPopup.Add(UGUILayoutContainer.Horizontal(10, new Padding(0,0, 10, 10)));

                buttonHorizontal.Add(UGUIButton.Create("Start", () => { }));
                buttonHorizontal.AddSpace(0, 1);
                buttonHorizontal.Add(UGUIButton.Create("Close", CloseParameterPopup));

                var minSize = parameterPopup.Layout();
                UIFactory.Layout(parameterPopup.GameObject, parent, UIFactory.LAYOUT_START, UIFactory.LAYOUT_CENTER, -0.5f * minSize.x, 0, minSize.x, minSize.y);
            }

            internal void CloseParameterPopup() {
                if (parameterPopup != null) {
                    parameterPopup.Destroy();
                    onParameterPopupClose();
                    parameterPopup = null;
                    onParameterPopupClose = null;
                }
            }

            internal void OnReboot() {
                var processes = new FakeProcess[100];

                for (int i = 0; i < 100; i++) {
                    processes[i] = new FakeProcess($"Process {i + 10}", i % 4 == 0, i % 5);
                }
                
                processList.Elements = processes;
            }

            public virtual void OnDestroy() {
                CloseParameterPopup();
            }
        }
        
        internal class StateUITab : UITab {
            internal StateUITab() : base("Reboot Errors") {
            }
            
            public override void Create(RectTransform parent) {

                var horizonal = new UGUIHorizontalLayout(parent);

                var t = UIFactory.Instance.CreateText("Abc\nAbc\nAbc\nAbc\nAbc\nAbc\nAbc\nAbc\nAbc\nAbc\nAbc\nAbc\nAbc\nAbc\nAbc\nAbc\nAbc\nAbc\nAbc", 20, HorizontalAlignmentOptions.Left, VerticalAlignmentOptions.Top);

                var h = UIFactory.Instance.CreateScrollView(t);
                horizonal.Add(h, UGUILayout.Align.STRETCH, new Vector2(200, 200), 1);
                
                var vertical = horizonal.Add(UGUILayoutContainer.Vertical(20, new Padding(0,10,0,0)));

                var rebootButton = vertical.Add(UGUIButton.Create("Rebooting...", () => { }));
                vertical.Add(UGUIButton.Create("Copy", () => { }));

                horizonal.Layout();
            }
        }

        internal class ModulesUITab : UITab {
            private UIList<FakeModule, UIModuleElement> moduleList;
            private GameObject editButton;
            
            internal ModulesUITab() : base("Modules") {
            }
            
            public override void Create(RectTransform parent) {
                moduleList = new UIList<FakeModule, UIModuleElement>(30, element => new UIModuleElement(element));

                var h = UIFactory.Instance.CreateScrollView(moduleList.GameObject);
                UIFactory.Layout(h, parent, new UIFactory.LayoutAlign(0, 0.6f, 0), UIFactory.LAYOUT_STRECH, 10, 0, -15, 0);

                var processes = new FakeModule[100];

                for (int i = 0; i < 100; i++) {
                    processes[i] = new FakeModule($"Module {i}");
                }

                moduleList.Elements = processes;
                
                var panel2 = UIFactory.Instance.CreatePanel();
                UIFactory.Layout(panel2, parent, new UIFactory.LayoutAlign(0.6f, 1, 0), UIFactory.LAYOUT_STRECH, 5, 0, -15, -40);
                
                editButton = UIFactory.Instance.CreateButton("Edit");
                UIFactory.Layout(editButton, parent, new UIFactory.LayoutAlign(0.6f, 0.6f, 0), UIFactory.LAYOUT_END, 5, 0, 120, 30);
                editButton.GetComponent<Button>().interactable = false;
            }
        }

        internal class UIProcessElement : UIListElement<FakeProcess> {
            private readonly GameObject root;
            private readonly TextMeshProUGUI label;
            private readonly RawImage startStopIcon;
            private readonly GameObject parameterToggle;
            private FakeProcess element;

            internal UIProcessElement(FakeProcess element, Action<FakeProcess, Action> showParameters, Action closeParameters) {
                this.element = element;
                
                root = new GameObject("ProcessElement", typeof(RectTransform));
                
                GameObject label = UIFactory.Instance.CreateText(element.name, 18);
                UIFactory.Layout(label, root.transform, UIFactory.LAYOUT_STRECH, UIFactory.LAYOUT_START, 0, 0, -60, 24);
                this.label = label.GetComponent<TextMeshProUGUI>();

                parameterToggle = UIFactory.Instance.CreateSelectButton(element.parameters.ToString());
                UIFactory.Layout(parameterToggle, root.transform, UIFactory.LAYOUT_END, UIFactory.LAYOUT_START, -30, 0, 24, 24);
                parameterToggle.SetActive(element.parameters > 0);
                parameterToggle.GetComponent<Toggle>().onValueChanged.AddListener(isOn => {
                    if (isOn) {
                        showParameters(this.element, () => {
                            parameterToggle.GetComponent<Toggle>().isOn = false;
                        });
                    } else {
                        closeParameters();
                    }
                });
                
                GameObject startStop = UIFactory.Instance.CreateIconButton(element.isRunning ? UIFactory.Instance.stopIcon : UIFactory.Instance.startIcon);
                UIFactory.Layout(startStop, root.transform, UIFactory.LAYOUT_END, UIFactory.LAYOUT_START, 0, 0, 24, 24);
                startStopIcon = startStop.GetComponentInChildren<RawImage>();
            }
            
            public FakeProcess Element => element;
            
            public GameObject Root => root;
            
            public void Update(FakeProcess element) {
                this.element = element;
                label.SetText(element.name);
                startStopIcon.texture = element.isRunning ? UIFactory.Instance.stopIcon : UIFactory.Instance.startIcon;
                parameterToggle.GetComponentInChildren<TextMeshProUGUI>().text = element.parameters.ToString();
                parameterToggle.SetActive(element.parameters > 0);
            }
        }

        internal class UIModuleElement : UIListElement<FakeModule> {
            private readonly GameObject root;
            private readonly GameObject selectButton;
            private FakeModule element;

            internal UIModuleElement(FakeModule element) {
                this.element = element;

                root = new GameObject("ModuleElement", typeof(RectTransform));

                selectButton = UIFactory.Instance.CreateSelectButton(element.name);
                UIFactory.Layout(selectButton, root.transform, UIFactory.LAYOUT_STRECH, UIFactory.LAYOUT_STRECH, 
                    0, 0, 0, 0);

                selectButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 16;
            }

            public FakeModule Element => element;

            public GameObject Root => root;
            
            public void Update(FakeModule element) {
                this.element = element;
                
                selectButton.GetComponent<TextMeshProUGUI>().text = this.element.name;
            }
        }
    }
}
