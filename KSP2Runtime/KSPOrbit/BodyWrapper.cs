using KSP.Sim.Definitions;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    public class BodyWrapper : KSPOrbitModule.IBody {
        private readonly CelestialBodyCore body;

        public BodyWrapper(CelestialBodyCore body) => this.body = body;
        
        public string Name => body.data.bodyName;
    }
}
