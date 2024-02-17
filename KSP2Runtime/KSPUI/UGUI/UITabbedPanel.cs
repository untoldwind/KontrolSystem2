using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KontrolSystem.KSP.Runtime.KSPUI.UGUI;

public abstract class UITab {
    public readonly string title;

    protected UITab(string title) {
        this.title = title;
    }

    public abstract void Create(RectTransform parent);

    public virtual void OnDestroy() {
    }
}

public class UITabbedPanels : UGUIElement {
    private readonly UITab[] tabs;
    private int currentTabIdx = -1;
    private GameObject? currentTabPanel;

    public UITabbedPanels(UITab[] tabs, Vector2 minSize) : base(
        new GameObject("TabbedPanels", typeof(RectTransform), typeof(ToggleGroup)), minSize) {
        this.tabs = tabs;
        var toggleGroup = GameObject.GetComponent<ToggleGroup>();
        var count = (float)tabs.Length;
        GameObject.AddComponent<UGUILifecycleCallback>().AddOnDestroy(OnDestroy);

        for (var i = 0; i < count; i++) {
            var tab = tabs[i];
            var tabButton = UIFactory.Instance!.CreateSelectButton(tab.title);
            var tabButtonTransform = tabButton.GetComponent<RectTransform>();
            tabButtonTransform.SetParent(GameObject.transform);
            tabButtonTransform.anchorMin = new Vector2(i / count, 1);
            tabButtonTransform.anchorMax = new Vector2((i + 1) / count, 1);
            tabButtonTransform.pivot = new Vector2(0, 1);
            tabButtonTransform.localPosition = Vector3.zero;
            tabButtonTransform.anchoredPosition = new Vector2(5, 0);
            tabButtonTransform.sizeDelta = new Vector2(-10, UIFactory.Instance.uiFontSize + 10);
            var tabButtonToggle = tabButton.GetComponent<Toggle>();
            tabButtonToggle.group = toggleGroup;
            tabButtonToggle.onValueChanged.AddListener(OnTabChange(i));
        }
    }

    private UnityAction<bool> OnTabChange(int tabIdx) {
        return delegate (bool on) {
            if (!on || tabIdx == currentTabIdx) return;

            if (currentTabIdx >= 0) tabs[currentTabIdx].OnDestroy();
            Object.Destroy(currentTabPanel);
            currentTabPanel = new GameObject("TabPanel", typeof(RectTransform));
            var tabPanelTransform = UIFactory.Layout(currentTabPanel, GameObject.transform,
                UIFactory.LAYOUT_STRETCH, UIFactory.LAYOUT_STRETCH, 0, -UIFactory.Instance!.uiFontSize - 20, 0,
                -UIFactory.Instance.uiFontSize - 20);

            tabs[tabIdx].Create(tabPanelTransform);
            currentTabIdx = tabIdx;
        };
    }

    private void OnDestroy() {
        if (currentTabIdx >= 0) tabs[currentTabIdx].OnDestroy();
    }
}
