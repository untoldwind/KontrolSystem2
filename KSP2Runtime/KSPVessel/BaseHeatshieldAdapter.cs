﻿using System.Linq;
using KontrolSystem.KSP.Runtime.KSPResource;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseHeatshieldAdapter<P, T>(P part, Data_Heatshield dataHeatshield)
    : BaseModuleAdapter<P, T>(part)
    where P : BasePartAdapter<T>
    where T : IDeltaVPart {
    protected readonly Data_Heatshield dataHeatshield = dataHeatshield;

    [KSField] public bool IsDeployed => dataHeatshield.IsDeployed;

    [KSField] public bool IsAblating => dataHeatshield.IsAblating;

    [KSField] public bool IsAblatorExhausted => dataHeatshield.IsAblatorExhausted;

    [KSField] public double AblatorRatio => dataHeatshield.AblatorRatio;

    [KSField]
    public KSPResourceModule.ResourceSettingAdapter[] RequiredResources =>
        dataHeatshield.requiredResources.Select(settings => new KSPResourceModule.ResourceSettingAdapter(settings)).ToArray();

}
