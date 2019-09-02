using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;
using Verse.AI;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(HaulAIUtility), "HaulToCellStorageJob")]
    public static class HaulToCellStorageJobPatch
    {
        public static bool Prefix(Pawn p, Thing t, IntVec3 storeCell, bool fitInStoreCell, ref Job __result)
        {
            int limit = Limits.CalculateStackLimit(t);
            if (t.stackCount > limit)
            {
                Job job = new Job(JobDefOf.HaulToCell, t, storeCell);
                job.count = t.stackCount - limit;
                job.haulOpportunisticDuplicates = true;
                job.haulMode = HaulMode.ToCellStorage;
                __result = job;
                Mod.Debug($"dispatch job1, thing={t},cell={storeCell}");
                return false;
            }
            else
            {
                limit = Limits.CalculateStackLimit(p.Map.haulDestinationManager.SlotGroupAt(storeCell));
                if (limit >= 99999)
                {
                    Mod.Debug($"dispatch job3, thing={t},cell={storeCell}");
                    return true;
                }
                else
                {
                    Job job = new Job(JobDefOf.HaulToCell, t, storeCell);
                    Thing thing = p.Map.thingGrid.ThingAt(storeCell, t.def);
                    if (thing != null)
                    {
                        job.count = limit - thing.stackCount;
                    }
                    else
                        job.count = limit;
                    job.haulOpportunisticDuplicates = false;
                    job.haulMode = HaulMode.ToCellStorage;
                    __result = job;
                    Mod.Debug($"dispatch job2, thing={t},cell={storeCell}");
                    return false;
                }
            }
        }
    }
}
