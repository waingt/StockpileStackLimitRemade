using HarmonyLib;
using RimWorld;
using Verse;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(StoreUtility), "TryFindBestBetterStoreCellFor")]
    internal class FindBestBetterStoreCellPatcher
    {
        public static void Prefix(Thing t, Pawn carrier, Map map, ref StoragePriority currentPriority,
            ref IntVec3 foundCell)
        {
            if (t.stackCount > Limits.CalculateStackLimit(t))
            {
                currentPriority = StoragePriority.Unstored;
            }
        }
    }
}