using HarmonyLib;
using RimWorld;
using Verse;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(StoreUtility), nameof(StoreUtility.TryFindBestBetterStoreCellFor))]
    internal class FindBestBetterStoreCellPatcher
    {
        public static void Prefix(Thing t, Pawn carrier, Map map, ref StoragePriority currentPriority,
            ref IntVec3 foundCell)
        {
            if (t.stackCount > Limits.CalculateStackLimit(t))
                currentPriority = StoragePriority.Unstored;
        }
    }

    [HarmonyPatch(typeof(StoreUtility), "NoStorageBlockersIn")]
    public static class NoStorageBlockersInPatch
    {
        public static bool Postfix(bool _, IntVec3 c, Map map, Thing thing)
        {
            if (Limits.CalculateStackLimit(map, c) == 0) return false;
            foreach (var thing2 in map.thingGrid.ThingsListAt(c))
            {
                if (thing2.def.EverStorable(false))
                {
                    if (!thing2.CanStackWith(thing))
                    {
                        return false;
                    }
                    if (thing2.stackCount >= Limits.CalculateStackLimit(thing2))
                    {
                        return false;
                    }
                }
                if (thing2.def.entityDefToBuild != null && thing2.def.entityDefToBuild.passability != Traversability.Standable)
                {
                    return false;
                }
                if (thing2.def.surfaceType == SurfaceType.None && thing2.def.passability != Traversability.Standable)
                {
                    return false;
                }
            }
            return true;
        }
    }
}