using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleParachute")]
    public class ModuleParachuteAdapter {
        private readonly Data_Parachute dataParachute;
        private readonly PartComponent part;

        public ModuleParachuteAdapter(PartComponent part, Data_Parachute dataParachute) {
            this.part = part;
            this.dataParachute = dataParachute;
        }

        [KSField] public Data_Parachute.DeploymentStates DeployState => dataParachute.deployState.GetValue();

        [KSField]
        public Data_Parachute.DeploymentSafeStates ChuteSafety => dataParachute.deploymentSafetyState.GetValue();

        [KSField]
        public Data_Parachute.DeployMode DeployMode {
            get => dataParachute.DeploymentMode.GetValue();
            set => dataParachute.DeploymentMode.SetValue(value);
        }

        [KSField]
        public double MinAirPressure {
            get => dataParachute.minAirPressureToOpen.GetValue();
            set => dataParachute.minAirPressureToOpen.SetValue(Mathf.Clamp((float)value, 0.01f, 0.75f));
        }

        [KSField]
        public double DeployAltitude {
            get => dataParachute.deployAltitude.GetValue();
            set => dataParachute.deployAltitude.SetValue(Mathf.Clamp((float)value, 50f, 5000f));
        }

        [KSField]
        public bool Armed {
            get => dataParachute.armedToggle.GetValue();
            set => dataParachute.armedToggle.SetValue(value);
        }

        [KSMethod]
        public bool Deploy() {
            if (!KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(part.SimulationObject,
                    out var viewObject)) return false;

            if (!viewObject.TryGetComponent<Module_Parachute>(out var moduleParachute)) return false;

            var willDeploy = dataParachute.deployState.GetValue() == Data_Parachute.DeploymentStates.STOWED &&
                             (!moduleParachute.part.ShieldedFromAirstream ||
                              dataParachute
                                  .shieldedCanDeploy); // one of the fields needed to check whether this was successful is private

            moduleParachute.DeployNow();

            return willDeploy;
        }

        [KSMethod]
        public bool Cut() {
            if (!KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(part.SimulationObject,
                    out var viewObject)) return false;

            if (!viewObject.TryGetComponent<Module_Parachute>(out var moduleParachute)) return false;

            moduleParachute.CutParachute();

            return dataParachute.deployState.GetValue() == Data_Parachute.DeploymentStates.CUT;
        }

        [KSMethod]
        public bool Repack() {
            if (!KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(part.SimulationObject,
                    out var viewObject)) return false;

            if (!viewObject.TryGetComponent<Module_Parachute>(out var moduleParachute)) return false;

            moduleParachute.Repack();

            return dataParachute.deployState.GetValue() == Data_Parachute.DeploymentStates.STOWED;
        }
    }
}
