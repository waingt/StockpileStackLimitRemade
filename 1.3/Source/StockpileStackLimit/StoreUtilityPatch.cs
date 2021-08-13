using HarmonyLib;
using RimWorld;
using Verse;

namespace StockpileLimit
{
    [HarmonyPatch(typeof(StoreUtility), nameof(StoreUtility.TryFindBestBetterStoreCellFor))]
    internal class FindBestBetterStoreCellPatcher
    {
        public static void Prefix(Thing t, Pawn carrier, Map map, ref StoragePriority currentPriority,
            ref IntVec3 foundCell)
        {
            if (t.stackCount > AdditionalStorageSettings.CalculateStackLimit(t))
                currentPriority = StoragePriority.Unstored;
        }
    }

    [HarmonyPatch(typeof(StoreUtility), "NoStorageBlockersIn")]
    public static class NoStorageBlockersInPatch
    {
        public static bool Postfix(bool _, IntVec3 c, Map map, Thing thing)
        {
            var setting = c.GetSlotGroup(map).Settings;
            var limit = setting.GetStacklimit();
            if (setting.IsRefillingDisabled() || limit == 0) return false;
            if (limit < 0) limit = thing.def.stackLimit;
            limit = limit * setting.GetRefillPercent();
            foreach (var thing2 in map.thingGrid.ThingsListAt(c))
            {
                if (thing2.def.EverStorable(false))
                {
                    if (!thing2.CanStackWith(thing)) return false;
                    if (thing2.stackCount * 100 >= limit) return false;
                }
                if (thing2.def.entityDefToBuild != null && thing2.def.entityDefToBuild.passability != Traversability.Standable) return false;
                if (thing2.def.surfaceType == SurfaceType.None && thing2.def.passability != Traversability.Standable) return false;
            }
            return true;
        }
    }
}