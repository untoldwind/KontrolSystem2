using KontrolSystem.TO2.Binding;
using KSP.Modules;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("EngineMode")]
    public class EngineModeAdapter {
        private readonly Data_Engine.EngineMode engineMode;

        public EngineModeAdapter(Data_Engine.EngineMode engineMode) {
            this.engineMode = engineMode;
        }

        [KSField] public string Name => engineMode.engineID;

        [KSField] public bool AllowRestart => engineMode.allowRestart;

        [KSField] public bool AllowShutdown => engineMode.allowShutdown;

        [KSField] public bool ThrottleLocked => engineMode.throttleLocked;

        [KSField] public double MinThrust => engineMode.minThrust;

        [KSField] public double MaxThrust => engineMode.maxThrust;

        [KSField] public EngineType EngineType => engineMode.engineType;
    }
}
