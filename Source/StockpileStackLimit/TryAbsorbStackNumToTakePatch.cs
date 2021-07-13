using HarmonyLib;
using UnityEngine;
using Verse;

namespace StockpileStackLimit
{
    //[HarmonyPatch(typeof(ThingUtility), "TryAbsorbStackNumToTake")]
    public static class TryAbsorbStackNumToTakePatch
    {
        [HarmonyPriority(Priority.Low)]
        public static bool Prefix(ref int __result, Thing thing, Thing other, bool respectStackLimit)
        {
            if (respectStackLimit)
            {
                var t = Limits.CalculateStackLimit(thing) - thing.stackCount;
                if (t < 0)
                {
                    t = 0;
                }

                __result = Mathf.Min(other.stackCount, t);
            }
            else
            {
                __result = other.stackCount;
            }

            return false;
        }
    }
}