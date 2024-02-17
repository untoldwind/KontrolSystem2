using System;
using System.Collections.Generic;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using KSP.Game.Science;
using KSP.Modules;

namespace KontrolSystem.KSP.Runtime.KSPScience;

[KSModule("ksp::science",
    Description = "Collection of types and functions to get information and manipulate in-game science experiments."
)]
public partial class KSPScienceModule {
    public static (IEnumerable<RealizedType>, IEnumerable<IKontrolConstant>) DirectBindings() {
        var (enumTypes, enumConstants) = BindingGenerator.RegisterEnumTypeMappings("ksp::science",
            new[] {
                ("ScienceExperimentType", "Science experiment type", typeof(ScienceExperimentType),
                    new (Enum value, string description)[] {
                        (ScienceExperimentType.DataType, "Science experiment producing data"),
                        (ScienceExperimentType.SampleType, "Science experiment producing sample"),
                        (ScienceExperimentType.Both, "Science experiment producing both sample and data")
                    }),
                ("ExperimentState", "Science experiment state", typeof(ExperimentState),
                    new (Enum value, string description)[] {
                        (ExperimentState.NONE, "Unknown state"),
                        (ExperimentState.INVALIDLOCATION, "Location not valid"),
                        (ExperimentState.READY, "Experiment is ready to run"),
                        (ExperimentState.RUNNING, "Experiment is running"),
                        (ExperimentState.PAUSED, "Experiment is paused"),
                        (ExperimentState.OUTOFRESOURCE, "Experiment ran out of resources"),
                        (ExperimentState.LOCATIONCHANGED, "Experiment location changed"),
                        (ExperimentState.INSUFFICIENTCREW, "Experiment requires more available crew members"),
                        (ExperimentState.NOCONTROL, "Experiment requires control of the vessel"),
                        (ExperimentState.INSUFFICIENTSTORAGE, "Not enough storage capacity for experiment"),
                        (ExperimentState.ALREADYSTORED, "Experiment has already stored results"),
                        (ExperimentState.BLOCKED, "Experiment is blocked")
                    }),
                ("ScienceSituation", "Situation of a science experiment", typeof(ScienceSitutation),
                    new (Enum value, string description)[] {
                        (ScienceSitutation.None, "No specific situation required"),
                        (ScienceSitutation.HighOrbit, "Experiment in high orbit"),
                        (ScienceSitutation.LowOrbit, "Experiment in low orbit"),
                        (ScienceSitutation.Atmosphere, "Experiment inside an atmosphere"),
                        (ScienceSitutation.Splashed, "Experiment while splashed"),
                        (ScienceSitutation.Landed, "Experiment while landed")
                    })
            });

        return (enumTypes, enumConstants);
    }
}
