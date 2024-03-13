using System.Linq;
using KontrolSystem.KSP.Runtime.KSPResource;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseEngineAdapter<P, T>(P part, Data_Engine dataEngine) : BaseModuleAdapter<P, T>(part)
    where P : BasePartAdapter<T>
    where T : IDeltaVPart {
    protected readonly Data_Engine dataEngine = dataEngine;

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

    [KSField(Description = "Current thrust limit value between 0.0 - 1.0")]
    public double ThrustLimiter {
        get => dataEngine.thrustPercentage.GetValue() / 100.0;
        set => dataEngine.thrustPercentage.SetValue((float)(100.0 * value));
    }

    [KSField]
    public bool AutoSwitchMode {
        get => dataEngine.EngineAutoSwitchMode.GetValue();
        set => dataEngine.EngineAutoSwitchMode.SetValue(value);
    }

    [KSField(Description = "Toggle independent throttle")]
    public bool IndependentThrottleEnabled {
        get => dataEngine.IndependentThrottle.GetValue();
        set => dataEngine.IndependentThrottle.SetValue(value);
    }

    [KSField(Description = "Current independent throttle between 0.0 - 1.0")]
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


    [KSMethod(Description = "Calculate maximum thrust in atmosphere given atmospheric parameters")]
    public double CalcMaxThrustOutputAtm(double atmPressurekPa = 101.325, double atmTemp = 310.0,
        double atmDensity = 1.225000023841858, double machNumber = 0.0) =>
        dataEngine.MaxThrustOutputAtm(atmPressure: atmPressurekPa * 0.0098692326671601, atmTemp: atmTemp,
            atmDensity: atmDensity, machNumber: (float)machNumber);
}
