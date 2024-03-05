using KontrolSystem.TO2.Binding;
using KSP.Sim;
using KSP.Sim.DeltaV;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("EngineDeltaV")]
    public class DeltaVEngineInfoAdapter : BaseDeltaVEngineInfoAdapter {
        private readonly VesselAdapter vesselAdapter;

        internal DeltaVEngineInfoAdapter(VesselAdapter vesselAdapter, DeltaVEngineInfo deltaVEngineInfo) : base(deltaVEngineInfo) {
            this.vesselAdapter = vesselAdapter;
        }

        [KSField]
        public ModuleEngineAdapter EngineModule => new(new PartAdapter(vesselAdapter, (PartComponent)deltaVEngineInfo.Part), deltaVEngineInfo.Engine);

        [KSMethod(Description = "Estimated thrust vector of the engine in a given `situation`")]
        public Vector3d GetThrustVector(DeltaVSituationOptions situation) =>
            vesselAdapter.vessel.mainBody.transform.celestialFrame.ToLocalVector(
                KSPContext.CurrentContext.Game.UniverseView.PhysicsSpace.PhysicsToVector(
                    deltaVEngineInfo.GetSituationThrustVector(situation)));

        [KSMethod(Description = "Coordinate independent estimated thrust vector of the engine in a given `situation`")]
        public Vector GetGlobalThrustVector(DeltaVSituationOptions situation) =>
                KSPContext.CurrentContext.Game.UniverseView.PhysicsSpace.PhysicsToVector(
                    deltaVEngineInfo.GetSituationThrustVector(situation));
    }
}
