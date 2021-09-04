using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace StockpileLimit
{
    [HarmonyPatch]
    class JobDriver_HaulToInventory_MakeNewToilsPatch
    {
        static bool Prepare() => ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name == "Pick Up And Haul");
        static MethodBase TargetMethod()
        {
            var lambda_method = AccessTools.Method("PickUpAndHaul.JobDriver_HaulToInventory+<>c__DisplayClass1_0:<MakeNewToils>b__2");
            if (lambda_method == null) Utilities.Error("Cannot find lambda func in JobDriver_HaulToInventory.MakeNewToils");
            else Utilities.Debug($"{lambda_method.DeclaringType.FullName}::{lambda_method.Name}");
            return lambda_method;
        }
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var method = SymbolExtensions.GetMethodInfo(() => MassUtility.CountToPickUpUntilOverEncumbered(null, null));
            var method2 = SymbolExtensions.GetMethodInfo(() => CountToPickUpUntilOverEncumberedHooker(null, null));
            var found = false;
            instructions.Where(i => i.Calls(method)).Foreach(i => { found = true; i.operand = method2; });
            if (found) Utilities.Debug("JobDriver_HaulToInventory.MakeNewToils is patched");
            else Utilities.Error("Cannot find <Call MassUtility.CountToPickUpUntilOverEncumbered> in JobDriver_HaulToInventory.MakeNewToils");
            return instructions;
        }
        public static int CountToPickUpUntilOverEncumberedHooker(Pawn pawn, Thing thing)
        {
            int result = MassUtility.CountToPickUpUntilOverEncumbered(pawn, thing);
            int limit = AdditionalStorageSettings.CalculateStackLimit(thing.Map, thing.Position);
            if (thing.stackCount <= limit) return result;
            int num_can_pick = thing.stackCount - limit;
            if (result > num_can_pick) result = num_can_pick;
            return result;
        }
    }
}
