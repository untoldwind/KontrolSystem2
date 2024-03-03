using System.Linq;
using KontrolSystem.KSP.Runtime.KSPResource;
using KontrolSystem.TO2.Binding;
using KSP.Modules;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseEngineAdapter {
    protected readonly Data_Engine dataEngine;

    protected BaseEngineAdapter(Data_Engine dataEngine) {
        this.dataEngine = dataEngine;
    }

    [KSField(Description = "Check if engine is shutdown")]
    public bool IsShutdown => dataEngine.EngineShutdown;

    [KSField(Description = "Check if engine has ignited")]
    public bool HasIgnited => dataEngine.EngineIgnited;

    [KSField(Description = "Check if engine had a flame-out")]
    public bool IsFlameout => dataEngine.Flameout;

    [KSField] public bool IsStaged => dataEngine.staged;

    [KSField(Description = "Check if engine is operational")]
    public bool IsOperational => dataEngine.IsOperational;

    [KSField] public bool IsPropellantStarved => dataEngine.IsPropellantStarved;

    [KSField] public double CurrentThrottle => dataEngine.currentThrottle;

    [KSField] public double CurrentThrust => dataEngine.FinalThrustValue;

    [KSField] public double RealIsp => dataEngine.RealISPValue;

    [KSField] public double ThrottleMin => dataEngine.ThrottleMin;

    [KSField] public double MinFuelFlow => dataEngine.MinFuelFlow;

    [KSField] public double MaxFuelFlow => dataEngine.MaxFuelFlow;

    [KSField] public double MaxThrustOutputVac => dataEngine.MaxThrustOutputVac();

    [KSField] public double MaxThrustOutputAtm => dataEngine.MaxThrustOutputAtm();

    [KSField]
    public double ThrustLimiter {
        get => dataEngine.thrustPercentage.GetValue() / 100.0;
        set => dataEngine.thrustPercentage.SetValue((float)(100.0 * value));
    }

    [KSField]
    public bool AutoSwitchMode {
        get => dataEngine.EngineAutoSwitchMode.GetValue();
        set => dataEngine.EngineAutoSwitchMode.SetValue(value);
    }

    [KSField]
    public bool IndependentThrottleEnabled {
        get => dataEngine.IndependentThrottle.GetValue();
        set => dataEngine.IndependentThrottle.SetValue(value);
    }

    [KSField]
    public double IndependentThrottle {
        get => dataEngine.IndependentThrottlePercentage.GetValue() / 100.0;
        set => dataEngine.IndependentThrottlePercentage.SetValue((float)(value * 100.0));
    }

    [KSField(Description = "Get all engine modes")]
    public KSPVesselModule.EngineModeAdapter[] EngineModes => dataEngine.engineModes
        .Select(engineMode => new KSPVesselModule.EngineModeAdapter(engineMode)).ToArray();

    [KSField(Description = "Get the current engine mode")]
    public KSPVesselModule.EngineModeAdapter CurrentEngineMode =>
        new(dataEngine.engineModes[dataEngine.currentEngineModeIndex]);

    [KSField(Description = "Get the propellant of the current engine mode")]
    public KSPResourceModule.ResourceDefinitionAdapter CurrentPropellant =>
        new(dataEngine.CurrentPropellantState.resourceDef);

    [KSField(Description = "Get the propellants of the different engine modes")]
    public KSPResourceModule.ResourceDefinitionAdapter[] Propellants =>
        dataEngine.PropellantStates
            .Select(state => new KSPResourceModule.ResourceDefinitionAdapter(state.resourceDef)).ToArray();

}
