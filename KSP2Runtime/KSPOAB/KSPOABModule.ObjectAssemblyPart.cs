using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.OAB;
using KSP.Sim.DeltaV;
using UniLinq;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyPart", Description = "Represents are part in an object assembly.")]
    public class ObjectAssemblyPartAdapter : BasePartAdapter<IObjectAssemblyPart> {
        public ObjectAssemblyPartAdapter(IObjectAssemblyPart objectAssemblyPart) : base(objectAssemblyPart) {
        }

        [KSField] public bool FuelCrossFeed => part.FuelCrossFeed;

        [KSField]
        public Vector3d RelativePosition =>
            part.HasParent() ? part.AssemblyRelativePosition : Vector3.zero;

        [KSField] public bool IsEngine => part.IsPartEngine(out var _);

        [KSField]
        public Option<ObjectAssemblyEngineAdapter> Engine =>
            part.IsPartEngine(out var data)
                ? Option.Some(new ObjectAssemblyEngineAdapter(this, data))
                : Option.None<ObjectAssemblyEngineAdapter>();

        [KSField] public bool IsSolarPanel => part.IsPartSolarPanel(out var _);

        [KSField]
        public Option<ObjectAssemblySolarPanelAdapter> SolarPanel =>
            part.IsPartSolarPanel(out var data)
                ? Option.Some(new ObjectAssemblySolarPanelAdapter(this, data))
                : Option.None<ObjectAssemblySolarPanelAdapter>();
    }
}
