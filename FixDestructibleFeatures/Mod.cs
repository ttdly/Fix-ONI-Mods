using System;
using HarmonyLib;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using UnityEngine;

namespace FixDestructibleFeatures {
  public sealed class Mod : KMod.UserMod2 {
    public override void OnLoad(Harmony harmony) {
      base.OnLoad(harmony);
      PUtil.InitLibrary();
      new PVersionCheck().Register(this, new SteamVersionChecker());
    }

    [HarmonyPatch(typeof(GeyserGenericConfig))]
    [HarmonyPatch(nameof(GeyserGenericConfig.CreateGeyser))]
    [HarmonyPatch(new Type[] {
      typeof(string), typeof(string), typeof(int), typeof(int), typeof(string), typeof(string),
      typeof(HashedString), typeof(float), typeof(string[]), typeof(string[])
    })]
    public class GeyserGenericConfig_CreateGeyser_Patch {
      public static void Postfix(GameObject __result) {
        var demolishAbleComponent = __result.AddOrGet<Demolishable>();
        demolishAbleComponent.SetWorkTime(1500f);
        // demolishAbleComponent.OnWorkableEventCB += (workable, @event) => {
        //   if (@event != Workable.WorkableEvent.WorkCompleted) return;
        // }; TODO replace uno
      }
    }

    [HarmonyPatch(typeof(OilWellConfig), nameof(OilWellConfig.CreatePrefab))]
    public class OilWellConfig_CreatePrefab_Patch {
      public static void Postfix(GameObject __result) {
        __result.AddOrGet<Demolishable>().SetWorkTime(1500f);
      }
    }
  }
}