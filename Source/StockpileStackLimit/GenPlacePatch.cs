using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace StockpileStackLimit
{
    /*
     * yea, Tynan made me completely confused.
     */
    //[HarmonyPatch(typeof(GenPlace), "PlaceSpotQualityAt")]
    static class PlaceSpotQualityAtPatch
    {/*
#if DEBUG
        static bool Prepare() => HarmonyLibInstance.DEBUG = true;
        static bool Cleanup() => HarmonyLibInstance.DEBUG = false;
#endif
        static IEnumerable<CodeInstruction> Transpiler(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        => new TranspilerFactory().Search("ret", 2).Insert("localvar int;ldarg.1;ldarg.2;call Limits::CalculateStackLimit(Verse.Map,Verse.IntVec3);stloc.s 0")
            .Replace("ldarg.2;ldfld Verse.Thing::def;ldfld Verse.ThingDef::stackLimit", "ldloc.s 0;ldloc.s 0;brtrue 0;pop;ldarg.2;ldfld Verse.Thing::def;ldfld Verse.ThingDef::stackLimit;label 0")
            .Replace("ldarg.2;ldfld Verse.Thing::def;ldfld Verse.ThingDef::stackLimit", "ldloc.s 0;ldloc.s 0;brtrue 1;pop;ldarg.2;ldfld Verse.Thing::def;ldfld Verse.ThingDef::stackLimit;label 1")
            .Transpiler(generator, instructions);
            */
        public static bool Prefix(IntVec3 c, Map map, Thing thing, IntVec3 center, ref byte __result)
        {
            Mod.Debug("GenPlace.PlaceSpotQualityAt begin");
            if (thing.stackCount >= Limits.CalculateStackLimit(map, c))
            {
                __result = 0;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
