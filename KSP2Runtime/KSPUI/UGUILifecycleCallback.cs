using System;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public class UGUILifecycleCallback : MonoBehaviour {
        private Action onDestroyCallback;

        public void AddOnDestroy(Action action) {
            onDestroyCallback += action;
        }

        public void OnDestroy() {
            onDestroyCallback?.Invoke();
        }
    }
}
