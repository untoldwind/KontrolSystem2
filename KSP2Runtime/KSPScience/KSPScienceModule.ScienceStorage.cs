using KontrolSystem.TO2.Binding;
using KSP.Sim.impl;
using UniLinq;

namespace KontrolSystem.KSP.Runtime.KSPScience;

public partial class KSPScienceModule {
    [KSClass("ScienceStorage",
        Description = "Represents the science storage / research inventory of a vessel.")]
    public class ScienceStorageAdapter {
        private readonly ScienceStorageComponent scienceStorageComponent;

        public ScienceStorageAdapter(ScienceStorageComponent scienceStorageComponent) {
            this.scienceStorageComponent = scienceStorageComponent;
        }

        [KSField] public bool IsActive => scienceStorageComponent.IsActive;

        [KSField]
        public ResearchReportAdapter[] ResearchReports =>
            scienceStorageComponent.GetStoredResearchReports()
                .Select(report => new ResearchReportAdapter(scienceStorageComponent, report))
                .ToArray();

        [KSMethod]
        public bool StartTransmitAll() {
            return scienceStorageComponent.StartReportTransmissionAll();
        }
    }
}
