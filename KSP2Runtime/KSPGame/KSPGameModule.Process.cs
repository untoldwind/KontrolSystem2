using System;
using System.Linq;
using KontrolSystem.KSP.Runtime.Core;
using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPGame {
    public partial class KSPGameModule {
        [KSClass("Process")]
        public class ProcessAdapter {
            private readonly KontrolSystemProcess process;
            private readonly ProcessArgumentAdapter[] arguments;

            public ProcessAdapter(KontrolSystemProcess process, EntrypointArgumentDescriptor[] arguments) {
                this.process = process;
                this.arguments = arguments.Select(arg => new ProcessArgumentAdapter(arg)).ToArray();
            }

            [KSField] public string Name => process.Name;

            [KSField]
            public bool IsRunning => process.State == KontrolSystemProcessState.Running ||
                                     process.State == KontrolSystemProcessState.Outdated;

            [KSField]
            public ProcessArgumentAdapter[] Arguments => arguments;

            [KSMethod]
            public bool Start(Option<KSPVesselModule.VesselAdapter> forVessel = new Option<KSPVesselModule.VesselAdapter>(),
                string[] arguments = null) {
                object[] argumentObjs = null;
                if (arguments != null) {
                    argumentObjs = new object[Math.Min(this.arguments.Length, arguments.Length)];
                    
                    for (int i = 0; i < argumentObjs.Length; i++) {
                        switch (this.arguments[i].Type) {
                        case "int":
                            if (long.TryParse(arguments[i], out var lValue)) {
                                argumentObjs[i] = lValue;
                            }
                            break;
                        case "float":
                            if (double.TryParse(arguments[i], out var dValue)) {
                                argumentObjs[i] = dValue;
                            }
                            break;
                        case "bool":
                            argumentObjs[i] = arguments[i].ToLower() == "true";
                            break;
                        case "string":
                            argumentObjs[i] = arguments[i];
                            break;
                        }
                    }
                }
                return Mainframe.Instance.StartProcess(process, forVessel.value?.vessel, argumentObjs);
            }

            [KSMethod]
            public bool Stop() {
                return Mainframe.Instance.StopProcess(process);
            }
        }
    }
}
