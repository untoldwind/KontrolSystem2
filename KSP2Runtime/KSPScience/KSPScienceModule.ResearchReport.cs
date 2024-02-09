using KontrolSystem.TO2.Binding;
using KSP.Game.Science;

namespace KontrolSystem.KSP.Runtime.KSPScience {
    public partial class KSPScienceModule {
        [KSClass("ResearchReport",
            Description = "Represents the stored report of a science experiment")]
        public class ResearchReportAdapter {
            private readonly ResearchReport researchReport;

            public ResearchReportAdapter(ResearchReport researchReport) {
                this.researchReport = researchReport;
            }

            [KSField]
            public ResearchLocationAdapter ResearchLocation => new ResearchLocationAdapter(researchReport.Location);

            [KSField]
            public bool TransmissionStatus => researchReport.TransmissionStatus;

            [KSField]
            public double TransmissionPercentage => researchReport.TransmissionPercentage;

            [KSField]
            public double EcRequired => researchReport.EcRequired;

            [KSField]
            public double TimeRequired => researchReport.TimeRequired;

            [KSField]
            public double TransmissionSize => researchReport.TransmissionSize;
        }
    }
}
