﻿using KontrolSystem.KSP.Runtime.KSPResource;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseSolarPanelAdapter<P, T>(P part, Data_SolarPanel dataSolarPanel)
    : BaseModuleAdapter<P, T>(part)
    where P : BasePartAdapter<T>
    where T : IDeltaVPart {
    protected readonly Data_SolarPanel dataSolarPanel = dataSolarPanel;

    [KSField] public double EfficiencyMultiplier => dataSolarPanel.EfficiencyMultiplier;

    [KSField(Description = "Base flow rate")]
    public double BaseFlowRate => dataSolarPanel.ResourceSettings.Rate;

    [KSField]
    public KSPResourceModule.ResourceSettingAdapter ResourceSetting => new(dataSolarPanel.ResourceSettings);
}
