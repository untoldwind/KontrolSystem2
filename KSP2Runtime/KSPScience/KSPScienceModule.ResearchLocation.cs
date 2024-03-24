using KontrolSystem.TO2.Binding;
using KSP.Game.Science;

namespace KontrolSystem.KSP.Runtime.KSPScience;

public partial class KSPScienceModule {
    [KSClass("ResearchLocation",
        Description = "Represents a research location of a science experiment.")]
    public class ResearchLocationAdapter(ResearchLocation researchLocation) {
        private readonly ResearchLocation researchLocation = researchLocation;

        [KSField] public string Id => researchLocation.ResearchLocationId;

        [KSField] public bool RequiresRegion => researchLocation.RequiresRegion;

        [KSField] public string BodyName => researchLocation.BodyName;

        [KSField] public string ScienceRegion => researchLocation.ScienceRegion;

        [KSField] public ScienceSitutation ScienceSituation => researchLocation.ScienceSituation;
    }
}
