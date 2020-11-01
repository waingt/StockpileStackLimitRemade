using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using RimWorld;
using Verse;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(MassUtility), "CountToPickUpUntilOverEncumbered")]
    class MassUtilityPatch
    {
        static void Postfix(Pawn pawn, Thing thing, ref int __result)
        {
            if (Limits.HasStackLimit(thing))
            {
                var t = thing.stackCount - Limits.CalculateStackLimit(thing);
                if (t < 0)
                {
                    t = 0;
                }

                __result = Mathf.Min(t, __result);
            }
        }
    }
}
