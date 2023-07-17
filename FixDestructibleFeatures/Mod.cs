using HarmonyLib;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using UnityEngine;

namespace FixPack {
    public sealed class Mod : KMod.UserMod2 {
        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            new PVersionCheck().Register(this, new SteamVersionChecker());
        }

        [HarmonyPatch(typeof(GeyserGenericConfig),nameof(GeyserGenericConfig.CreateGeyser))]
        public class GeyserGenericConfig_CreateGeyser_Patch {
            public static void Postfix(GameObject __result) {
                __result.AddOrGet<Demolishable>().workTime = 1500f;
            }
        }

        [HarmonyPatch(typeof(OilWellConfig),nameof(OilWellConfig.CreatePrefab))]
        public class OilWellConfig_CreatePrefab_Patch {
            public static void Postfix(GameObject __result) {
                __result.AddOrGet<Demolishable>().SetWorkTime(1500f);
            }
        }
    }
}
