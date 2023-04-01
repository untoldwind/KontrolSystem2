﻿using KontrolSystem.TO2.Binding;
using KSP.Sim.DeltaV;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("EngineDeltaV")]
        public class DeltaVEngineInfoAdapter {
            private readonly VesselAdapter vesselAdapter;
            private readonly DeltaVEngineInfo deltaVEngineInfo;

            internal DeltaVEngineInfoAdapter(VesselAdapter vesselAdapter, DeltaVEngineInfo deltaVEngineInfo) {
                this.vesselAdapter = vesselAdapter;
                this.deltaVEngineInfo = deltaVEngineInfo;
            }

            [KSMethod("get_ISP", Description = "Estimated ISP of the engine in a given `situation`")]
            public double GetIsp(DeltaVSituationOptions situation) => deltaVEngineInfo.GetSituationISP(situation);

            [KSMethod(Description = "Estimated thrust of the engine in a given `situation`")]
            public double GetThrust(DeltaVSituationOptions situation) =>
                deltaVEngineInfo.GetSituationThrust(situation);

            [KSMethod(Description = "Estimated thrust vector of the engine in a given `situation`")]
            public Vector3d GetThrustVector(DeltaVSituationOptions situation) =>
                deltaVEngineInfo.GetSituationThrustVector(situation);

            [KSField(Description = "Number of the stage when engine is supposed to start")]
            public long StartBurnStage => deltaVEngineInfo.StartBurnStage;

            [KSField]
            public ModuleEngineAdapter EngineModule => new ModuleEngineAdapter(deltaVEngineInfo.Part as PartComponent, deltaVEngineInfo.Engine);
        }
    }
}
