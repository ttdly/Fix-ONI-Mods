
using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace FixPack {
    [ConfigFile("FixStoragePod.json", true, true)]
    [JsonObject(MemberSerialization.OptIn)]
    [RestartRequired]
    public class Option {
        [Option("STRINGS.UI.FIX_PACK.STORAGE_POD.OPTION1", "How many kg of Solids a Storage Pod can store.", Format = "F0")]
        [JsonProperty]
        public float PodCapacity { get; set; } = 5000f;

        [Option("STRINGS.UI.FIX_PACK.STORAGE_POD.OPTION2", "How many kg of Solids a Cool Pod can store.", Format = "F0")]
        [JsonProperty]
        public float CoolPodCapacity { get; set; } = 50f;

        [Option("STRINGS.UI.FIX_PACK.STORAGE_POD.OPTION3", "Can you store food in a Storage Pod?")]
        [JsonProperty]
        public bool PodStoresFood { get; set; } = false;

        [Option("STRINGS.UI.FIX_PACK.STORAGE_POD.OPTION3", "What is the cooling temperature in degrees below zero?", Format = "F0")]
        [JsonProperty]
        [Limit(0, 50)]
        public float CoolPodCapacityTemperature { get; set; } = 18f;
    }
}
