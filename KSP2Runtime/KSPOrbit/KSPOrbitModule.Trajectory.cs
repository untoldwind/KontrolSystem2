using System.Collections;
using System.Collections.Generic;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim;
using KSP.Sim.impl;
using UniLinq;

namespace KontrolSystem.KSP.Runtime.KSPOrbit;

public partial class KSPOrbitModule {
    [KSClass(Description = "Representation of a trajectory of a vessel that might has multiple orbit patches")]
    public class Trajectory(IKSPContext context, PatchedConicsOrbit[] patches) : IArrayLike<OrbitPatch> {
        [KSField(Description = "Universal time of the start of the trajectory")]
        public double StartUt => patches.Length == 0 ? 0.0 : patches.Select(patch => patch.StartUT).Min();

        [KSField(Description = "Universal time of the end of the trajectory")]
        public double EndUt => patches.Length == 0 ? 0.0 : patches.Select(patch => patch.EndUT).Max();

        [KSMethod(Description = "Find orbit patch for a given universal time `ut`")]
        public Option<OrbitPatch> FindPatch(double ut) {
            foreach (var patch in patches) {
                if (ut >= patch.StartUT &&
                    (ut <= patch.StartUT || patch.PatchEndTransition == PatchTransitionType.Final)) {
                    return Option.Some(new OrbitPatch(context, this, patch));
                }
            }

            return Option.None<OrbitPatch>();
        }

        public OrbitPatch GetElement(long index) => new(context, this, patches[(int)index]);

        public long Length => patches.Length;

        public IEnumerator<OrbitPatch> GetEnumerator() => patches.Select(patch => new OrbitPatch(context, this, patch)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
