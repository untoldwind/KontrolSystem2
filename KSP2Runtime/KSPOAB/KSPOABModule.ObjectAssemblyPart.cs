using KontrolSystem.TO2.Binding;
using KSP.OAB;
using UniLinq;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyPart", Description = "Represents are part in an object assembly.")]
    public class ObjectAssemblyPartAdapter {
        private readonly IObjectAssemblyPart objectAssemblyPart;

        public ObjectAssemblyPartAdapter(IObjectAssemblyPart objectAssemblyPart) {
            this.objectAssemblyPart = objectAssemblyPart;
            foreach (var k in objectAssemblyPart.Modules.Keys) {
                UnityEngine.Debug.Log(k.ToString());

            }
        }

        [KSField] public string PartName => objectAssemblyPart.PartName;

        [KSField(Description = "Dry mass of the part")]
        public double DryMass => objectAssemblyPart.DryMass;

        [KSField]
        public double WetMass => objectAssemblyPart.WetMass;

        [KSField(Description = "Green mass (Kerbals) of the part")]
        public double GreenMass => objectAssemblyPart.GreenMass;

        [KSField(Description = "Total mass of the part")]
        public double TotalMass => objectAssemblyPart.TotalMass;

        [KSField] public bool FuelCrossFeed => objectAssemblyPart.FuelCrossFeed;

        [KSField]
        public Vector3d RelativePosition =>
            objectAssemblyPart.HasParent() ? objectAssemblyPart.AssemblyRelativePosition : Vector3.zero;
    }
}
