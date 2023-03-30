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
    }
}
