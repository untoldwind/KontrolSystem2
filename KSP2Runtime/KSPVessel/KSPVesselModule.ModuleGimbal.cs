using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleGimbal")]
    public class ModuleGimbalAdapter {
        private readonly Data_Gimbal dataGimbal;
        private readonly PartComponent part;

        public ModuleGimbalAdapter(PartComponent part, Data_Gimbal dataGimbal) {
            this.dataGimbal = dataGimbal;
            this.part = part;
        }

        [KSField]
        public bool Enabled {
            get => dataGimbal.isEnabled.GetValue();
            set => dataGimbal.isEnabled.SetValue(value);
        }

        [KSField]
        public bool EnablePitch {
            get => dataGimbal.enablePitch.GetValue();
            set => dataGimbal.enablePitch.SetValue(value);
        }

        [KSField]
        public bool EnableYaw {
            get => dataGimbal.enableYaw.GetValue();
            set => dataGimbal.enableYaw.SetValue(value);
        }

        [KSField]
        public bool EnableRoll {
            get => dataGimbal.enableRoll.GetValue();
            set => dataGimbal.enableRoll.SetValue(value);
        }

        [KSField]
        public double Limiter {
            get => dataGimbal.gimbalLimiter.GetValue();
            set => dataGimbal.gimbalLimiter.SetValue((float)value);
        }

        [KSField]
        public Vector3d PitchYawRoll {
            get {
                if(!KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(part.SimulationObject,
                    out var viewObject)) return Vector3d.zero;
                if(!viewObject.TryGetComponent<Module_Gimbal>(out var moduleGimbal)) return Vector3d.zero;
                return ((Vector3d)moduleGimbal.GimbalActuation).SwapYAndZ;
            }
            set {
                if(!KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(part.SimulationObject,
                       out var viewObject)) return;
                if(!viewObject.TryGetComponent<Module_Gimbal>(out var moduleGimbal)) return;
                moduleGimbal.SetGimbalActuation((float)value.x, (float)value.z, (float)value.y);
            }
        }
    }
}
