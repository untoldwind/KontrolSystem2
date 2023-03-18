﻿using System;
using KontrolSystem.TO2.Runtime;
using KSP.Api;
using KSP.Sim;
using KSP.Sim.impl;
using Result = KontrolSystem.TO2.Runtime.Result;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    public class OrbitWrapper : KSPOrbitModule.IOrbit {
        private readonly IKSPContext context;
        private readonly PatchedConicsOrbit orbit;

        public OrbitWrapper(IKSPContext context, PatchedConicsOrbit orbit) {
            this.context = context;
            this.orbit = orbit;
        }

        public KSPOrbitModule.IBody ReferenceBody => new BodyWrapper(context, orbit.referenceBody);

        public double StartUt => orbit.StartUT;

        public double EndUt => orbit.EndUT;

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

        public ITransformFrame ReferenceFrame => orbit.ReferenceFrame;

        public Vector3d OrbitNormal => orbit.GetRelativeOrbitNormal().SwapYAndZ;

        public Vector3d RelativePosition(double ut) => orbit.GetRelativePositionAtUTZup(ut).SwapYAndZ;

        public Position Position(double ut) => new Position(ReferenceFrame, RelativePosition(ut));

        public Vector Velocity(double ut) => new Vector(ReferenceFrame, orbit.GetOrbitalVelocityAtUTZup(ut).SwapYAndZ);

        public Vector3d OrbitalVelocity(double ut) => orbit.GetOrbitalVelocityAtUTZup(ut).SwapYAndZ;

        public Vector3d Prograde(double ut) => OrbitalVelocity(ut).normalized;

        public Vector3d NormalPlus(double ut) => orbit.GetRelativeOrbitNormal().SwapYAndZ.normalized;

        public Vector3d RadialPlus(double ut) => Vector3d.Exclude(Prograde(ut), Up(ut)).normalized;

        public Vector3d Up(double ut) => RelativePosition(ut).normalized;

        public double Radius(double ut) => RelativePosition(ut).magnitude;

        public Vector3d Horizontal(double ut) => Vector3d.Exclude(Up(ut), Prograde(ut)).normalized;

        public KSPOrbitModule.IOrbit PerturbedOrbit(double ut, Vector3d dV) =>
            ReferenceBody.CreateOrbit(RelativePosition(ut), OrbitalVelocity(ut) + dV, ut);

        public double MeanAnomalyAtUt(double ut) {
            double ret = (orbit.ObTAtEpoch + (ut - orbit.epoch)) * orbit.meanMotion;

            return orbit.eccentricity < 1 ? DirectBindingMath.ClampRadians2Pi(ret) : ret;
        }

        public double UTAtMeanAnomaly(double meanAnomaly, double ut) {
            double currentMeanAnomaly = MeanAnomalyAtUt(ut);
            double meanDifference = meanAnomaly - currentMeanAnomaly;
            if (orbit.eccentricity < 1) meanDifference = DirectBindingMath.ClampRadians2Pi(meanDifference);
            return ut + meanDifference / MeanMotion;
        }

        public double GetMeanAnomalyAtEccentricAnomaly(double ecc) => orbit.GetMeanAnomaly(ecc);

        public double GetEccentricAnomalyAtTrueAnomaly(double trueAnomaly) => orbit.GetEccentricAnomaly(trueAnomaly);

        public double TimeOfTrueAnomaly(double trueAnomaly, double ut) {
            return UTAtMeanAnomaly(GetMeanAnomalyAtEccentricAnomaly(GetEccentricAnomalyAtTrueAnomaly(trueAnomaly)), ut);
        }


        public double NextPeriapsisTime(Option<double> maybeUt = new Option<double>()) {
            double ut = maybeUt.GetValueOrDefault(context.UniversalTime);
            if (orbit.eccentricity < 1) {
                return TimeOfTrueAnomaly(0, ut);
            } else {
                return ut - MeanAnomalyAtUt(ut) / MeanMotion;
            }
        }

        public Result<double, string> NextApoapsisTime(Option<double> maybeUt = new Option<double>()) {
            double ut = maybeUt.GetValueOrDefault(context.UniversalTime);
            if (orbit.eccentricity < 1) {
                return Result.Ok<double, string>(TimeOfTrueAnomaly(Math.PI, ut));
            } else {
                return Result.Err<double, string>("OrbitExtensions.NextApoapsisTime cannot be called on hyperbolic orbits");
            }
        }

        public double TrueAnomalyAtRadius(double radius) => orbit.TrueAnomalyAtRadius(radius);

        public Result<double, string> NextTimeOfRadius(double ut, double radius) {
            if (radius < orbit.Periapsis || (orbit.eccentricity < 1 && radius > orbit.Apoapsis))
                Result.Err<double, string>("OrbitExtensions.NextTimeOfRadius: given radius of " + radius +
                                           " is never achieved: PeR = " + orbit.Periapsis + " and ApR = " + orbit.Apoapsis);

            double trueAnomaly1 = orbit.TrueAnomalyAtRadius(radius);
            double trueAnomaly2 = 2 * Math.PI - trueAnomaly1;
            double time1 = TimeOfTrueAnomaly(trueAnomaly1, ut);
            double time2 = TimeOfTrueAnomaly(trueAnomaly2, ut);
            if (time2 < time1 && time2 > ut) return Result.Ok<double, string>(time2);
            else return Result.Ok<double, string>(time1);
        }

        public Vector3d RelativePositionApoapsis {
            get {
                Vector3d vectorToAn = QuaternionD.AngleAxis(-orbit.longitudeOfAscendingNode, Vector3d.up) * Vector3d.right;
                Vector3d vectorToPe = QuaternionD.AngleAxis((float)orbit.argumentOfPeriapsis, OrbitNormal) * vectorToAn;
                return -ApoapsisRadius * vectorToPe;
            }
        }

        public Vector3d RelativePositionPeriapsis {
            get {
                Vector3d vectorToAn = QuaternionD.AngleAxis(-orbit.longitudeOfAscendingNode, Vector3d.up) * Vector3d.right;
                Vector3d vectorToPe = QuaternionD.AngleAxis(orbit.argumentOfPeriapsis, OrbitNormal) * vectorToAn;
                return PeriapsisRadius * vectorToPe;
            }
        }
        public double TrueAnomalyFromVector(Vector3d vec) {
            Vector3d oNormal = OrbitNormal;
            Vector3d projected = Vector3d.Exclude(oNormal, vec);
            Vector3d vectorToPe = RelativePositionPeriapsis;
            double angleFromPe = Vector3d.Angle(vectorToPe, projected);

            //If the vector points to the infalling part of the orbit then we need to do 360 minus the
            //angle from Pe to get the true anomaly. Test this by taking the the cross product of the
            //orbit normal and vector to the periapsis. This gives a vector that points to center of the
            //outgoing side of the orbit. If vectorToAN is more than 90 degrees from this vector, it occurs
            //during the infalling part of the orbit.
            if (Math.Abs(Vector3d.Angle(projected, Vector3d.Cross(oNormal, vectorToPe))) < 90) {
                return angleFromPe * DirectBindingMath.DegToRad;
            } else {
                return (360 - angleFromPe) * DirectBindingMath.DegToRad;
            }
        }

        public double SynodicPeriod(KSPOrbitModule.IOrbit other) {
            int sign = (Vector3d.Dot(OrbitNormal, other.OrbitNormal) > 0 ? 1 : -1); //detect relative retrograde motion
            return Math.Abs(1.0 /
                            (1.0 / Period - sign * 1.0 / other.Period)); //period after which the phase angle repeats
        }

        public string ToString() => KSPOrbitModule.OrbitToString(this);

        public string ToFixed(long decimals) => KSPOrbitModule.OrbitToFixed(this, decimals);
    }
}
