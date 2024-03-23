using System.Linq;
using KontrolSystem.TO2.Binding;
using KSP.OAB;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssembly", Description = "Represents an object assembly, i.e. a potential vessel.")]
    public class ObjectAssemblyAdapter(IObjectAssembly objectAssembly) {
        private readonly IObjectAssembly objectAssembly = objectAssembly;

        [KSField(Description = "Get a list of all parts of assembly.")]
        public ObjectAssemblyPartAdapter[] Parts =>
            objectAssembly.Parts.Select(part => new ObjectAssemblyPartAdapter(part)).ToArray();

        [KSField(Description = "Total dry mass of assembly.")]
        public double DryMass => objectAssembly.GetDryMass();

        [KSField(Description = "Total wet mass of assembly.")]
        public double WetMass => objectAssembly.GetWetMass();

        [KSField(Description = "Total mass of assembly.")]
        public double TotalMass => objectAssembly.GetTotalMass();

        [KSField(Description = "Collection of methods to obtain delta-v information of the assembly.")]
        public ObjectAssemblyDeltaVAdapter DeltaV => new(objectAssembly.VesselDeltaV);
    }
}
