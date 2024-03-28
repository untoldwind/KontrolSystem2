using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPOrbit;

public partial class KSPOrbitModule {
    [KSClass(Description = "Represents a orbit patch of a trajectory", ScanInterfaces = [typeof(IOrbit)])]
    public class OrbitPatch(IKSPContext context, Trajectory trajectory, PatchedConicsOrbit orbit) : OrbitWrapper(context, orbit) {
        [KSField(Description = "The trajectory this orbit patch belongs to")]
        public Trajectory Trajectory => trajectory;

        [KSField(Description = "Universal time of the start of the orbit patch")]
        public double StartUt => orbit.StartUT;

        [KSField(Description = "Universal time of the start of the orbit patch")]
        public double EndUt => orbit.EndUT;

        [KSField(Description = "Get transition type at the beginning of the orbit patch")]
        public PatchTransitionType StartTransition => orbit.PatchStartTransition;

        [KSField(Description = "Get transition type at the end of the orbit patch")]
        public PatchTransitionType EndTransition => orbit.PatchEndTransition;

        [KSField(Description = "Get the previous orbit patch of the trajectory (if available)")]
        public Option<OrbitPatch> PreviousPatch {
            get {
                var previous = orbit.PreviousPatch;

                return previous is { ActivePatch: true } && previous is PatchedConicsOrbit prevOrbit
                    ? Option.Some(new OrbitPatch(context, trajectory, prevOrbit))
                    : Option.None<OrbitPatch>();
            }
        }

        [KSField(Description = "Get the next orbit patch of the trajectory (if available)")]
        public Option<OrbitPatch> NextPatch {
            get {
                var next = orbit.NextPatch;

                return next is { ActivePatch: true } && next is PatchedConicsOrbit nextOrbit
                    ? Option.Some(new OrbitPatch(context, trajectory, nextOrbit))
                    : Option.None<OrbitPatch>();
            }
        }

    }
}
