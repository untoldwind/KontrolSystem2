using KontrolSystem.TO2.Binding;
using KSP.Modules;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseSolarPanelAdapter {
    protected readonly Data_SolarPanel dataSolarPanel;

    protected BaseSolarPanelAdapter(Data_SolarPanel dataSolarPanel) {
        this.dataSolarPanel = dataSolarPanel;
    }

    [KSField] public double EfficiencyMultiplier => dataSolarPanel.EfficiencyMultiplier;

    [KSField(Description = "Base flow rate")]
    public double BaseFlowRate => dataSolarPanel.ResourceSettings.Rate;
}
