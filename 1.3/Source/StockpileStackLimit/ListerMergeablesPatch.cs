using HarmonyLib;
using RimWorld;
using Verse;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(ListerMergeables), "ShouldBeMergeable")]
    public class ShouldBeMergeablePatch
    {
        static void Postfix(Thing t, ref bool __result)
        {
            __result = !t.IsForbidden(Faction.OfPlayer) && t.GetSlotGroup() != null && t.stackCount != AdditionalStorageSettings.CalculateStackLimit(t);
        }
    }
}