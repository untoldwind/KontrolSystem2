using System;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI.UGUI;

public class UGUILifecycleCallback : MonoBehaviour {
    private Action? onDestroyCallback;

    public void OnDestroy() {
        onDestroyCallback?.Invoke();
    }

    public void AddOnDestroy(Action action) {
        onDestroyCallback += action;
    }
}
