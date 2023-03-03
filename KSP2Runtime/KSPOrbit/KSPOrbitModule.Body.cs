using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    public partial class KSPOrbitModule {
        [KSClass("Body", Description = "Represents an in-game celestial body.")]
        public interface IBody {
            [KSField(Description = "Name of the celestial body.")]
            string Name { get; }
        }
    }
}
