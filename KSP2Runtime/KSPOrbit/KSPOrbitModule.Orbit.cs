using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    public partial class KSPOrbitModule {
        [KSClass("Orbit",
            Description = "Represents an in-game orbit."
        )]
        public interface IOrbit {
            [KSField(Description = "The celestial body the orbit is referenced on.")]
            IBody ReferenceBody { get; }

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
        }
    }
}
