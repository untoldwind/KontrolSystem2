using System.Diagnostics.CodeAnalysis;
using KSP.Modules;
using KSP.Sim.DeltaV;
using KSP.Sim.impl;

namespace KontrolSystem.KSP2.Runtime.KSPVessel;

public static class IsPartExtensions {
    public static bool IsPartDeployable(this IDeltaVPart? PartRef, [MaybeNullWhen(false)] out Data_Deployable deployable) {
        if (PartRef == null) {
            deployable = null;
            return false;
        }

        return
            PartRef.TryGetModuleData<PartComponentModule_Deployable, Data_Deployable>(
                out deployable) // this doesn't work on PartComponentModule_SolarPanel, unexpectedly...
            || PartRef.TryGetModuleData<PartComponentModule_SolarPanel, Data_Deployable>(
                out deployable); // ...so we do a call for PartComponentModule_SolarPanel specifically
    }

    public static bool IsParachute(this IDeltaVPart? PartRef, [MaybeNullWhen(false)] out Data_Parachute parachute) {
        if (PartRef == null) {
            parachute = null;
            return false;
        }

        return PartRef.TryGetModuleData<PartComponentModule_Parachute, Data_Parachute>(out parachute);
    }
}
