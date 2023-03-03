using KontrolSystem.TO2;
using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime {
    public static class KontrolSystemKSPRegistry {
        public static KontrolRegistry CreateKSP() {
            KontrolRegistry registry = KontrolRegistry.CreateCore();

            registry.RegisterModule(KSPMathModule.Instance.module);

            KSPVessel.KSPVesselModule.DirectBindings();
            registry.RegisterModule(
                BindingGenerator.BindModule(typeof(KSPConsole.KSPConsoleModule)));
            registry.RegisterModule(
                BindingGenerator.BindModule(typeof(KSPGame.KSPGameModule)));
/*            registry.RegisterModule(
                BindingGenerator.BindModule(typeof(KSPGame.KSPGameWarpModule)));
            registry.RegisterModule(
                BindingGenerator.BindModule(typeof(KSPOrbit.KSPOrbitModule)));
            registry.RegisterModule(
                BindingGenerator.BindModule(typeof(KSPControl.KSPControlModule)));*/
            registry.RegisterModule(
                BindingGenerator.BindModule(typeof(KSPVessel.KSPVesselModule)));
/*            registry.RegisterModule(BindingGenerator.BindModule(typeof(KSPUI.KSPUIModule)));
            registry.RegisterModule(
                BindingGenerator.BindModule(typeof(KSPDebug.KSPDebugModule)));
            registry.RegisterModule(BindingGenerator.BindModule(typeof(KSPResource.KSPResourceModule)));
            registry.RegisterModule(BindingGenerator.BindModule(typeof(KSPAddons.KSPAddonsModule)));
            registry.RegisterModule(BindingGenerator.BindModule(typeof(KSPGame.KSPAlarmClockModule)));
            registry.RegisterModule(BindingGenerator.BindModule(typeof(Testing.KSPTesting)));
*/
            return registry;
        }
    }
}
