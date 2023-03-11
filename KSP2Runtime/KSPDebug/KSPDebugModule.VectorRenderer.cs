using System;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPDebug {
    public partial class KSPDebugModule {
        public class VectorRenderer : IMarker {
            private Func<Vector3d> startProvider;
            private Func<Vector3d> endProvider;

            public VectorRenderer(Func<Vector3d> startProvider, Func<Vector3d> endProvider,
                KSPConsoleModule.RgbaColor color, string label, double width, bool pointy) {
                this.startProvider = startProvider;
                this.endProvider = endProvider;
            }

            public bool Visible { get; set; }
            public void Update() {
            }

            public void OnRender() {
            }

            [KSField(Description = "The current starting position of the debugging vector.")]
            public Vector3d Start {
                get => startProvider();
                set => startProvider = () => value;
            }


            [KSField(Description = "The current end position of the debugging vector.")]
            public Vector3d End {
                get => endProvider();
                set => endProvider = () => value;
            }

        }
    }
}
