using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Targetable")]
        public interface IKSPTargetable {
            [KSField] string Name { get; }
            
            [KSField] KSPOrbitModule.IOrbit Orbit { get; }
        }
    }
}
