using KontrolSystem.TO2.Binding;
using KSP.Modules;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("EngineData")]
        public class EngineDataAdapter {
            private readonly Data_Engine dataEngine;

            public EngineDataAdapter(Data_Engine dataEngine) => this.dataEngine = dataEngine;
            
            [KSField] public bool IsShutdown => dataEngine.EngineShutdown;

            [KSField] public bool HasIgnited => dataEngine.EngineIgnited;

            [KSField] public bool IsFlameout => dataEngine.Flameout;

            [KSField] public bool IsStaged => dataEngine.staged;

            [KSField] public double ThrottleMin => dataEngine.ThrottleMin;

            [KSField] public double MinFuelFlow => dataEngine.MinFuelFlow;
            
            [KSField] public double MaxFuelFlow => dataEngine.MaxFuelFlow;

            [KSField] public double MaxThrustOutputVac => dataEngine.MaxThrustOutputVac(true);

            [KSField] public double MaxThrustOutputAtm => dataEngine.MaxThrustOutputAtm();
        }
    }
}
