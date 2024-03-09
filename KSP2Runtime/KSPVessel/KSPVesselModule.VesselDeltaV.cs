﻿using System.Linq;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("VesselDeltaV")]
    public class VesselDeltaVAdapter(KSPVesselModule.VesselAdapter vesselAdapter) {
        private readonly VesselDeltaVComponent deltaV = vesselAdapter.vessel.VesselDeltaV;
        private readonly VesselAdapter vesselAdapter = vesselAdapter;

        [KSField]
        public DeltaVStageInfoAdapter[] Stages => deltaV.StageInfo
            .Select(stage => new DeltaVStageInfoAdapter(vesselAdapter, stage)).ToArray();

        [KSMethod(Description = "Get delta-v information for a specific `stage` of the vessel, if existent.")]
        public Option<DeltaVStageInfoAdapter> Stage(long stage) {
            var stageInfo = deltaV.GetStage((int)stage);

            return stageInfo != null
                ? new Option<DeltaVStageInfoAdapter>(new DeltaVStageInfoAdapter(vesselAdapter, stageInfo))
                : new Option<DeltaVStageInfoAdapter>();
        }
    }
}
