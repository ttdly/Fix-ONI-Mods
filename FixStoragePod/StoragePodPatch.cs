using HarmonyLib;
using static SkyLib.OniUtils;

namespace FixPack.StoragePod {
    public class StoragePodPatch {
        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public static class Db_Initialize_Patch {
            public static void Prefix() {
                AddBuildingStrings(
                    StoragePodConfig.ID,
                    StoragePodConfig.DisplayName,
                    StoragePodConfig.Description,
                    StoragePodConfig.Effect);
                AddBuildingStrings(
                    CoolPodConfig.ID,
                    CoolPodConfig.DisplayName,
                    CoolPodConfig.Description,
                    CoolPodConfig.Effect);
            }

            public static void Postfix() {
                AddBuildingToBuildMenu("Base", StoragePodConfig.ID);
                AddBuildingToBuildMenu("Food", CoolPodConfig.ID);
                AddBuildingToTech("RefinedObjects", StoragePodConfig.ID);
                AddBuildingToTech("Agriculture", CoolPodConfig.ID);
            }
        }
    }
}
