using System;
using System.IO;
using KSP.Game;
using SpaceWarp.API;
using SpaceWarp.API.Configuration;
using SpaceWarp.API.Managers;

namespace KontrolSystem.SpaceWarpMod {
    public class ConfigAdapter {
        public static readonly string MOD_ID = "kontrol_system";
        private static KontrolSystemConfig cached = null;
        
        public static KontrolSystemConfig Config {
            get {
                if (cached != null) return cached;
                
                KontrolSystemConfig config = getFromManager();
                
                if (config.stdLibPath == null || config.stdLibPath.Length == 0) {
                    config.stdLibPath = Path.Combine(SpaceWarpManager.MODS_FULL_PATH, "KontrolSystem2", "stdLib");
                    updateInManager();
                }
                if (config.to2Path == null || config.to2Path.Length == 0) {
                    config.to2Path = Path.Combine(SpaceWarpManager.MODS_FULL_PATH, "KontrolSystem2", "to2");
                    updateInManager();
                }

                cached = config;

                return config;
            }
        }

        private static KontrolSystemConfig getFromManager() {
            if (ManagerLocator.TryGet(out ConfigurationManager configurationManager)) {
                if (configurationManager.TryGet(MOD_ID, out (Type, object, string) managedConfig)) {
                    return (KontrolSystemConfig)managedConfig.Item2;
                }
            }

            return new KontrolSystemConfig();
        }

        private static void updateInManager() {
            if (ManagerLocator.TryGet(out ConfigurationManager configurationManager)) {
                configurationManager.UpdateConfiguration(MOD_ID);
            }
        }
    }
}
