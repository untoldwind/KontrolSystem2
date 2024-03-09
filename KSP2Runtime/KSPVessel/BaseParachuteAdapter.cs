using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseParachuteAdapter<P, T>(P part, Data_Parachute dataParachute) : BaseModuleAdapter<P, T>(part)
    where P : BasePartAdapter<T>
    where T : IDeltaVPart {
    protected readonly Data_Parachute dataParachute = dataParachute;

    [KSField] public Data_Parachute.DeploymentStates DeployState => dataParachute.deployState.GetValue();

    [KSField]
    public Data_Parachute.DeploymentSafeStates ChuteSafety => dataParachute.deploymentSafetyState.GetValue();

    [KSField]
    public Data_Parachute.DeployMode DeployMode {
        get => dataParachute.DeploymentMode.GetValue();
        set => dataParachute.DeploymentMode.SetValue(value);
    }

    [KSField]
    public double MinAirPressure {
        get => dataParachute.minAirPressureToOpen.GetValue();
        set => dataParachute.minAirPressureToOpen.SetValue(Mathf.Clamp((float)value, 0.01f, 0.75f));
    }

    [KSField]
    public double DeployAltitude {
        get => dataParachute.deployAltitude.GetValue();
        set => dataParachute.deployAltitude.SetValue(Mathf.Clamp((float)value, 50f, 5000f));
    }

    [KSField]
    public bool Armed {
        get => dataParachute.armedToggle.GetValue();
        set => dataParachute.armedToggle.SetValue(value);
    }
}
