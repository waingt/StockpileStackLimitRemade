using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using Verse.AI;

namespace StockpileLimit
{
    [HarmonyPatch]
    public static class PlaceHauledThingInCellPatch
    {
        static MethodBase TargetMethod()
        {
            var lambda_method = typeof(Toils_Haul).Assembly.GetTypes().Where(t => t.FullName.Contains("Toils_Haul") && t.FullName.Contains("DisplayClass")).SelectMany(t => t.GetMethods(AccessTools.allDeclared)).FirstOrDefault(m => m.Name.Contains("PlaceHauledThingInCell"));
            if (lambda_method == null) Utilities.Error("Cannot find lambda func in Verse.Toils_Haul::PlaceHauledThingInCell");
            else Utilities.Debug($"{lambda_method.DeclaringType.FullName}::{lambda_method.Name}");
            return lambda_method;
        }
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Thing t;
            var method = SymbolExtensions.GetMethodInfo((Pawn_CarryTracker tracker) => tracker.TryDropCarriedThing(default, 0, out t, null));
            var method2 = SymbolExtensions.GetMethodInfo(() => TryDropCarriedThingHooker(null, default, 0, out t, null));
            var found = false;
            instructions.Where(i => i.Calls(method)).Foreach(i => { found = true; i.operand = method2; });
            if (found) Utilities.Debug("Toils_Haul.PlaceHauledThingInCell is patched");
            else Utilities.Error("Cannot find <Call Pawn_CarryTracker.TryDropCarriedThing> in Toils_Haul.PlaceHauledThingInCell");
            return instructions;
        }

        public static bool TryDropCarriedThingHooker(Pawn_CarryTracker this_, IntVec3 dropLoc, ThingPlaceMode mode, out Thing resultingThing, Action<Thing, int> placedAction)
        {
            var map = this_.pawn.MapHeld;
            var limit = AdditionalStorageSettings.CalculateStackLimit(map, dropLoc);
            if (limit < AdditionalStorageSettings.max_limit)
            {
                var carried_thing = this_.CarriedThing;
                int count = map.thingGrid.ThingAt(dropLoc, carried_thing.def)?.stackCount ?? 0;
                var num_can_drop = limit - count;
                if (count >= limit) Utilities.Error("Try drop carried thing in overlimited cell???");
                else if (num_can_drop < carried_thing.stackCount)
                {
                    this_.innerContainer.TryDrop(carried_thing, dropLoc, map, mode, num_can_drop, out resultingThing, placedAction, null);
                    return false;
                }
            }
            return this_.TryDropCarriedThing(dropLoc, mode, out resultingThing, placedAction);
        }
    }
}
