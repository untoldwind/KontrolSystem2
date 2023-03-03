using SpaceWarp.API.Configuration;
using Newtonsoft.Json;

namespace KontrolSystem.SpaceWarpMod {
    [JsonObject(MemberSerialization.OptOut)]
    [ModConfig]
    public class KontrolSystemConfig {
        [ConfigSection("Paths")]
        [ConfigField("stdLibPath")] 
        public string stdLibPath;
        
        [ConfigSection("Paths")]
        [ConfigField("to2Path")] 
        public string to2Path;
    }
}
