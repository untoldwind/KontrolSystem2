using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Modules;
using KSP.Sim;
using KSP.Sim.impl;
using UniLinq;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ScienceStorage",
        Description = "Represents the science storage / research inventory of a vessel.")]
    public class ScienceStorageAdapter(VesselAdapter vessel, ScienceStorageComponent scienceStorageComponent) {
        [KSField] public bool IsActive => scienceStorageComponent.IsActive;

        [KSField]
        public Option<ModuleTransmitterAdapter> ActiveTransmitter {
            get {
                var activeTransmitter = scienceStorageComponent.ActiveTransmitter;
                if (activeTransmitter == null) return Option.None<ModuleTransmitterAdapter>();
                if (activeTransmitter.DataModules.TryGetByType<Data_Transmitter>(out var data)) {
                    return Option.Some(new ModuleTransmitterAdapter(new PartAdapter(vessel, activeTransmitter.Part), data));
                }
                return Option.None<ModuleTransmitterAdapter>();
            }
        }

        [KSField]
        public KSPScience.KSPScienceModule.ResearchReportAdapter[] ResearchReports =>
            scienceStorageComponent.GetStoredResearchReports()
                .Select(report => new KSPScience.KSPScienceModule.ResearchReportAdapter(scienceStorageComponent, report))
                .ToArray();

        [KSMethod]
        public bool StartTransmitAll() {
            return scienceStorageComponent.StartReportTransmissionAll();
        }
    }
}
