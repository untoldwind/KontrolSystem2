using System;
using System.Linq;
using KontrolSystem.KSP.Runtime.Core;
using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPGame;

public partial class KSPGameModule {
    [KSClass("Process")]
    public class ProcessAdapter {
        private readonly KontrolSystemProcess process;

        public ProcessAdapter(KontrolSystemProcess process, EntrypointArgumentDescriptor[] arguments) {
            this.process = process;
            this.Arguments = arguments.Select(arg => new ProcessArgumentAdapter(arg)).ToArray();
        }

        [KSField] public string Name => process.Name;

        [KSField]
        public bool IsRunning => process.State == KontrolSystemProcessState.Running ||
                                 process.State == KontrolSystemProcessState.Outdated;

        [KSField] public ProcessArgumentAdapter[] Arguments { get; }

        [KSMethod]
        public bool Start(Option<KSPVesselModule.VesselAdapter> forVessel = new(),
            string[]? arguments = null) {
            object[]? argumentObjs = null;
            if (arguments != null) {
                argumentObjs = new object[Math.Min(this.Arguments.Length, arguments.Length)];

                for (var i = 0; i < argumentObjs.Length; i++)
                    switch (this.Arguments[i].Type) {
                    case "int":
                        if (long.TryParse(arguments[i], out var lValue)) argumentObjs[i] = lValue;
                        break;
                    case "float":
                        if (double.TryParse(arguments[i], out var dValue)) argumentObjs[i] = dValue;
                        break;
                    case "bool":
                        argumentObjs[i] = arguments[i].ToLower() == "true";
                        break;
                    case "string":
                        argumentObjs[i] = arguments[i];
                        break;
                    }
            }

            var context = KSPContext.CurrentContext;
            bool result;
            try {
                result = Mainframe.Instance!.StartProcess(process, forVessel.value?.vessel, argumentObjs);
            } finally {
                ContextHolder.CurrentContext.Value = context;
            }

            return result;
        }

        [KSMethod]
        public bool Stop() {
            var context = KSPContext.CurrentContext;
            bool result;

            try {
                result = Mainframe.Instance!.StopProcess(process);
            } finally {
                ContextHolder.CurrentContext.Value = context;
            }

            return result;
        }
    }
}
