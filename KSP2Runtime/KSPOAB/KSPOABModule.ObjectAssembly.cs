using System.Linq;
using KontrolSystem.TO2.Binding;
using KSP.OAB;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssembly", Description = "Represents an object assembly, i.e. a potential vessel.")]
    public class ObjectAssemblyAdapter {
        private readonly IObjectAssembly objectAssembly;

        public ObjectAssemblyAdapter(IObjectAssembly objectAssembly) {
            this.objectAssembly = objectAssembly;
        }

        [KSField]
        public ObjectAssemblyPartAdapter[] Parts =>
            objectAssembly.Parts.Select(part => new ObjectAssemblyPartAdapter(part)).ToArray();

        [KSField] public double DryMass => objectAssembly.GetDryMass();

        [KSField] public double WetMass => objectAssembly.GetWetMass();

        [KSField] public double TotalMass => objectAssembly.GetTotalMass();

        [KSField]
        public ObjectAssemblyDeltaVAdapter DeltaV => new ObjectAssemblyDeltaVAdapter(objectAssembly.VesselDeltaV);
    }
}
