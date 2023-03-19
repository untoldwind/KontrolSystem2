using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;
using System;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("ModuleParachute")]
        public class ModuleParachuteAdapter {
            private readonly PartComponent part;
            private readonly Data_Parachute dataParachute;

            public ModuleParachuteAdapter(PartComponent part, Data_Parachute dataParachute) {
                this.part = part;
                this.dataParachute = dataParachute;
            }

            [KSField]
            public string DeployState => dataParachute.deployState.GetValue().ToString();

            [KSField]
            public string ChuteSafety => dataParachute.deploymentSafetyState.GetValue().ToString();

            [KSField]
            public string DeployMode {
                get => dataParachute.DeploymentMode.GetValue().ToString();
                set {
                    if (Enum.TryParse(value, true, out Data_Parachute.DeployMode mode)) {
                        dataParachute.DeploymentMode.SetValue(mode);
                    }
                }
            }

            [KSField]
            public double MinAirPressure { //todo: clamp this setter
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

                bool willDeploy = dataParachute.deployState.GetValue() == Data_Parachute.DeploymentStates.STOWED && (!moduleParachute.part.ShieldedFromAirstream || dataParachute.shieldedCanDeploy); // one of the fields needed to check whether this was successful is private

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
}
