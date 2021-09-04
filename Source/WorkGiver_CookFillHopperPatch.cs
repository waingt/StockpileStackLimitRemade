using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace StockpileLimit
{
    [HarmonyPatch(typeof(WorkGiver_CookFillHopper), nameof(WorkGiver_CookFillHopper.JobOnThing))]
    public class WorkGiver_CookFillHopperPatch
    {
        public static bool Prefix(ref Job __result, Pawn pawn, Thing thing)
        {
            __result = null;
            if (!(thing is ISlotGroupParent hopperSgp)) return false;

            var setting = hopperSgp.GetStoreSettings();
            var limit = setting.GetStacklimit();
            if (setting.IsRefillingDisabled() || limit == 0) return false;

            foreach (var thing2 in pawn.Map.thingGrid.ThingsListAt(thing.Position))
            {
                if (Building_NutrientPasteDispenser.IsAcceptableFeedstock(thing2.def))
                {
                    if ((thing2.stackCount * 100) >= ((limit < 0 ? thing2.def.stackLimit : limit) * setting.GetRefillPercent())) return false;
                }
            }
            return true;
        }
    }
}
