using System;
using System.Globalization;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    public partial class KSPOrbitModule {
        [KSClass("Orbit",
            Description = "Represents an in-game orbit."
        )]
        public interface IOrbit {
            [KSField(Description = "The celestial body the orbit is referenced on.")]
            IBody ReferenceBody { get; }

            [KSField]
            public double StartUt { get; }

            [KSField]
            public double EndUt { get; }

            [KSField(Description = "Apoapsis of the orbit above sealevel of the `reference_body`.")]
            double Apoapsis { get; }

            [KSField(Description = "Periapsis of the orbit above sealevel of the `reference_body`")]
            double Periapsis { get; }

            [KSField(Description = "Radius of apoapsis of the orbit (i.e. from the center of the `reference_body')")]
            double ApoapsisRadius { get; }

            [KSField(Description = "Radius of periapsis of the orbit (i.e. from the center of the `reference_body')")]
            double PeriapsisRadius { get; }

            [KSField(Description = "Semi major axis of the orbit.")]
            double SemiMajorAxis { get; }

            [KSField(Description = "Inclination of the orbit in degree.")]
            double Inclination { get; }

            [KSField(Description = "Eccentricity of the orbit.")]
            double Eccentricity { get; }

            [KSField("LAN", Description = "Longitude of ascending node of the orbit in degree")]
            double Lan { get; }

            [KSField(Description = "Orbit epoch.")]
            double Epoch { get; }

            [KSField(Description = "Argument of periapsis of the orbit.")]
            double ArgumentOfPeriapsis { get; }

            [KSField(Description = "Mean anomaly of the orbit at `epoch`")]
            double MeanAnomalyAtEpoch { get; }

            [KSField(Description = "Mean motion of the orbit.")]
            double MeanMotion { get; }

            [KSField(Description = "Orbital period.")]
            double Period { get; }

            [KSField]
            public ITransformFrame ReferenceFrame { get; }

            [KSField(Description = "Normal vector perpendicular to orbital plane.")]
            Vector3d OrbitNormal { get; }

            [KSMethod]
            Vector3d OrbitalVelocity(double ut);

            [KSMethod(Description = "Get the absolute position at a given universal time `ut`")]
            Vector3d AbsolutePosition(double ut);

            [KSMethod(Description = "Get the absolute position at a given universal time `ut`")]
            Position Position(double ut);

            [KSMethod]
            Vector Velocity(double ut);

            [KSMethod]
            Vector3d RelativePosition(double ut);

            [KSMethod]
            Vector3d Prograde(double ut);

            [KSMethod]
            Vector3d NormalPlus(double ut);

            [KSMethod]
            Vector3d RadialPlus(double ut);

            [KSMethod]
            Vector3d Up(double ut);

            [KSMethod]
            double Radius(double ut);

            [KSMethod]
            Vector3d Horizontal(double ut);

            /// <summary>
            /// Returns a new Orbit object that represents the result of applying a given dV to o at UT
            /// </summary>
            [KSMethod]
            IOrbit PerturbedOrbit(double ut, Vector3d dV);

            /// <summary>
            /// The mean anomaly of the orbit.
            /// For elliptical orbits, the value return is always between 0 and 2pi
            /// For hyperbolic orbits, the value can be any number.
            /// </summary>
            [KSMethod]
            double MeanAnomalyAtUt(double ut);

            /// <summary>
            /// The next time at which the orbiting object will reach the given mean anomaly.
            /// For elliptical orbits, this will be a time between UT and UT + o.period
            /// For hyperbolic orbits, this can be any time, including a time in the past, if
            /// the given mean anomaly occurred in the past
            /// </summary>
            [KSMethod]
            double UTAtMeanAnomaly(double meanAnomaly, double ut);

            /// <summary>
            /// Converts an eccentric anomaly into a mean anomaly.
            /// For an elliptical orbit, the returned value is between 0 and 2pi
            /// For a hyperbolic orbit, the returned value is any number
            /// </summary>
            [KSMethod]
            double GetMeanAnomalyAtEccentricAnomaly(double ecc);

            /// <summary>
            /// Converts a true anomaly into an eccentric anomaly.
            /// For elliptical orbits this returns a value between 0 and 2pi
            /// For hyperbolic orbits the returned value can be any number.
            /// NOTE: For a hyperbolic orbit, if a true anomaly is requested that does not exist (a true anomaly
            /// past the true anomaly of the asymptote) then an ArgumentException is thrown
            /// </summary>
            [KSMethod]
            double GetEccentricAnomalyAtTrueAnomaly(double trueAnomaly);

            /// <summary>
            /// Next time of a certain true anomly.
            /// NOTE: this function can throw an ArgumentException, if o is a hyperbolic orbit with an eccentricity
            /// large enough that it never attains the given true anomaly.
            /// </summary>
            [KSMethod]
            double TimeOfTrueAnomaly(double trueAnomaly, double ut);

            /// <summary>
            /// The next time at which the orbiting object will be at periapsis.
            /// For elliptical orbits, this will be between UT and UT + Period.
            /// For hyperbolic orbits, this can be any time, including a time in the past,
            /// if the periapsis is in the past.
            /// </summary>
            [KSMethod]
            double NextPeriapsisTime(Option<double> ut = new Option<double>());

            /// <summary>
            /// Returns the next time at which the orbiting object will be at apoapsis.
            /// For elliptical orbits, this is a time between UT and UT + period.
            /// For hyperbolic orbits, this throws an ArgumentException.
            /// </summary>
            [KSMethod]
            Result<double, string> NextApoapsisTime(Option<double> ut = new Option<double>());

            /// <summary>
            /// Get the true anomaly of a radius.
            /// If the radius is below the periapsis the true anomaly of the periapsis
            /// with be returned. If it is above the apoapsis the true anomaly of the
            /// apoapsis is returned.
            /// </summary>
            [KSMethod]
            double TrueAnomalyAtRadius(double radius);

            /// <summary>
            /// Finds the next time at which the orbiting object will achieve a given radius
            /// from the center of the primary.
            /// If the given radius is impossible for this orbit, an ArgumentException is thrown.
            /// For elliptical orbits this will be a time between UT and UT + period
            /// For hyperbolic orbits this can be any time. If the given radius will be achieved
            /// in the future then the next time at which that radius will be achieved will be returned.
            /// If the given radius was only achieved in the past, then there are no guarantees
            /// about which of the two times in the past will be returned.
            /// </summary>
            [KSMethod]
            Result<double, string> NextTimeOfRadius(double ut, double radius);

            /// <summary>
            /// Computes the period of the phase angle between orbiting objects a and b.
            /// This only really makes sense for approximately circular orbits in similar planes.
            /// For noncircular orbits the time variation of the phase angle is only "quasiperiodic"
            /// and for high eccentricities and/or large relative inclinations, the relative motion is
            /// not really periodic at all.
            /// </summary>
            [KSMethod]
            double SynodicPeriod(IOrbit other);

            [KSMethod]
            string ToString();

            [KSMethod]
            string ToFixed(long decimals);
        }

        internal static string OrbitToString(IOrbit orbit) {
            return
                $"Orbit(inc={orbit.Inclination.ToString(CultureInfo.InvariantCulture)},ecc={orbit.Eccentricity.ToString(CultureInfo.InvariantCulture)},sma={orbit.SemiMajorAxis.ToString(CultureInfo.InvariantCulture)},lan={orbit.Lan.ToString(CultureInfo.InvariantCulture)},aop={orbit.ArgumentOfPeriapsis.ToString(CultureInfo.InvariantCulture)},mae={orbit.MeanAnomalyAtEpoch.ToString(CultureInfo.InvariantCulture)},epoch={orbit.Epoch.ToString(CultureInfo.InvariantCulture)})";
        }

        internal static string OrbitToFixed(IOrbit orbit, long decimals) {
            String format = decimals <= 0 ? "F0" : "F" + decimals;
            return
                $"Orbit(inc={orbit.Inclination.ToString(format, CultureInfo.InvariantCulture)},ecc={orbit.Eccentricity.ToString(format, CultureInfo.InvariantCulture)},sma={orbit.SemiMajorAxis.ToString(format, CultureInfo.InvariantCulture)},lan={orbit.Lan.ToString(format, CultureInfo.InvariantCulture)},aop={orbit.ArgumentOfPeriapsis.ToString(format, CultureInfo.InvariantCulture)},mae={orbit.MeanAnomalyAtEpoch.ToString(format, CultureInfo.InvariantCulture)},epoch={orbit.Epoch.ToString(format, CultureInfo.InvariantCulture)})";
        }
    }
}
