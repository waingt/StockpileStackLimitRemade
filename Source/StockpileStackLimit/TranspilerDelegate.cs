using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace StockpileStackLimit
{
    public delegate IEnumerable<CodeInstruction> TranspilerDelegate(ILGenerator generator,
        IEnumerable<CodeInstruction> instructions);
}