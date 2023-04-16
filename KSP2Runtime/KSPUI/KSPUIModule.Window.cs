using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        [KSClass("Window")]
        public class Window {
            private UGUIResizableWindow window;
            private Container root;

            internal Window(string title, Rect initialRect) {
                window = UIWindows.Instance.gameObject.AddComponent<UGUIResizableWindow>();
                window.Initialize(title, initialRect);
            }

            [KSMethod]
            public Container Horizontal(double gap = 10) {
                if (root != null) return root;
                root = new Container(this, window.RootHorizontalLayout((float)gap));
                return root;
            }

            [KSMethod]
            public Container Vertical(double gap = 10) {
                if (root != null) return root;
                root = new Container(this, window.RootVerticalLayout((float)gap));
                return root;
            }

            internal void Layout() {
                if (root != null) {
                    window.MinSize = root.layout.Layout();
                }
            }

            [KSMethod]
            public void Close() {
                window.Close();
            }
        }
    }
}
