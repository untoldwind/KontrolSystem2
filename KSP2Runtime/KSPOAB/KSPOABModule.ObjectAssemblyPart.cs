using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.KSP2.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Modules;
using KSP.OAB;
using KSP.Sim.DeltaV;
using KSP.Sim.impl;
using UniLinq;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyPart", Description = "Represents are part in an object assembly.")]
    public class ObjectAssemblyPartAdapter : BasePartAdapter<IObjectAssemblyPart> {
        public ObjectAssemblyPartAdapter(IObjectAssemblyPart objectAssemblyPart) : base(objectAssemblyPart) {
        }

        [KSField] public PartCategories PartCategory => part.Category;
        
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

        [KSField]
        public Option<ObjectAssemblyAirIntakeAdapter> AirIntake =>
            part.IsPartAirIntake(out var data)
                ? Option.Some(new ObjectAssemblyAirIntakeAdapter(this, data))
                : Option.None<ObjectAssemblyAirIntakeAdapter>();

        [KSField]
        public Option<ObjectAssemblyDockingNodeAdapter> DockingNode =>
            part.IsPartDockingPort(out var data)
                ? Option.Some(new ObjectAssemblyDockingNodeAdapter(this, data))
                : Option.None<ObjectAssemblyDockingNodeAdapter>();


        [KSField] public bool IsDeployable => part.IsPartDeployable(out _);

        [KSField]
        public Option<ObjectAssemblyDeployableAdapter> Deployable =>
            part.IsPartDeployable(out var data)
                ? Option.Some(new ObjectAssemblyDeployableAdapter(this, data))
                : Option.None<ObjectAssemblyDeployableAdapter>();

        [KSField] public bool IsDecoupler => part.IsPartDecoupler(out _);

        [KSField]
        public Option<ObjectAssemblyDecouplerAdapter> Decoupler =>
            part.IsPartDecoupler(out var data)
                ? Option.Some(new ObjectAssemblyDecouplerAdapter(this, data))
                : Option.None<ObjectAssemblyDecouplerAdapter>();

        [KSField]
        public bool IsTransmitter =>
            part.TryGetModuleData<PartComponentModule_DataTransmitter, Data_Transmitter>(out _);

        [KSField]
        public Option<ObjectAssemblyTransmitterAdapter> Transmitter =>
            part.TryGetModuleData<PartComponentModule_DataTransmitter, Data_Transmitter>(out var data)
                ? Option.Some(new ObjectAssemblyTransmitterAdapter(this, data))
                : Option.None<ObjectAssemblyTransmitterAdapter>();

        [KSField]
        public bool IsLight =>
            part.TryGetModuleData<PartComponentModule_Light, Data_Light>(out _);

        [KSField]
        public Option<ObjectAssemblyLightAdapter> Light =>
            part.TryGetModuleData<PartComponentModule_Light, Data_Light>(out var data)
                ? Option.Some(new ObjectAssemblyLightAdapter(this, data))
                : Option.None<ObjectAssemblyLightAdapter>();

        [KSField]
        public bool IsGenerator =>
            part.TryGetModuleData<PartComponentModule_Generator, Data_ModuleGenerator>(out _);

        [KSField]
        public Option<ObjectAssemblyGeneratorAdapter> Generator =>
            part.TryGetModuleData<PartComponentModule_Generator, Data_ModuleGenerator>(out var data)
                ? Option.Some(new ObjectAssemblyGeneratorAdapter(this, data))
                : Option.None<ObjectAssemblyGeneratorAdapter>();

        [KSField]
        public Option<ObjectAssemblyCommandAdapter> CommandModule =>
            part.TryGetModuleData<PartComponentModule_Command, Data_Command>(out var data)
                ? Option.Some(new ObjectAssemblyCommandAdapter(this, data))
                : Option.None<ObjectAssemblyCommandAdapter>();
    }
}
