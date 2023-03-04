using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    public class OrbitWrapper : KSPOrbitModule.IOrbit {
        private readonly PatchedConicsOrbit orbit;

        public OrbitWrapper(PatchedConicsOrbit orbit) => this.orbit = orbit;

        public KSPOrbitModule.IBody ReferenceBody => new BodyWrapper(orbit.referenceBody);

        public double Apoapsis => orbit.ApoapsisArl;
        
        public double Periapsis => orbit.PeriapsisArl;
        
        public double ApoapsisRadius => orbit.Apoapsis;
        
        public double PeriapsisRadius => orbit.Periapsis;
        
        public double SemiMajorAxis => orbit.SemiMinorAxis;
        
        public double Inclination => orbit.inclination;
        
        public double Eccentricity => orbit.eccentricity;
        
        public double Lan => orbit.longitudeOfAscendingNode;
        
        public double Epoch => orbit.epoch;
        
        public double ArgumentOfPeriapsis => orbit.argumentOfPeriapsis;
        
        public double MeanAnomalyAtEpoch => orbit.meanAnomalyAtEpoch;
        
        public double MeanMotion => orbit.meanMotion;
        
        public double Period => orbit.period;
    }
}
