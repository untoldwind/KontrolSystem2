using KontrolSystem.TO2.Runtime;
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
        public Vector3d OrbitalVelocity(double ut) {
            OrbitUtilities.GetLocalStateVector(ut, orbit, out Vector3d localPosition, out Vector3d localVelocity);

            return localVelocity;
        }

        public Vector3d RelativePosition(double ut) {
            OrbitUtilities.GetLocalStateVector(ut, orbit, out Vector3d localPosition, out Vector3d localVelocity);

            return localPosition;
        }

        public Vector3d Prograde(double ut) => OrbitalVelocity(ut).normalized;

        public Vector3d RadialPlus(double ut) => Vector3d.Exclude(Prograde(ut), Up(ut)).normalized;

        public Vector3d Up(double ut) => RelativePosition(ut).normalized;

        public double Radius(double ut) => RelativePosition(ut).magnitude;

        public Vector3d Horizontal(double ut) => Vector3d.Exclude(Up(ut), Prograde(ut)).normalized;


        public double NextPeriapsisTime(Option<double> ut = new Option<double>()) {
            throw new System.NotImplementedException();
        }

        public Result<double, string> NextApoapsisTime(Option<double> ut = new Option<double>()) {
            throw new System.NotImplementedException();
        }
        
        
    }
}
