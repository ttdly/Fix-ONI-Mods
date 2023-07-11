using HarmonyLib;
using Klei.CustomSettings;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;

namespace FixStartWithAllResearch {
    public sealed class Mod : KMod.UserMod2 {
        private static SettingConfig StartWithAllResearch;

        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            new PVersionCheck().Register(this, new SteamVersionChecker());
            StartWithAllResearch = (SettingConfig)new ToggleSettingConfig(
                id: "StartWithAllResearch",
                label: "Start with all Research",
                tooltip: "When active will start a save with all research nodes completed/researched.",
                off_level: new SettingLevel("Disabled", "Disabled", "Unchecked: Start with all Research is turned off (Default)", 0, null),
                on_level: new SettingLevel("Enabled", "Enabled", "Checked: Start with all Research is turned on", 0, (object)null),
                default_level_id: "Disabled",
                nosweat_default_level_id: "Disabled",
                coordinate_dimension: -1,
                coordinate_dimension_width: -1,
                debug_only: false
                );
        }


        [HarmonyPatch(typeof(CustomGameSettings), "OnPrefabInit")]
        public class CustomGameSettings_OnPrefabInit_Patch {
            public static void Postfix(CustomGameSettings __instance) {
                __instance.AddQualitySettingConfig(StartWithAllResearch);
            }
        }

        [HarmonyPatch(typeof(ResearchScreen), "OnSpawn")]
        public class Game_OnSpawn_Patch {
            public static void Postfix() {
                if (CustomGameSettings.Instance.GetCurrentQualitySetting(StartWithAllResearch).id != "Enabled")
                    return;

                foreach (Tech tech in Db.Get().Techs.resources) {
                    if (!tech.IsComplete()) {
                        TechInstance ti = Research.Instance.Get(tech);
                        ti.Purchased();
                        //if (ti.tech!= null) Game.Instance.Trigger(-107300940, ti.tech);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(ResearchEntry), "ResearchCompleted")]
        public class ResearchEntry_ResearchCompleted_Patch {
            public static void Prefix(ref bool notify) {
                if (CustomGameSettings.Instance.GetCurrentQualitySetting(StartWithAllResearch).id == "Enabled")
                    notify = false;
            }
        }
    }
}
