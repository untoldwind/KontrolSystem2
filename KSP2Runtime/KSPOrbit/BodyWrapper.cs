using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    public class BodyWrapper : KSPOrbitModule.IBody, KSPVesselModule.IKSPTargetable {
        private readonly CelestialBodyComponent body;

        public BodyWrapper(CelestialBodyComponent body) => this.body = body;
        
        public string Name => body.Name;
        
        [KSField] public KSPOrbitModule.IOrbit Orbit => new OrbitWrapper(body.Orbit);

    }
}
