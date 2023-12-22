using System;
using System.Collections.Generic;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using KSP.Game.Science;
using KSP.Sim.ResourceSystem;

namespace KontrolSystem.KSP.Runtime.KSPScience {
    [KSModule("ksp::science",
        Description = "Collection of types and functions to get information and manipulate in-game science experiments."
    )]
    public partial class KSPScienceModule {

        public static (IEnumerable<RealizedType>, IEnumerable<IKontrolConstant>) DirectBindings() {
            var (enumTypes, enumConstants) = BindingGenerator.RegisterEnumTypeMappings("ksp::resource",
                new[] {
                    ("FlowDirection", "Resource flow direction", typeof(FlowDirection), new (Enum value, string description)[] {
                        (ScienceExperimentType.DataType, "Science experiment producing data"),
                        (ScienceExperimentType.SampleType, "Science experiment producing sample"),
                        (ScienceExperimentType.Both, "Science experiment producing both sample and data"),
                    }),
                });

            return (enumTypes, enumConstants);
        }
    }
}
