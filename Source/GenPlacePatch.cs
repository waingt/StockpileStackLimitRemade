using Harmony;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(GenPlace), "PlaceSpotQualityAt")]
    static class PlaceSpotQualityAtPatch
    {
#if DEBUG
        static bool Prepare() => HarmonyInstance.DEBUG = true;
        static bool Cleanup() => HarmonyInstance.DEBUG = false;
#endif
        static IEnumerable<CodeInstruction> Transpiler(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        => new TranspilerFactory().Search("ret", 2).Insert("localvar int;ldarg.1;ldarg.2;call Limits::CalculateStackLimit(Verse.Map,Verse.IntVec3);stloc.s 0")
            .Replace("ldarg.2;ldfld Verse.Thing::def;ldfld Verse.ThingDef::stackLimit", "ldloc.s 0;ldloc.s 0;brtrue 0;pop;ldarg.2;ldfld Verse.Thing::def;ldfld Verse.ThingDef::stackLimit;label 0")
            .Replace("ldarg.2;ldfld Verse.Thing::def;ldfld Verse.ThingDef::stackLimit", "ldloc.s 0;ldloc.s 0;brtrue 1;pop;ldarg.2;ldfld Verse.Thing::def;ldfld Verse.ThingDef::stackLimit;label 1")
            .Transpiler(generator, instructions);
    }
}
