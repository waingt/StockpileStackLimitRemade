using Harmony;
using UnityEngine;
using Verse;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(ThingUtility), "TryAbsorbStackNumToTake")]
    public static class TryAbsorbStackNumToTakePatch
    {
        [HarmonyPriority(Priority.Low)]
        public static bool Prefix(ref int __result, Thing thing, Thing other, bool respectStackLimit)
        {
            if (respectStackLimit)
            {
                __result = Mathf.Min(other.stackCount, Limits.CalculateStackLimit(thing) - thing.stackCount);
            }
            else
                __result = other.stackCount;
            return false;
        }
    }
}
