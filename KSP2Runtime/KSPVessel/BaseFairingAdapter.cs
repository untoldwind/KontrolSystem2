﻿using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseFairingAdapter<P, T>(P part, Data_Fairing dataFairing) : BaseModuleAdapter<P, T>(part)
    where P : BasePartAdapter<T>
    where T : IDeltaVPart {
    protected readonly Data_Fairing dataFairing = dataFairing;

    [KSField]
    public double EjectionForce {
        get => dataFairing.EjectionForce.GetValue();
        set => dataFairing.EjectionForce.SetValue((float)value);
    }

    [KSField] public bool IsJettisoned => dataFairing.IsDeployed.GetValue();

    [KSField]
    public bool Enabled {
        get => dataFairing.FairingEnabled.GetValue();
        set => dataFairing.FairingEnabled.SetObject(value);
    }

}
