using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(StoreUtility), "NoStorageBlockersIn")]
    public static class NoStorageBlockersInPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            return new TranspilerFactory().Replace("ldarg.2;ldfld *;ldfld *", "ldloc.2;call Limits::CalculateStackLimit(Verse.Thing)").Transpiler(generator, instructions);
        }
    }

    [HarmonyPatch(typeof(StoreUtility), "TryFindBestBetterStoreCellFor")]
    class FindBestBetterStoreCellPatcher
    {
        public static void Prefix(Thing t, Pawn carrier, Map map, ref StoragePriority currentPriority, ref IntVec3 foundCell)
        {
            if (t.stackCount > Limits.CalculateStackLimit(t))
            {
                currentPriority = StoragePriority.Unstored;
            }
        }
    }
}
