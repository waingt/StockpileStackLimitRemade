using HarmonyLib;
using RimWorld;
using Verse;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(ListerMergeables), "ShouldBeMergeable")]
    public class ShouldBeMergeablePatch
    {
        private static void Postfix(Thing t, ref bool __result)
        {
            if (__result)
            {
                __result = !Limits.HasStackLimit(t) && t.stackCount != Limits.CalculateStackLimit(t);
            }
            //if (t.IsForbidden(Faction.OfPlayer))
            //{
            //    __result = false;
            //}
            //else if (!Limits.HasStackLimit(t))
            //{
            //    __result = false;
            //}
            //else if (t.stackCount == Limits.CalculateStackLimit(t))
            //{
            //    __result = false;
            //}
            //else
            //    __result = true;
            //return false;
        }
    }
}