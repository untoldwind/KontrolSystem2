using System.Collections.Generic;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using KSP.Sim.ResourceSystem;

namespace KontrolSystem.KSP.Runtime.KSPResource {
    [KSModule("ksp::resource",
        Description = "Collection of types and functions to get information and manipulate in-game resources."
    )]
    public partial class KSPResourceModule {
        [KSFunction]
        public static ResourceTransfer CreateResourceTransfer() {
            var resourceTransfer = new ResourceTransfer();
            KSPContext.CurrentContext.AddResourceTransfer(resourceTransfer);
            return resourceTransfer;
        }

        public static (IEnumerable<RealizedType>, IEnumerable<IKontrolConstant>) DirectBindings() {
            var (enumTypes, enumConstants) = BindingGenerator.RegisterEnumTypeMappings("ksp::resource",
                new[] {
                    ("FlowDirection", "Resource flow direction", typeof(FlowDirection)),
                });

            return (enumTypes, enumConstants);
        }

    }
}
