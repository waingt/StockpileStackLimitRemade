using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(Thing), nameof(Thing.TryAbsorbStack))]
    class ThingPatch
    {
        public static bool Prefix(Thing __instance, Thing other, bool respectStackLimit, ref bool __result)
        {
            Mod.Debug("Thing.TryAbsorbStack begin");
            if (Limits.HasStackLimit(__instance))
            {
                if (!__instance.CanStackWith(other))
                {
                    __result = false; return false;
                }
                int num;
                if (respectStackLimit)
                {
                    var t = Limits.CalculateStackLimit(__instance) - __instance.stackCount;
                    if (t < 0)
                    {
                        t = 0;
                    }

                    num = Mathf.Min(other.stackCount, t);
                }
                else
                {
                    num = other.stackCount;
                }

                if (num <= 0) { __result = false; return false; }
                if (__instance.def.useHitPoints)
                {
                    __instance.HitPoints = Mathf.CeilToInt(((__instance.HitPoints * __instance.stackCount) + (other.HitPoints * num)) / (float)(__instance.stackCount + num));
                }
                __instance.stackCount += num;
                other.stackCount -= num;
                StealAIDebugDrawer.Notify_ThingChanged(__instance);
                if (__instance.Spawned)
                {
                    __instance.Map.listerMergeables.Notify_ThingStackChanged(__instance);
                }
                if (other.stackCount <= 0)
                {
                    other.Destroy();
                    __result = true;
                }
                else
                {
                    __result = false;
                }

                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
