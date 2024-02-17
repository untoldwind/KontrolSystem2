using KontrolSystem.KSP.Runtime.KSPAddons;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPControl;
using KontrolSystem.KSP.Runtime.KSPDebug;
using KontrolSystem.KSP.Runtime.KSPGame;
using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.KSP.Runtime.KSPResource;
using KontrolSystem.KSP.Runtime.KSPScience;
using KontrolSystem.KSP.Runtime.KSPTelemetry;
using KontrolSystem.KSP.Runtime.KSPUI;
using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.KSP.Runtime.Testing;
using KontrolSystem.TO2;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime;

public static class KontrolSystemKSPRegistry {
    public static KontrolRegistry CreateKSP() {
        var registry = KontrolRegistry.CreateCore();

        registry.RegisterModule(KSPMathModule.Instance.module);

        var (orbitTypes, orbitConstants) = KSPOrbitModule.DirectBindings();
        var (vesselTypes, vesselConstants) = KSPVesselModule.DirectBindings();
        var (resourceTypes, resourceConstants) = KSPResourceModule.DirectBindings();
        var (scienceTypes, scienceConstants) = KSPScienceModule.DirectBindings();
        var (uiTypes, ruiConstants) = KSPUIModule.DirectBindings();
        registry.RegisterModule(
            BindingGenerator.BindModule(typeof(KSPConsoleModule)));
        registry.RegisterModule(
            BindingGenerator.BindModule(typeof(KSPOrbitModule), orbitTypes, orbitConstants));
        registry.RegisterModule(
            BindingGenerator.BindModule(typeof(KSPControlModule)));
        registry.RegisterModule(
            BindingGenerator.BindModule(typeof(KSPResourceModule), resourceTypes, resourceConstants));
        registry.RegisterModule(
            BindingGenerator.BindModule(typeof(KSPScienceModule), scienceTypes, scienceConstants));
        registry.RegisterModule(
            BindingGenerator.BindModule(typeof(KSPVesselModule), vesselTypes, vesselConstants));
        registry.RegisterModule(
            BindingGenerator.BindModule(typeof(KSPGameModule)));
        registry.RegisterModule(
            BindingGenerator.BindModule(typeof(KSPGameWarpModule)));
        registry.RegisterModule(
            BindingGenerator.BindModule(typeof(KSPDebugModule)));
        registry.RegisterModule(
            BindingGenerator.BindModule(typeof(KSPTelemetryModule)));
        registry.RegisterModule(
            BindingGenerator.BindModule(typeof(KSPAddonsModule)));
        registry.RegisterModule(BindingGenerator.BindModule(typeof(KSPUIModule), uiTypes, ruiConstants));
        registry.RegisterModule(BindingGenerator.BindModule(typeof(KSPTesting)));

        return registry;
    }
}
