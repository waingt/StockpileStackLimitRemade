using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(StoreUtility), "NoStorageBlockersIn")]
    public static class NoStorageBlockersInPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(ILGenerator generator,
            IEnumerable<CodeInstruction> instructions)
        {
            return new TranspilerFactory()
                .Replace("ldarg.2;ldfld *;ldfld *", "ldloc.2;call Limits::CalculateStackLimit(Verse.Thing)")
                .Transpiler(generator, instructions);
        }
    }
}