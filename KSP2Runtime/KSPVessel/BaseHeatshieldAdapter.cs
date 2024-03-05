using System.Linq;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseHeatshieldAdapter<P, T> : BaseModuleAdapter<P, T> where P : BasePartAdapter<T> where T : IDeltaVPart {
    protected readonly Data_Heatshield dataHeatshield;

    protected BaseHeatshieldAdapter(P part, Data_Heatshield dataHeatshield) : base(part) {
        this.dataHeatshield = dataHeatshield;
    }

    [KSField] public bool IsDeployed => dataHeatshield.IsDeployed;

    [KSField] public bool IsAblating => dataHeatshield.IsAblating;

    [KSField] public bool IsAblatorExhausted => dataHeatshield.IsAblatorExhausted;

    [KSField] public double AblatorRatio => dataHeatshield.AblatorRatio;

    [KSField]
    public KSPVesselModule.ResourceSettingAdapter[] RequiredResources =>
        dataHeatshield.requiredResources.Select(settings => new KSPVesselModule.ResourceSettingAdapter(settings)).ToArray();

}
