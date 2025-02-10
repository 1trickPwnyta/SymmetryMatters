using HarmonyLib;
using RimWorld;
using Verse;

namespace SymmetryMatters
{
    [HarmonyPatch(typeof(RoomStatWorker_Impressiveness))]
    [HarmonyPatch(nameof(RoomStatWorker_Impressiveness.GetScore))]
    public static class Patch_RoomStatWorker_Impressiveness
    {
        public static void Postfix(Room room, ref float __result)
        {
            __result += room.GetStat(DefDatabase<RoomStatDef>.GetNamed("Symmetry")) * SymmetryMattersSettings.RoomImpressivenessEffect;
        }
    }
}
