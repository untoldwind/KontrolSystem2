using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPAddons {
    [KSModule("ksp::addons", Description =
        @"Provides access to optional addons."
    )]
    public partial class KSPAddonsModule {
        [KSFunction(
            Description = @"Access the ""Flight Plan"" API (https://github.com/schlosrat/FlightPlan)
                Will be undefined if FlightPlan is not installed."
        )]
        public static Option<FlightPlanAdapter> FlightPlan() {
            var (instance, version) = KSPContext.CurrentContext.OptionalAddons.FlightPlan;

            if (instance != null)
                return new Option<FlightPlanAdapter>(new FlightPlanAdapter(instance, version));
            return new Option<FlightPlanAdapter>();
        }
    }
}
