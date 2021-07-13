using System.Collections.Generic;
using HarmonyLib;

namespace StockpileStackLimit
{
    internal interface ITranspiler
    {
        IEnumerable<CodeInstruction> TransMethod(TranspilerFactory factory);
    }
}