using System.Collections;
using System.Collections.Generic;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;
using UniLinq;

namespace KontrolSystem.KSP.Runtime.KSPOrbit;

public partial class KSPOrbitModule {
    [KSClass(Description = "Representation of a trajectory of a vessel that might has multiple orbit patches")]
    public class Trajectory : IArrayLike<OrbitPatch> {
        private readonly List<OrbitPatch> orbitPatches;

        public Trajectory(IKSPContext context, IEnumerable<PatchedConicsOrbit> patches) {
            orbitPatches = patches.Select(item => new OrbitPatch(context, this, item)).ToList();
        }

        [KSField(Description = "Universal time of the start of the trajectory")]
        public double StartUt => orbitPatches.Count == 0 ? 0.0 : orbitPatches.Select(patch => patch.StartUt).Min();

        [KSField(Description = "Universal time of the end of the trajectory")]
        public double EndUt => orbitPatches.Count == 0 ? 0.0 : orbitPatches.Select(patch => patch.EndUt).Max();
        
        public OrbitPatch GetElement(long index) => orbitPatches[(int)index];

        public long Length => orbitPatches.Count;

        public IEnumerator<OrbitPatch> GetEnumerator() => orbitPatches.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
