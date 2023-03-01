using System;
using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime {
    public delegate IAnyFuture Entrypoint(VesselComponent vessel);

    public static class KontrolModuleExtensions {
        private const string MainKsc = "main_ksc";
        private const string MainEditor = "main_editor";
        private const string MainTracking = "main_tracking";
        private const string MainFlight = "main_flight";

        private static Entrypoint GetEntrypoint(IKontrolModule module, string name, IKSPContext context) {
            try {
                IKontrolFunction function = module.FindFunction(name);
                if (function == null || !function.IsAsync) return null;

                if (function.Parameters.Count == 0) {
                    return _ => (IAnyFuture)function.Invoke(context);
                }

                if (function.Parameters.Count == 1 && function.Parameters[0].type.Name == "ksp::vessel::Vessel") {
                    return vessel =>
                        (IAnyFuture)function.Invoke(context, new KSPVesselModule.VesselAdapter(context, vessel));
                }

                context.Logger.Error($"GetEntrypoint {name} failed: Invalid parameters {function.Parameters}");
                return null;
            } catch (Exception e) {
                context.Logger.Error($"GetEntrypoint {name} failed: {e}");
                return null;
            }
        }

        private static bool HasEntrypoint(IKontrolModule module, string name, bool allowVessel) {
            IKontrolFunction function = module.FindFunction(name);
            return function != null && function.IsAsync &&
                   (function.Parameters.Count == 0 || allowVessel && function.Parameters.Count == 1 &&
                       function.Parameters[0].type.Name == "ksp::vessel::Vessel");
        }

        public static bool HasKSCEntrypoint(this IKontrolModule module) => HasEntrypoint(module, MainKsc, false);

        public static Entrypoint GetKSCEntrypoint(this IKontrolModule module, IKSPContext context) =>
            GetEntrypoint(module, MainKsc, context);

        public static bool HasEditorEntrypoint(this IKontrolModule module) => HasEntrypoint(module, MainEditor, false);

        public static Entrypoint GetEditorEntrypoint(this IKontrolModule module, IKSPContext context) =>
            GetEntrypoint(module, MainEditor, context);

        public static bool HasTrackingEntrypoint(this IKontrolModule module) =>
            HasEntrypoint(module, MainTracking, false);

        public static Entrypoint GetTrackingEntrypoint(this IKontrolModule module, IKSPContext context) =>
            GetEntrypoint(module, MainTracking, context);

        public static bool HasFlightEntrypoint(this IKontrolModule module) => HasEntrypoint(module, MainFlight, true);

        public static Entrypoint GetFlightEntrypoint(this IKontrolModule module, IKSPContext context) =>
            GetEntrypoint(module, MainFlight, context);

        public static bool IsBootFlightEntrypointFor(this IKontrolModule module, VesselComponent vessel) =>
            vessel != null && module.Name.ToLowerInvariant() == "boot::vessels::" + vessel.Name.ToLowerInvariant().Replace(' ', '_') &&
            HasEntrypoint(module, MainFlight, true);
    }
}
