using HarmonyLib;
using Klei.CustomSettings;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;

namespace FixStartWithAllResearch {
    public sealed class Mod : KMod.UserMod2 {
        private static SettingConfig _startWithAllResearch;
        private static Harmony _harmony;

        
        
        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
            _harmony = harmony;
            PUtil.InitLibrary();
            new PVersionCheck().Register(this, new SteamVersionChecker());
        }

       
       


        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        public class GeneratedBuildingsLoadGeneratedBuildingsPatch {
            public static void Postfix() {
                _harmony.Patch(AccessTools.Method(typeof(CustomGameSettings), "OnPrefabInit"), 
                    postfix: new HarmonyMethod(AccessTools.Method(typeof(CustomGameSettingsOnPrefabInitPatch), "Postfix")));
            }
        }
        
        
        private class CustomGameSettingsOnPrefabInitPatch {
            public static void Postfix() {
                if (_startWithAllResearch == null) {
                    var label = "Start with all Research";
                    var tooltip = "When active will start a save with all research nodes completed/researched.";
                    var tooltipDis = "Unchecked: Start with all Research is turned off (Default)";
                    var tooltipAct = "Checked: Start with all Research is turned on";
                    if (Localization.GetCurrentLanguageCode() == "zh_klei") {
                        label = "全科技开局";
                        tooltip = "启用时，开始游戏时所有科技都会被解锁";
                        tooltipDis = "不选中：不会解锁任何科技（默认）";
                        tooltipAct = "选中：开始游戏时，所有科技会被解锁"; 
                    }
                    _startWithAllResearch = new ToggleSettingConfig(
                        id: "StartWithAllResearch",
                        label: label,
                        tooltip: tooltip,
                        off_level: new SettingLevel("Disabled", "Disabled", tooltipDis, 0, null),
                        on_level: new SettingLevel("Enabled", "Enabled", tooltipAct, 0, (object)null),
                        default_level_id: "Disabled",
                        nosweat_default_level_id: "Disabled",
                        debug_only: false
                    );
                };
               
                
                CustomGameSettings.Instance.AddQualitySettingConfig(_startWithAllResearch);
            }
        }

        [HarmonyPatch(typeof(ResearchScreen), "OnSpawn")]
        public class Game_OnSpawn_Patch {
            public static void Postfix() {
                if (CustomGameSettings.Instance.GetCurrentQualitySetting(_startWithAllResearch).id != "Enabled")
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
                if (CustomGameSettings.Instance.GetCurrentQualitySetting(_startWithAllResearch).id == "Enabled")
                    notify = false;
            }
        }
    }
}
