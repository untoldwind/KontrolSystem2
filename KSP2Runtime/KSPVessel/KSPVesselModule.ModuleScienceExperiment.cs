using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;
using static KontrolSystem.KSP.Runtime.KSPOrbit.KSPOrbitModule;
using KontrolSystem.TO2.Runtime;
using System;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("ModuleScienceExperiment")]
        public class ModuleScienceExperimentPanelAdapter {
            private readonly PartComponent part;
            private readonly Data_ScienceExperiment dataScienceExperiment;

            public ModuleScienceExperimentPanelAdapter(PartComponent part, Data_ScienceExperiment dataScienceExperiment) {
                this.part = part;
                this.dataScienceExperiment = dataScienceExperiment;
            }
            
            [KSField] public string PartName => part?.PartName ?? "Unknown";

            [KSField] public bool IsDeployed => dataScienceExperiment.PartIsDeployed;
        }
    }
}
