using System;
using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Runtime;
using KSP.Sim;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.Testing;

public class MockOrbit : KSPOrbitModule.IOrbit {
    public const double DegToRad = Math.PI / 180.0;
    public const double RadToDeg = 180.0 / Math.PI;
    public readonly double argumentOfPeriapsis;

    public readonly MockBody body;
    public readonly double eccentricity;
    public readonly double epoch;
    public readonly double inclination;
    public readonly double lan;
    public readonly double meanAnomalyAtEpoch;
    public readonly double meanMotion;
    public readonly double orbitTimeAtEpoch;
    public readonly double period;
    public readonly double semiMajorAxis;
    public Vector3d ascendingNode;
    public Vector3d eccVec;
    public Vector3d frameX;
    public Vector3d frameY;
    public Vector3d frameZ;

    public MockOrbit(MockBody body,
        double inclination,
        double eccentricity,
        double semiMajorAxis,
        double lan,
        double argumentOfPeriapsis,
        double epoch,
        double meanAnomalyAtEpoch) {
        this.body = body;
        this.inclination = inclination;
        this.eccentricity = eccentricity;
        this.semiMajorAxis = semiMajorAxis;
        this.lan = lan;
        this.argumentOfPeriapsis = argumentOfPeriapsis;
        this.meanAnomalyAtEpoch = meanAnomalyAtEpoch;
        this.epoch = epoch;

        var anX = Math.Cos(lan * DegToRad);
        var anY = Math.Sin(lan * DegToRad);
        var incX = Math.Cos(inclination * DegToRad);
        var incY = Math.Sin(inclination * DegToRad);
        var peX = Math.Cos(argumentOfPeriapsis * DegToRad);
        var peY = Math.Sin(argumentOfPeriapsis * DegToRad);
        frameX = new Vector3d(anX * peX - anY * incX * peY, anY * peX + anX * incX * peY, incY * peY);
        frameY = new Vector3d(-anX * peY - anY * incX * peX, -anY * peY + anX * incX * peX, incY * peX);
        frameZ = new Vector3d(anY * incY, -anX * incY, incX);

        ascendingNode = Vector3d.Cross(Vector3d.forward, frameZ);
        if (ascendingNode.sqrMagnitude == 0.0) ascendingNode = Vector3d.right;

        eccVec = frameX * eccentricity;
        meanMotion = GetMeanMotion();
        orbitTimeAtEpoch = meanAnomalyAtEpoch / meanMotion;
        if (eccentricity < 1.0)
            period = 2 * Math.PI / meanMotion;
        else
            period = double.PositiveInfinity;
    }

    public MockOrbit(MockBody body, Vector3d position, Vector3d velocity, double ut) {
        this.body = body;

        var h = Vector3d.Cross(position, velocity);
        var orbitalEnergy = velocity.sqrMagnitude / 2.0 - body.mu / position.magnitude;

        if (h.sqrMagnitude == 0.0)
            ascendingNode = Vector3d.Cross(position, Vector3d.forward);
        else
            ascendingNode = Vector3d.Cross(Vector3d.forward, h);

        if (ascendingNode.sqrMagnitude == 0.0) ascendingNode = Vector3d.right;

        lan = RadToDeg * Math.Atan2(ascendingNode.y, ascendingNode.x);
        eccVec = Vector3d.Cross(velocity, h) / body.mu - position / position.magnitude;
        eccentricity = eccVec.magnitude;
        if (eccentricity < 1.0)
            semiMajorAxis = -body.mu / (2.0 * orbitalEnergy);
        else
            semiMajorAxis = -h.sqrMagnitude / body.mu / (eccVec.sqrMagnitude - 1.0);

        if (eccentricity == 0.0) {
            frameX = ascendingNode.normalized;
            argumentOfPeriapsis = 0.0;
        } else {
            frameX = eccVec.normalized;
            argumentOfPeriapsis =
                RadToDeg * Math.Acos(Vector3d.Dot(ascendingNode, frameX) / ascendingNode.magnitude);
            if (frameX.z < 0.0) argumentOfPeriapsis = 360.0 - argumentOfPeriapsis;
        }

        if (h.sqrMagnitude == 0.0) {
            frameY = ascendingNode.normalized;
            frameZ = Vector3d.Cross(frameX, frameY);
        } else {
            frameZ = h.normalized;
            frameY = Vector3d.Cross(frameZ, frameX);
        }

        inclination = RadToDeg * Math.Acos(frameZ.z);
        epoch = ut;
        var trueAnomaly = Math.Atan2(Vector3d.Dot(frameY, position), Vector3d.Dot(frameX, position));
        var eccentricAnomaly = GetEccentricAnomalyForTrue(trueAnomaly);
        var meanAnomaly = GetMeanAnomaly(eccentricAnomaly);
        meanAnomalyAtEpoch = meanAnomaly;
        meanMotion = GetMeanMotion();
        orbitTimeAtEpoch = meanAnomalyAtEpoch / meanMotion;
        if (eccentricity < 1.0)
            period = 2 * Math.PI / meanMotion;
        else
            period = double.PositiveInfinity;
    }

    // This is pretty much fake ATM
    public string PatchEndTransition => "<not supported>";

    public bool HasEndTransition => false;

    public double PatchEndTime => 0;

    public Option<Vector3d> RelativePositionApoapsis =>
        ApoapsisRadius.Map(apr => {
            Vector3d vectorToAn = Quaternion.AngleAxis(-(float)lan, Vector3d.up) * Vector3d.right;
            Vector3d vectorToPe = Quaternion.AngleAxis((float)argumentOfPeriapsis, OrbitNormal) * vectorToAn;
            return -apr * vectorToPe;
        });

    public Vector3d RelativePositionPeriapsis {
        get {
            Vector3d vectorToAn = Quaternion.AngleAxis((float)-lan, Vector3d.up) * Vector3d.right;
            Vector3d vectorToPe = Quaternion.AngleAxis((float)argumentOfPeriapsis, OrbitNormal) * vectorToAn;
            return PeriapsisRadius * vectorToPe;
        }
    }

    public Option<double> Apoapsis => ApoapsisRadius.Map(apr => apr - body.radius);
    public double Periapsis => PeriapsisRadius - body.radius;

    public Option<double> ApoapsisRadius =>
        eccentricity < 1 ? Option.Some((1.0 + eccentricity) * semiMajorAxis) : Option.None<double>();

    public double PeriapsisRadius => (1.0 - eccentricity) * semiMajorAxis;

    public double SemiMajorAxis => semiMajorAxis;

    public double Inclination => inclination;

    public double Eccentricity => eccentricity;

    double KSPOrbitModule.IOrbit.Lan => lan;

    public KSPOrbitModule.IBody ReferenceBody => body;

    public double MeanMotion => meanMotion;

    public double Epoch => epoch;

    public double ArgumentOfPeriapsis => argumentOfPeriapsis;

    public double MeanAnomalyAtEpoch => meanAnomalyAtEpoch;

    public double Period => period;

    public ITransformFrame ReferenceFrame => KSPTesting.IDENTITY_COORDINATE_SYSTEM;

    public Vector3d OrbitNormal => -frameZ.normalized.SwapYAndZ;

    public Vector3d RelativePositionForTrueAnomaly(double trueAnomaly) {
        var x = Math.Cos(trueAnomaly);
        var y = Math.Sin(trueAnomaly);
        var r = semiMajorAxis * (1.0 - eccentricity * eccentricity) / (1.0 + eccentricity * x);
        return r * (frameX * x + frameY * y);
    }

    public Position GlobalPositionForTrueAnomaly(double trueAnomaly) {
        return new Position(ReferenceFrame, RelativePositionForTrueAnomaly(trueAnomaly));
    }

    public Position GlobalPosition(double ut) {
        var bodyPosition = body.orbit?.GlobalPosition(ut) ?? new Position(ReferenceFrame, Vector3d.zero);

        return bodyPosition + new Vector(ReferenceFrame, RelativePosition(ut));
    }

    public Vector GlobalRelativePosition(double ut) {
        return new Vector(ReferenceFrame, RelativePosition(ut));
    }

    public VelocityAtPosition GlobalVelocity(double ut) {
        return new VelocityAtPosition(new Velocity(ReferenceFrame.motionFrame, GetOrbitalVelocityAtUT(ut).SwapYAndZ),
            GlobalPosition(ut));
    }

    public Vector3d OrbitalVelocity(double ut) {
        return GetOrbitalVelocityAtUT(ut).SwapYAndZ;
    }

    public Vector3d RelativePosition(double ut) {
        return GetRelativePositionAtUT(ut).SwapYAndZ;
    }

    public Vector3d Prograde(double ut) {
        return OrbitalVelocity(ut).normalized;
    }

    public Vector3d NormalPlus(double ut) {
        return -frameZ.SwapYAndZ;
    }

    public Vector3d RadialPlus(double ut) {
        return Vector3d.Exclude(Prograde(ut), Up(ut)).normalized;
    }

    public Vector3d Up(double ut) {
        return RelativePosition(ut).normalized;
    }

    public double Radius(double ut) {
        return RelativePosition(ut).magnitude;
    }

    public Vector3d Horizontal(double ut) {
        return Vector3d.Exclude(Up(ut), Prograde(ut)).normalized;
    }

    public KSPOrbitModule.IOrbit PerturbedOrbit(double ut, Vector3d dV) {
        return new MockOrbit(body,
            GetRelativePositionAtUT(ut), GetOrbitalVelocityAtUT(ut) + dV.SwapYAndZ, ut);
    }

    public double MeanAnomalyAtUt(double ut) {
        // We use ObtAtEpoch and not meanAnomalyAtEpoch because somehow meanAnomalyAtEpoch
        // can be wrong when using the RealSolarSystem mod. ObtAtEpoch is always correct.
        var ret = (orbitTimeAtEpoch + (ut - epoch)) * MeanMotion;
        if (eccentricity < 1) ret = DirectBindingMath.ClampRadians2Pi(ret);
        return ret;
    }

    public double UTAtMeanAnomaly(double meanAnomaly, double ut) {
        var currentMeanAnomaly = MeanAnomalyAtUt(ut);
        var meanDifference = meanAnomaly - currentMeanAnomaly;
        if (eccentricity < 1) meanDifference = DirectBindingMath.ClampRadians2Pi(meanDifference);
        return ut + meanDifference / MeanMotion;
    }

    public double GetMeanAnomalyAtEccentricAnomaly(double ecc) {
        var e = eccentricity;
        if (e < 1)
            //elliptical orbits
            return DirectBindingMath.ClampRadians2Pi(ecc - e * Math.Sin(ecc));
        //hyperbolic orbits
        return e * Math.Sinh(ecc) - ecc;
    }

    public double GetEccentricAnomalyAtTrueAnomaly(double trueAnomaly) {
        var e = eccentricity;
        trueAnomaly = DirectBindingMath.ClampRadians2Pi(trueAnomaly);

        if (e < 1) {
            //elliptical orbits
            var cosE = (e + Math.Cos(trueAnomaly)) / (1 + e * Math.Cos(trueAnomaly));
            var sinE = Math.Sqrt(1 - cosE * cosE);
            if (trueAnomaly > Math.PI) sinE *= -1;

            return DirectBindingMath.ClampRadians2Pi(Math.Atan2(sinE, cosE));
        }

        //hyperbolic orbits
        var coshE = (e + Math.Cos(trueAnomaly)) / (1 + e * Math.Cos(trueAnomaly));
        if (coshE < 1)
            throw new ArgumentException("OrbitExtensions.GetEccentricAnomalyAtTrueAnomaly: True anomaly of " +
                                        trueAnomaly + " radians is not attained by orbit with eccentricity " +
                                        eccentricity);

        var ecc = DirectBindingMath.Acosh(coshE);
        if (trueAnomaly > Math.PI) ecc *= -1;

        return ecc;
    }

    public double TimeOfTrueAnomaly(double trueAnomaly, Option<double> maybeUT = new()) {
        var ut = maybeUT.GetValueOrDefault(0);
        return UTAtMeanAnomaly(GetMeanAnomalyAtEccentricAnomaly(GetEccentricAnomalyAtTrueAnomaly(trueAnomaly)), ut);
    }

    public double NextPeriapsisTime(Option<double> maybeUT = new()) {
        var ut = maybeUT.GetValueOrDefault(0);
        if (eccentricity < 1)
            return TimeOfTrueAnomaly(0, Option.Some(ut));
        return ut - MeanAnomalyAtUt(ut) / MeanMotion;
    }

    public Option<double> NextApoapsisTime(Option<double> maybeUT = new()) {
        var ut = maybeUT.GetValueOrDefault(0);
        if (eccentricity < 1) return Option.Some(TimeOfTrueAnomaly(Math.PI, Option.Some(ut)));

        return Option.None<double>();
    }

    public double TrueAnomalyAtRadius(double radius) {
        radius = Math.Max(radius, PeriapsisRadius);
        radius = ApoapsisRadius.Map(apr => Math.Min(radius, apr)).GetValueOrDefault(radius);

        return Math.Acos((semiMajorAxis * (1.0 - eccentricity * eccentricity) / radius - 1.0) / eccentricity);
    }

    public Option<double> NextTimeOfRadius(double ut, double radius) {
        if (radius < PeriapsisRadius || (eccentricity < 1 && radius > ApoapsisRadius.value))
            return Option.None<double>();

        var trueAnomaly1 = TrueAnomalyAtRadius(radius);
        var trueAnomaly2 = 2 * Math.PI - trueAnomaly1;
        var time1 = TimeOfTrueAnomaly(trueAnomaly1, Option.Some(ut));
        var time2 = TimeOfTrueAnomaly(trueAnomaly2, Option.Some(ut));
        if (time2 < time1 && time2 > ut) return Option.Some(time2);
        return Option.Some(time1);
    }

    public double SynodicPeriod(KSPOrbitModule.IOrbit other) {
        var sign = Vector3d.Dot(OrbitNormal, other.OrbitNormal) > 0 ? 1 : -1; //detect relative retrograde motion
        return Math.Abs(1.0 /
                        (1.0 / Period - sign * 1.0 / other.Period)); //period after which the phase angle repeats
    }

    public Vector3d RelativeAscendingNode => Quaternion.AngleAxis(-(float)lan, Vector3d.up) * Vector3d.right;

    public Vector3d RelativeEccentricityVector => eccVec;

    public double TrueAnomalyFromVector(Vector3d vec) {
        var oNormal = OrbitNormal;
        var projected = Vector3d.Exclude(oNormal, vec);
        var vectorToPe = RelativePositionPeriapsis;
        var angleFromPe = Vector3d.Angle(vectorToPe, projected);

        //If the vector points to the infalling part of the orbit then we need to do 360 minus the
        //angle from Pe to get the true anomaly. Test this by taking the the cross product of the
        //orbit normal and vector to the periapsis. This gives a vector that points to center of the
        //outgoing side of the orbit. If vectorToAN is more than 90 degrees from this vector, it occurs
        //during the infalling part of the orbit.
        if (Math.Abs(Vector3d.Angle(projected, Vector3d.Cross(oNormal, vectorToPe))) < 90)
            return angleFromPe * DirectBindingMath.DegToRad;
        return (360 - angleFromPe) * DirectBindingMath.DegToRad;
    }

    public double AscendingNodeTrueAnomaly(KSPOrbitModule.IOrbit b) {
        var vectorToAn = Vector3d.Cross(OrbitNormal, b.OrbitNormal);
        return TrueAnomalyFromVector(vectorToAn);
    }

    public double DescendingNodeTrueAnomaly(KSPOrbitModule.IOrbit b) {
        return DirectBindingMath.ClampRadians2Pi(AscendingNodeTrueAnomaly(b) + Math.PI);
    }

    public double TimeOfAscendingNode(KSPOrbitModule.IOrbit b, Option<double> maybeUt = new()) {
        return TimeOfTrueAnomaly(AscendingNodeTrueAnomaly(b), maybeUt);
    }

    public double TimeOfDescendingNode(KSPOrbitModule.IOrbit b, Option<double> maybeUt = new()) {
        return TimeOfTrueAnomaly(DescendingNodeTrueAnomaly(b), maybeUt);
    }

    public override string ToString() {
        return KSPOrbitModule.OrbitToString(this);
    }

    public string ToFixed(long decimals) {
        return KSPOrbitModule.OrbitToFixed(this, decimals);
    }

    public double GetEccentricAnomalyForTrue(double trueAnomaly) {
        var x = Math.Cos(trueAnomaly / 2.0);
        var y = Math.Sin(trueAnomaly / 2.0);
        if (eccentricity < 1.0)
            return 2.0 * Math.Atan2(Math.Sqrt(1.0 - eccentricity) * y, Math.Sqrt(1.0 + eccentricity) * x);

        var r = Math.Sqrt((eccentricity - 1.0) / (eccentricity + 1.0)) * y / x;
        if (r >= 1.0) return double.PositiveInfinity;

        if (r <= -1.0) return double.NegativeInfinity;

        return Math.Log((1.0 + r) / (1.0 - r));
    }

    public double GetMeanAnomaly(double eccentricAnomaly) {
        if (eccentricity < 1.0) return eccentricAnomaly - eccentricity * Math.Sin(eccentricAnomaly);

        if (double.IsInfinity(eccentricAnomaly)) return eccentricAnomaly;

        return eccentricity * Math.Sinh(eccentricAnomaly) - eccentricAnomaly;
    }

    public double GetMeanMotion() => Math.Sqrt(body.mu / Math.Abs(semiMajorAxis * semiMajorAxis * semiMajorAxis));

    public double TrueAnomalyAtUT(double ut) => GetTrueAnomalyAtOrbitTime(GetOrbitTimeAtUT(ut));

    public double GetOrbitTimeAtUT(double ut) {
        double orbitTime;
        if (eccentricity < 1.0) {
            orbitTime = (ut - epoch + orbitTimeAtEpoch) % period;
            if (orbitTime > period / 2.0) orbitTime -= period;
        } else {
            orbitTime = orbitTimeAtEpoch + (ut - epoch);
        }

        return orbitTime;
    }

    public double GetOrbitTimeAtMeanAnomaly(double meanAnomaly) => meanAnomaly / meanMotion;

    public Vector3d GetRelativePositionAtUT(double ut) => GetPositionAtOrbitTime(GetOrbitTimeAtUT(ut));

    public Vector3d GetPositionAtOrbitTime(double orbitTime) => RelativePositionForTrueAnomaly(GetTrueAnomalyAtOrbitTime(orbitTime));

    public Vector3d GetOrbitalVelocityAtUT(double ut) => GetOrbitalVelocityAtOrbitTime(GetOrbitTimeAtUT(ut));

    public Vector3d GetOrbitalVelocityAtOrbitTime(double orbitTime) => GetOrbitalVelocityAtTrueAnomaly(GetTrueAnomalyAtOrbitTime(orbitTime));

    public Vector3d GetOrbitalVelocityAtTrueAnomaly(double trueAnomaly) {
        var x = Math.Cos(trueAnomaly);
        var y = Math.Sin(trueAnomaly);
        var muOverH = Math.Sqrt(body.mu / (semiMajorAxis * (1.0 - eccentricity * eccentricity)));
        var vx = -y * muOverH;
        var vy = (x + eccentricity) * muOverH;
        return frameX * vx + frameY * vy;
    }

    public double GetTrueAnomalyAtOrbitTime(double orbitTime) {
        var meanAnomaly = orbitTime * meanMotion;
        var eccentricAnomaly = GetEccentricAnomalyForMean(meanAnomaly);
        return GetTrueAnomalyForEccentric(eccentricAnomaly);
    }

    public double GetEccentricAnomalyForMean(double meanAnomaly) {
        if (eccentricity < 1.0) {
            if (eccentricity < 0.8)
                return SolveEccentricAnomalyNewton(meanAnomaly);
            return SolveEccentricAnomalySeries(meanAnomaly);
        }

        return SolveEccentricAnomalyHypNewton(meanAnomaly);
    }

    private double SolveEccentricAnomalyNewton(double meanAnomaly) {
        var dE = 1.0;
        var ecc = meanAnomaly + eccentricity * Math.Sin(meanAnomaly) +
                  0.5 * eccentricity * eccentricity * Math.Sin(2.0 * meanAnomaly);
        while (Math.Abs(dE) > 1e-7) {
            var y = ecc - eccentricity * Math.Sin(ecc);
            dE = (meanAnomaly - y) / (1.0 - eccentricity * Math.Cos(ecc));
            ecc += dE;
        }

        return ecc;
    }

    private double SolveEccentricAnomalySeries(double mean) {
        var ecc = mean + 0.85 * eccentricity * Math.Sign(Math.Sin(mean));
        for (var i = 0; i < 8; i++) {
            var f1 = eccentricity * Math.Sin(ecc);
            var f2 = eccentricity * Math.Cos(ecc);
            var f4 = ecc - f1 - mean;
            var f5 = 1.0 - f2;
            ecc += -5.0 * f4 / (f5 + Math.Sign(f5) * Math.Sqrt(Math.Abs(16.0 * f5 * f5 - 20.0 * f4 * f1)));
        }

        return ecc;
    }

    private double SolveEccentricAnomalyHypNewton(double meanAnomaly) {
        var dE = 1.0;
        var f = 2.0 * meanAnomaly / eccentricity;
        var ecc = Math.Log(Math.Sqrt(f * f + 1.0) + f);
        while (Math.Abs(dE) > 1e-7) {
            dE = (eccentricity * Math.Sinh(ecc) - ecc - meanAnomaly) / (eccentricity * Math.Cosh(ecc) - 1.0);
            ecc -= dE;
        }

        return ecc;
    }

    public double GetTrueAnomalyForEccentric(double eccentricAnomaly) {
        if (eccentricity < 1.0) {
            var x = Math.Cos(eccentricAnomaly / 2.0);
            var y = Math.Sin(eccentricAnomaly / 2.0);
            return 2.0 * Math.Atan2(Math.Sqrt(1.0 + eccentricity) * y, Math.Sqrt(1.0 - eccentricity) * x);
        } else {
            var x = Math.Cosh(eccentricAnomaly / 2.0);
            var y = Math.Sinh(eccentricAnomaly / 2.0);
            return 2.0 * Math.Atan2(Math.Sqrt(eccentricity + 1.0) * y, Math.Sqrt(eccentricity - 1.0) * x);
        }
    }
}
