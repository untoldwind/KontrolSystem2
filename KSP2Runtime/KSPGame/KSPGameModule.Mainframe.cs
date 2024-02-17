using System.Linq;
using KontrolSystem.KSP.Runtime.Core;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPGame;

public partial class KSPGameModule {
    [KSClass("Mainframe")]
    public class MainframeAdapter {
        [KSField]
        public ProcessAdapter[] AvailableProcesses {
            get {
                var gameMode = Mainframe.Instance!.GameMode;
                return Mainframe.Instance.AvailableProcesses.Select(process =>
                    new ProcessAdapter(process, process.EntrypointArgumentDescriptors(gameMode))).ToArray();
            }
        }

        [KSMethod]
        public Option<ProcessAdapter> FindProcess(string name) {
            var process = Mainframe.Instance!.AvailableProcesses.FirstOrDefault(process => process.Name == name);

            if (process != null) {
                var gameMode = Mainframe.Instance.GameMode;
                return new Option<ProcessAdapter>(new ProcessAdapter(process,
                    process.EntrypointArgumentDescriptors(gameMode)));
            }

            return new Option<ProcessAdapter>();
        }
    }
}
