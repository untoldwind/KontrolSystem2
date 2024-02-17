using System;
using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.TO2.Runtime;
using KSP.Sim;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPOrbit;

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

    public PatchTransitionType StartTransition => orbit.PatchStartTransition;

    public PatchTransitionType EndTransition => orbit.PatchEndTransition;

    public Option<KSPOrbitModule.IOrbit> PreviousPatch {
        get {
            var previous = orbit.PreviousPatch;

            return previous is { ActivePatch: true } && previous is PatchedConicsOrbit prevOrbit
                ? new Option<KSPOrbitModule.IOrbit>(new OrbitWrapper(context, prevOrbit))
                : new Option<KSPOrbitModule.IOrbit>();
        }
    }

    public Option<KSPOrbitModule.IOrbit> NextPatch {
        get {
            var next = orbit.NextPatch;

            return next is { ActivePatch: true } && next is PatchedConicsOrbit nextOrbit
                ? new Option<KSPOrbitModule.IOrbit>(new OrbitWrapper(context, nextOrbit))
                : new Option<KSPOrbitModule.IOrbit>();
        }
    }

    public Option<double> Apoapsis => orbit.eccentricity < 1 ? Option.Some(orbit.ApoapsisArl) : Option.None<double>();

    public double Periapsis => orbit.PeriapsisArl;

    public Option<double> ApoapsisRadius =>
        orbit.eccentricity < 1 ? Option.Some(orbit.Apoapsis) : Option.None<double>();

    public double PeriapsisRadius => orbit.Periapsis;

    public double SemiMajorAxis => orbit.semiMajorAxis;

    public double Inclination => orbit.inclination;

    public double Eccentricity => orbit.eccentricity;

    public double Lan => orbit.longitudeOfAscendingNode;

    public double Epoch => orbit.epoch;

    public double ArgumentOfPeriapsis => orbit.argumentOfPeriapsis;

    public double MeanAnomalyAtEpoch => orbit.meanAnomalyAtEpoch;

    public double MeanMotion => orbit.meanMotion;

    public double Period => orbit.period;

    public ITransformFrame ReferenceFrame => orbit.ReferenceFrame;

    public Vector3d OrbitNormal => -orbit.GetRelativeOrbitNormal().SwapYAndZ;

    public Vector3d RelativePosition(double ut) {
        return orbit.GetRelativePositionAtUTZup(ut).SwapYAndZ;
    }

    public Vector3d RelativePositionForTrueAnomaly(double trueAnomaly) {
        return orbit.GetRelativePositionFromTrueAnomaly(trueAnomaly * DirectBindingMath.DegToRad);
    }

    public Position GlobalPositionForTrueAnomaly(double trueAnomaly) {
        return new Position(ReferenceFrame,
            orbit.GetRelativePositionFromTrueAnomaly(trueAnomaly * DirectBindingMath.DegToRad));
    }

    public Position GlobalPosition(double ut) {
        return ReferenceBody.Orbit.GlobalPosition(ut) + GlobalRelativePosition(ut);
    }

    public Vector GlobalRelativePosition(double ut) {
        return new Vector(ReferenceFrame, orbit.GetRelativePositionAtUTZup(ut).SwapYAndZ);
    }

    public VelocityAtPosition GlobalVelocity(double ut) {
        return new VelocityAtPosition(
            new Velocity(context.Game.UniverseModel.GalacticOrigin.celestialFrame.motionFrame,
                orbit.GetFrameVelAtUTZup(ut).SwapYAndZ), GlobalPosition(ut));
    }

    public Vector3d OrbitalVelocity(double ut) {
        return orbit.GetOrbitalVelocityAtUTZup(ut).SwapYAndZ;
    }

    public Vector3d Prograde(double ut) {
        return OrbitalVelocity(ut).normalized;
    }

    public Vector3d NormalPlus(double ut) {
        return orbit.GetRelativeOrbitNormal().SwapYAndZ.normalized;
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
        return ReferenceBody.CreateOrbit(RelativePosition(ut), OrbitalVelocity(ut) + dV, ut);
    }

    public double MeanAnomalyAtUt(double ut) {
        var ret = (orbit.ObTAtEpoch + (ut - orbit.epoch)) * orbit.meanMotion;

        return orbit.eccentricity < 1 ? DirectBindingMath.ClampRadians2Pi(ret) : ret;
    }

    public double UTAtMeanAnomaly(double meanAnomaly, double ut) {
        var currentMeanAnomaly = MeanAnomalyAtUt(ut);
        var meanDifference = meanAnomaly - currentMeanAnomaly;
        if (orbit.eccentricity < 1) meanDifference = DirectBindingMath.ClampRadians2Pi(meanDifference);
        return ut + meanDifference / MeanMotion;
    }

    public double GetMeanAnomalyAtEccentricAnomaly(double ecc) {
        return orbit.GetMeanAnomaly(ecc);
    }

    public double GetEccentricAnomalyAtTrueAnomaly(double trueAnomaly) {
        return orbit.GetEccentricAnomaly(trueAnomaly);
    }

    public double TimeOfTrueAnomaly(double trueAnomaly, Option<double> maybeUt = new()) {
        var ut = maybeUt.GetValueOrDefault(context.UniversalTime);
        return UTAtMeanAnomaly(GetMeanAnomalyAtEccentricAnomaly(GetEccentricAnomalyAtTrueAnomaly(trueAnomaly)), ut);
    }

    public double NextPeriapsisTime(Option<double> maybeUt = new()) {
        var ut = maybeUt.GetValueOrDefault(context.UniversalTime);
        if (orbit.eccentricity < 1)
            return TimeOfTrueAnomaly(0, Option.Some(ut));
        return ut - MeanAnomalyAtUt(ut) / MeanMotion;
    }

    public Option<double> NextApoapsisTime(Option<double> maybeUt = new()) {
        var ut = maybeUt.GetValueOrDefault(context.UniversalTime);
        if (orbit.eccentricity < 1) return Option.Some(TimeOfTrueAnomaly(Math.PI, Option.Some(ut)));

        return Option.None<double>();
    }

    public double TrueAnomalyAtRadius(double radius) {
        return orbit.TrueAnomalyAtRadius(radius);
    }

    public Option<double> NextTimeOfRadius(double ut, double radius) {
        if (radius < orbit.Periapsis || (orbit.eccentricity < 1 && radius > orbit.Apoapsis))
            return Option.None<double>();

        var trueAnomaly1 = orbit.TrueAnomalyAtRadius(radius);
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

    public double TrueAnomalyFromVector(Vector3d vec) {
        var oNormal = OrbitNormal;
        var projected = Vector3d.Exclude(oNormal, vec.normalized);
        var vectorToPe = RelativeEccentricityVector;
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

    public Vector3d RelativeAscendingNode =>
        ReferenceFrame.ToLocalPosition(orbit.ReferenceFrame, orbit.GetRelativeANVector().SwapYAndZ);

    public Vector3d RelativeEccentricityVector =>
        ReferenceFrame.ToLocalPosition(orbit.ReferenceFrame, orbit.GetRelativeEccVector().SwapYAndZ);

    public override string ToString() {
        return KSPOrbitModule.OrbitToString(this);
    }

    public string ToFixed(long decimals) {
        return KSPOrbitModule.OrbitToFixed(this, decimals);
    }
}
