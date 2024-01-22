using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.TO2.Runtime;
using KSP.Sim;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    // Not a real orbit, just something to return if there is no reference body.
    public class StaticOrbit : KSPOrbitModule.IOrbit {
        private readonly IKSPContext context;
        private readonly CelestialBodyComponent body;

        public StaticOrbit(IKSPContext context, CelestialBodyComponent body) {
            this.context = context;
            this.body = body;
        }

        public KSPOrbitModule.IBody ReferenceBody => new BodyWrapper(context, body);

        public double StartUt => 0;
        public double EndUt => 0;
        public PatchTransitionType StartTransition => PatchTransitionType.Initial;
        public PatchTransitionType EndTransition => PatchTransitionType.Final;
        public Option<KSPOrbitModule.IOrbit> PreviousPatch => new Option<KSPOrbitModule.IOrbit>();
        public Option<KSPOrbitModule.IOrbit> NextPatch => new Option<KSPOrbitModule.IOrbit>();
        public Option<double> Apoapsis => new Option<double>();
        public double Periapsis => 0;
        public Option<double> ApoapsisRadius => new Option<double>();
        public double PeriapsisRadius => 0;
        public double SemiMajorAxis => 0;
        public double Inclination => 0;
        public double Eccentricity => 0;
        public double Lan => 0;
        public double Epoch => 0;
        public double ArgumentOfPeriapsis => 0;
        public double MeanAnomalyAtEpoch => 0;
        public double MeanMotion => 0;
        public double Period => 0;
        public ITransformFrame ReferenceFrame => body.transform.celestialFrame;
        public Vector3d OrbitNormal => Vector3d.up;
        
        public Vector3d OrbitalVelocity(double ut) => Vector3d.zero;

        public Position GlobalPosition(double ut) => body.Position;

        public Vector GlobalRelativePosition(double ut) => new Vector(ReferenceFrame, Vector3d.zero);

        public VelocityAtPosition GlobalVelocity(double ut) => new VelocityAtPosition(body.Velocity, body.Position);

        public Vector3d RelativePosition(double ut) => Vector3d.zero;

        public Vector3d RelativePositionForTrueAnomaly(double trueAnomaly) => Vector3d.zero;

        public Position GlobalPositionForTrueAnomaly(double trueAnomaly) => body.Position;

        public Vector3d Prograde(double ut) => Vector3d.forward;

        public Vector3d NormalPlus(double ut) => Vector3d.up;

        public Vector3d RadialPlus(double ut) => Vector3d.right;

        public Vector3d Up(double ut) => Vector3d.up;

        public double Radius(double ut) => 0;

        public Vector3d Horizontal(double ut) => Vector3d.forward;

        public KSPOrbitModule.IOrbit PerturbedOrbit(double ut, Vector3d dV) => this;

        public double MeanAnomalyAtUt(double ut) => 0;

        public double UTAtMeanAnomaly(double meanAnomaly, double ut) => 0;

        public double GetMeanAnomalyAtEccentricAnomaly(double ecc) => 0;

        public double GetEccentricAnomalyAtTrueAnomaly(double trueAnomaly) => 0;

        public double TimeOfTrueAnomaly(double trueAnomaly, Option<double> maybeUt = new Option<double>()) => 0;

        public double NextPeriapsisTime(Option<double> ut = new Option<double>()) => 0;

        public Option<double> NextApoapsisTime(Option<double> ut = new Option<double>()) => new Option<double>();

        public double TrueAnomalyAtRadius(double radius) => 0;

        public Option<double> NextTimeOfRadius(double ut, double radius) => new Option<double>();

        public double SynodicPeriod(KSPOrbitModule.IOrbit other) => 0;

        public double TrueAnomalyFromVector(Vector3d vec) => 0;

        public double AscendingNodeTrueAnomaly(KSPOrbitModule.IOrbit b) => 0;

        public double DescendingNodeTrueAnomaly(KSPOrbitModule.IOrbit b) => 0;

        public double TimeOfAscendingNode(KSPOrbitModule.IOrbit b, Option<double> maybeUt = new Option<double>()) => 0;

        public double TimeOfDescendingNode(KSPOrbitModule.IOrbit b, Option<double> maybeUt = new Option<double>()) => 0;

        public Vector3d RelativeAscendingNode => Vector3d.zero;
        public Vector3d RelativeEccentricityVector => Vector3d.zero;

        public string ToFixed(long decimals) => "Static orbit";
    }
}
