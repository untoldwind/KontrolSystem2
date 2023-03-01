using SpaceWarp.API.Configuration;
using Newtonsoft.Json;

namespace KontrolSystem.SpaceWarpMod {
    [JsonObject(MemberSerialization.OptOut)]
    [ModConfig]
    public class KontrolSystemConfig {
        [ConfigField("pi")] 
        [ConfigDefaultValue(3.14159)]
        public double pi;
    }
}
