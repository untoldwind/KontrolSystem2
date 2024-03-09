using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleParachute")]
    public class ModuleParachuteAdapter(KSPVesselModule.PartAdapter part, Data_Parachute dataParachute) : BaseParachuteAdapter<PartAdapter, PartComponent>(part, dataParachute) {
        [KSMethod]
        public bool Deploy() {
            if (!KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(part.part.SimulationObject,
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
            if (!KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(part.part.SimulationObject,
                    out var viewObject)) return false;

            if (!viewObject.TryGetComponent<Module_Parachute>(out var moduleParachute)) return false;

            moduleParachute.CutParachute();

            return dataParachute.deployState.GetValue() == Data_Parachute.DeploymentStates.CUT;
        }

        [KSMethod]
        public bool Repack() {
            if (!KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(part.part.SimulationObject,
                    out var viewObject)) return false;

            if (!viewObject.TryGetComponent<Module_Parachute>(out var moduleParachute)) return false;

            moduleParachute.Repack();

            return dataParachute.deployState.GetValue() == Data_Parachute.DeploymentStates.STOWED;
        }
    }
}
