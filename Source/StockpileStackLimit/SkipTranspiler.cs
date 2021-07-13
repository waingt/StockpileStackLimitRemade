using System;
using System.Collections.Generic;
using HarmonyLib;

namespace StockpileStackLimit
{
    internal class SkipTranspiler : ITranspiler
    {
        private readonly int count;

        public SkipTranspiler(int num)
        {
            if (num <= 0)
            {
                throw new Exception();
            }

            count = num;
        }

        public IEnumerable<CodeInstruction> TransMethod(TranspilerFactory factory)
        {
            var t = count;
            var instructions = factory.CodeEnumerator;
            while (t > 0 && instructions.MoveNext())
            {
                t--;
            }

            return null;
        }
    }
}