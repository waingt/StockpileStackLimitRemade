using Harmony;
using RimWorld;
using Verse;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(ListerMergeables), "ShouldBeMergeable")]
    public class ShouldBeMergeablePatch
    {
        static bool Prefix(Thing t, ref bool __result)
        {
            if (t.IsForbidden(Faction.OfPlayer))
            {
                __result = false;
            }
            else if (!Limits.HasStackLimit(t))
            {
                __result = false;
            }
            else if (t.stackCount == Limits.CalculateStackLimit(t))
            {
                __result = false;
            }
            else
                __result = true;
            return false;
        }
    }
}
