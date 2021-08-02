using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace StockpileStackLimit
{
    public class TranspilerFactory
    {
        private readonly TranspilerDelegate GetTranspiler;
        private readonly List<ITranspiler> transpilers;
        internal IEnumerator<CodeInstruction> CodeEnumerator;
        internal ILGenerator Generator;
        internal List<Label> Labels;
        internal List<LocalBuilder> Locals;

        public TranspilerFactory()
        {
            transpilers = new List<ITranspiler>();
            GetTranspiler = Transpiler;
        }

        public TranspilerFactory Search(string str)
        {
            transpilers.Add(new SearchTranspiler(CodeParser.ParseMutiple(str)));
            return this;
        }

        public TranspilerFactory Search(string str, int num)
        {
            for (var i = 0; i < num; i++)
            {
                transpilers.Add(new SearchTranspiler(CodeParser.ParseMutiple(str)));
            }

            return this;
        }

        public TranspilerFactory Replace(string from, string to)
        {
            transpilers.Add(new SearchDeleteTranspiler(CodeParser.ParseMutiple(from)));
            transpilers.Add(new InsertTranspiler(CodeParser.ParseMutiple(to)));
            return this;
        }

        public TranspilerFactory Replace(string from, string to, int num)
        {
            for (var i = 0; i < num; i++)
            {
                transpilers.Add(new SearchDeleteTranspiler(CodeParser.ParseMutiple(from)));
                transpilers.Add(new InsertTranspiler(CodeParser.ParseMutiple(to)));
            }

            return this;
        }

        public TranspilerFactory Insert(string str)
        {
            transpilers.Add(new InsertTranspiler(CodeParser.ParseMutiple(str)));
            return this;
        }

        public TranspilerFactory Delete(string str)
        {
            transpilers.Add(new SearchDeleteTranspiler(CodeParser.ParseMutiple(str)));
            return this;
        }

        public TranspilerFactory Delete(string str, int num)
        {
            for (var i = 0; i < num; i++)
            {
                transpilers.Add(new SearchDeleteTranspiler(CodeParser.ParseMutiple(str)));
            }

            return this;
        }

        public TranspilerFactory Delete(int num)
        {
            transpilers.Add(new SkipTranspiler(num));
            return this;
        }

        public HarmonyMethod GetTranspilerMethod()
        {
            return new HarmonyMethod(GetTranspiler.Method);
        }

        public IEnumerable<CodeInstruction> Transpiler(ILGenerator generator, IEnumerable<CodeInstruction> instr)
        {
            if (transpilers.Count == 0 || !(transpilers[transpilers.Count - 1] is EndingTranspiler))
            {
                transpilers.Add(new EndingTranspiler());
            }

            Generator = generator;
            CodeEnumerator = instr.GetEnumerator();
            Locals = new List<LocalBuilder>();
            Labels = new List<Label>();
            foreach (var t in transpilers)
            {
                foreach (var code in t.TransMethod(this))
                {
                    yield return code;
                }
            }
        }
    }
}