using KontrolSystem.TO2.Binding;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BasePartAdapter<T> where T : IDeltaVPart {
    protected readonly T part;

    protected BasePartAdapter(T part) {
        this.part = part;
    }

    [KSField] public string PartName => part.PartName;

    [KSField] public long ActivationStage => part.ActivationStage;

    [KSField] public long DecoupleStage => part.DecoupleStage;
    
    [KSField(Description = "Dry mass of the part")]
    public double DryMass => part.DryMass;

    [KSField]
    public double WetMass => part.WetMass;

    [KSField(Description = "Green mass (Kerbals) of the part")]
    public double GreenMass => part.GreenMass;

    [KSField(Description = "Total mass of the part")]
    public double TotalMass => part.TotalMass;

}
