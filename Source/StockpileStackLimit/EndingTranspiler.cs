using System.Collections.Generic;
using HarmonyLib;

namespace StockpileStackLimit
{
    internal class EndingTranspiler : ITranspiler
    {
        public IEnumerable<CodeInstruction> TransMethod(TranspilerFactory factory)
        {
            var instructions = factory.CodeEnumerator;
            while (instructions.MoveNext())
            {
                yield return instructions.Current;
            }
        }
    }
}