using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(HaulAIUtility), nameof(HaulAIUtility.HaulToCellStorageJob))]
    public static class HaulToCellStorageJobPatch
    {
        static void Postfix(ref Job __result, Pawn p, Thing t, IntVec3 storeCell, bool fitInStoreCell)
        {
            var temp = t.stackCount - Limits.CalculateStackLimit(t);
            if (temp > 0)
            {
                __result.count = temp;
                __result.haulOpportunisticDuplicates = false;
                Main.Debug($"clamp down hauling job count due to overlimit,job={__result},pawn={p},thing={t},count={__result.count}");
                return;
            }
            var slotGroup = p.Map.haulDestinationManager.SlotGroupAt(storeCell);
            int limit;
            if (slotGroup != null && Limits.TryGetLimit(slotGroup, out limit))
            {
                limit = Mathf.Min(limit, t.def.stackLimit);
                int count = 0;
                var statValue = p.GetStatValue(StatDefOf.CarryingCapacity, true);
                foreach (var cell in slotGroup.CellsList)
                {
                    if (StoreUtility.IsGoodStoreCell(cell, p.Map, t, p, p.Faction))
                    {
                        Thing thing2 = p.Map.thingGrid.ThingAt(cell, t.def);
                        count += thing2 != null ? Mathf.Max(limit, -thing2.stackCount, 0) : limit;
                        if (count >= statValue)
                        {
                            count = (int)statValue;
                            break;
                        }
                    }
                }
                __result.count = limit;
                Main.Debug($"destination has a limit,recalculating job count,job={__result},pawn={p},thing={t},count={__result.count}");
            }
        }
    }
}