using System.Linq;
using KontrolSystem.KSP.Runtime.Core;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPGame {
    public partial class KSPGameModule {
        [KSClass("Mainframe")]
        public class MainframeAdapter {
            public ProcessAdapter[] AvailableProcesses {
                get {
                    var gameMode = Mainframe.Instance.GameMode;
                    return Mainframe.Instance.AvailableProcesses.Select(process =>
                        new ProcessAdapter(process, process.EntrypointArgumentDescriptors(gameMode))).ToArray();
                }
            }
        }
    }
}
