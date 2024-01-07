using System;
using System.Collections.Generic;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    [KSModule("ksp::orbit")]
    public partial class KSPOrbitModule {
        [KSFunction(Description = "Find a body by name.")]
        public static Result<IBody, string> FindBody(string name) {
            IBody body = KSPContext.CurrentContext.FindBody(name);
            return body != null ? Result.Ok<IBody, string>(body) : Result.Err<IBody, string>($"No such body '{name}'");
        }

        [KSFunction(Description = "Get the galactic celestial frame.")]
        public static ITransformFrame GalacticOrigin() {
            return KSPContext.CurrentContext.Game.UniverseModel.GalacticOrigin.celestialFrame;
        }

        public static (IEnumerable<RealizedType>, IEnumerable<IKontrolConstant>) DirectBindings() {
            return BindingGenerator.RegisterEnumTypeMappings("ksp::vessel",
                new[] {
                    ("PatchTransitionType", "Transition type at the beginning or end of an orbit patch", typeof(PatchTransitionType), new (Enum value, string description)[] {
                        (PatchTransitionType.Initial, "Initial transition (orbit starts here)"),
                        (PatchTransitionType.Final, "Final transition (orbit ends here)"),
                        (PatchTransitionType.Encounter, "Orbit enters a sphere of influence (SOI)"),
                        (PatchTransitionType.Escape, "Orbit leaves a sphere of influence (SOI)"),
                        (PatchTransitionType.Maneuver, "Orbit changes due to a planed maneuver"),
                        (PatchTransitionType.Collision, "Orbits collides with a (celestial) object"),
                        (PatchTransitionType.EndThrust, "End of thrust of a planed maneuver"),
                        (PatchTransitionType.PartialOutOfFuel, "Planed maneuver will partially run out of fuel"),
                        (PatchTransitionType.CompletelyOutOfFuel, "Planed maneuver will run out of fuel"),
                    }),
                });
        }
    }
}
