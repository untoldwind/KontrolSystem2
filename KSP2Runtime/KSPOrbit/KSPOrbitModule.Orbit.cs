using System;
using System.Globalization;
using KontrolSystem.KSP.Runtime.KSPMath;
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

            [KSField(Description = "Universal time of the start of the orbit, in case it is an orbit-patch")]
            public double StartUt { get; }

            [KSField(Description = "Universal time of the start of the orbit, in case it is an orbit-patch")]
            public double EndUt { get; }

            [KSField(Description = "Apoapsis of the orbit above sealevel of the `reference_body`. Is not defined for a hyperbolic orbit")]
            Option<double> Apoapsis { get; }

            [KSField(Description = "Periapsis of the orbit above sealevel of the `reference_body`")]
            double Periapsis { get; }

            [KSField(Description = "Radius of apoapsis of the orbit (i.e. from the center of the `reference_body'). Is not defined for a hyperbolic orbit")]
            Option<double> ApoapsisRadius { get; }

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

            [KSField(Description = "Reference frame of the orbit. All relative vectors are in this frame.")]
            public ITransformFrame ReferenceFrame { get; }

            [KSField(Description = "Normal vector perpendicular to orbital plane.")]
            Vector3d OrbitNormal { get; }

            [KSMethod(Description = "Get the relative orbital velocity at a given universal time `ut`")]
            Vector3d OrbitalVelocity(double ut);

            [KSMethod(Description = "Get the absolute position at a given universal time `ut`")]
            Position GlobalPosition(double ut);

            [KSMethod(Description = "Get the coordinate independent velocity at a given universal time `ut`")]
            VelocityAtPosition GlobalVelocity(double ut);

            [KSMethod(Description = "Get relative position at a given universal time `ut`")]
            Vector3d RelativePosition(double ut);

            [KSMethod(Description = "Get relative position for a given `trueAnomaly`")] 
            Vector3d RelativePositionForTrueAnomaly(double trueAnomaly);
                
            [KSMethod(Description = "The relative prograde vector at a given universal time `ut`")]
            Vector3d Prograde(double ut);

            [KSMethod(Description = "The relative normal-plus vector at a given universal time `ut`")]
            Vector3d NormalPlus(double ut);

            [KSMethod(Description = "The relative radial-plus vector at a given universal time `ut`")]
            Vector3d RadialPlus(double ut);

            [KSMethod(Description = "Relative up vector of the orbit at a given universal time `ut`")]
            Vector3d Up(double ut);

            [KSMethod(Description = "Get the orbital radius (distance from center of body) at a given universal time `ut`")]
            double Radius(double ut);

            [KSMethod(Description = "Relative horizontal vector at a given universal time `ut`")]
            Vector3d Horizontal(double ut);

            [KSMethod(Description = "Returns a new Orbit object that represents the result of applying a given relative `deltaV` to o at `ut`.")]
            IOrbit PerturbedOrbit(double ut, Vector3d dV);

            [KSMethod(Description = @"The mean anomaly of the orbit.
                For elliptical orbits, the value return is always between 0 and 2pi.
                For hyperbolic orbits, the value can be any number.")]
            double MeanAnomalyAtUt(double ut);

            [KSMethod("ut_at_mean_anomaly", Description = @"The next time at which the orbiting object will reach the given mean anomaly.
                For elliptical orbits, this will be a time between UT and UT + o.period.
                For hyperbolic orbits, this can be any time, including a time in the past, if the given mean anomaly occurred in the past")]
            double UTAtMeanAnomaly(double meanAnomaly, double ut);

            [KSMethod(Description = @"Converts an eccentric anomaly into a mean anomaly.
                For an elliptical orbit, the returned value is between 0 and 2pi.
                For a hyperbolic orbit, the returned value is any number.")]
            double GetMeanAnomalyAtEccentricAnomaly(double ecc);

            [KSMethod(Description = @"Converts a true anomaly into an eccentric anomaly.
                For elliptical orbits this returns a value between 0 and 2pi.
                For hyperbolic orbits the returned value can be any number.")]
            double GetEccentricAnomalyAtTrueAnomaly(double trueAnomaly);

            [KSMethod(Description = @"Next time of a certain true anomaly after a given universal time `ut`. 
                If `ut` is omitted the current time will be used")]
            double TimeOfTrueAnomaly(double trueAnomaly, Option<double> maybeUt = new Option<double>());

            [KSMethod(Description = @"The next time at which the orbiting object will be at periapsis after a given universal time `ut`.
                If `ut` is omitted the current time will be used.
                For elliptical orbits, this will be between `ut` and `ut` + Period.
                For hyperbolic orbits, this can be any time, including a time in the past, if the periapsis is in the past.")]
            double NextPeriapsisTime(Option<double> ut = new Option<double>());

            [KSMethod(Description = @"Returns the next time at which the orbiting object will be at apoapsis after a given universal time `ut`.
                If `ut` is omitted the current time will be used.
                For elliptical orbits, this will be between `ut` and `ut` + Period.
                For hyperbolic orbits, this is undefined.")]
            Option<double> NextApoapsisTime(Option<double> ut = new Option<double>());

            [KSMethod(Description = @"Get the true anomaly of a radius.
                If the radius is below the periapsis the true anomaly of the periapsis will be returned.
                If it is above the apoapsis the true anomaly of the apoapsis is returned.")]
            double TrueAnomalyAtRadius(double radius);

            [KSMethod(Description = @"Finds the next time at which the orbiting object will achieve a given `radius` from center of the body
                after a given universal time `ut`.
                This will be undefined if the specified `radius` is impossible for this orbit, otherwise:
                For elliptical orbits this will be a time between `ut` and `ut` + period.
                For hyperbolic orbits this can be any time. If the given radius will be achieved
                in the future then the next time at which that radius will be achieved will be returned.
                If the given radius was only achieved in the past, then there are no guarantees
                about which of the two times in the past will be returned.")]
            Option<double> NextTimeOfRadius(double ut, double radius);

            [KSMethod(Description = @"Computes the period of the phase angle between orbiting objects of this orbit and and `other` orbit.
                 For noncircular orbits the time variation of the phase angle is only quasiperiodic
                 and for high eccentricities and/or large relative inclinations, the relative motion is
                 not really periodic at all.")]
            double SynodicPeriod(IOrbit other);

            [KSField(Description = "Get the relative position of the ascending node.")]
            Vector3d RelativeAscendingNode { get; }
            
            [KSField(Description = "Get the relative eccentricity vector.")]
            Vector3d RelativeEccentricityVector { get; }
            
            [KSMethod(Description = "Convert orbital parameters to string.")]
            string ToString();

            [KSMethod(Description = "Convert orbital parameter to string using specified number of `decimals`")]
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
