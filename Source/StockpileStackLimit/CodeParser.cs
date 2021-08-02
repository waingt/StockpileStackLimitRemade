using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using HarmonyLib;

namespace StockpileStackLimit
{
    public static class CodeParser
    {
        public static readonly OpCode AnyOpcode =
            (OpCode) typeof(OpCode).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0]
                .Invoke(new object[] {256, -257283419});

        public static readonly object AnyOprand = "*";

        public static readonly OpCode LocalvarOpcode =
            (OpCode) typeof(OpCode).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0]
                .Invoke(new object[] {257, 279317166});

        public static readonly OpCode LabelOpcode =
            (OpCode) typeof(OpCode).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0]
                .Invoke(new object[] {258, 279317166});

        public static readonly Regex MatchMethod = new Regex(@"^(.*?)::?(.*?)(?:\((.*?)\))?$");

        public static readonly Dictionary<string, Type> KeywordTypes = new Dictionary<string, Type>
        {
            {"bool", typeof(bool)}, {"byte", typeof(byte)}, {"char", typeof(char)},
            {
                "decimal", typeof(decimal)
            },
            {"double", typeof(double)}, {"float", typeof(float)}, {"int", typeof(int)}, {"long", typeof(long)},
            {"sbyte", typeof(sbyte)}, {"short", typeof(short)}, {"string", typeof(string)}, {"uint", typeof(uint)},
            {"ulong", typeof(ulong)}, {"ushort", typeof(ushort)}
        };

        public static Type String2Type(string str)
        {
            _ = KeywordTypes.TryGetValue(str, out var t);
            if (t == null)
            {
                t = AccessTools.TypeByName(str);
            }

            return t;
        }

        public static CodeInstruction Parse(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new Exception("String to Parse is null or empty");
            }

            var parts = str.Split(new[] {' '}, 2);
            var opcodestr = parts[0];
            var opcode = AnyOpcode;
            if (opcodestr != "*")
            {
                opcodestr = opcodestr.ToLower();
                if (opcodestr == "localvar")
                {
                    return new CodeInstruction(LocalvarOpcode, String2Type(parts[1]));
                }

                if (opcodestr == "label")
                {
                    return new CodeInstruction(LabelOpcode, Convert.ToInt32(parts[1]));
                }

                opcodestr = opcodestr.Replace('.', '_');
                opcode = (OpCode) typeof(OpCodes)
                    .GetField(opcodestr, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public)
                    ?.GetValue(null);
            }

            if (parts.Length == 1 || opcode.OperandType == OperandType.InlineNone)
            {
                return new CodeInstruction(opcode);
            }

            var oprandstr = parts[1];
            if (oprandstr == "*")
            {
                return new CodeInstruction(opcode, AnyOprand);
            }

            object obj = null;
            switch (opcode.OperandType)
            {
                case OperandType.InlineMethod:
                    var result = MatchMethod.Match(oprandstr);
                    if (!result.Success)
                    {
                        obj = null;
                    }
                    else
                    {
                        var type = String2Type(result.Groups[1].Value);
                        var method = result.Groups[2].Value;
                        var argstr = result.Groups[3].Value;
                        Type[] args = null;
                        if (argstr != "")
                        {
                            var t = argstr.Split(',');
                            args = new Type[t.Length];
                            for (var i = 0; i < t.Length; i++)
                            {
                                var s = t[i].Trim();
                                _ = KeywordTypes.TryGetValue(s, out args[i]);
                                if (args[i] == null)
                                {
                                    args[i] = String2Type(s);
                                }
                            }
                        }

                        obj = opcode == OpCodes.Newobj
                            ? AccessTools.Constructor(type, args)
                            : AccessTools.Method(type, method, args);
                    }

                    break;
                case OperandType.InlineField:
                    parts = oprandstr.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);
                    obj = AccessTools.Field(String2Type(parts[0]), parts[1]);
                    break;
                case OperandType.InlineString:
                    obj = oprandstr;
                    break;
                case OperandType.InlineType:
                    obj = String2Type(oprandstr);
                    break;
                case OperandType.InlineI:
                case OperandType.InlineBrTarget:
                case OperandType.ShortInlineBrTarget:
                    obj = Convert.ToInt32(oprandstr);
                    break;
                case OperandType.InlineVar:
                case OperandType.ShortInlineI:
                    obj = Convert.ToInt16(oprandstr);
                    break;
                case OperandType.ShortInlineVar:
                case OperandType.InlineI8:
                    obj = Convert.ToByte(oprandstr);
                    break;
                case OperandType.InlineR:
                    obj = Convert.ToDouble(oprandstr);
                    break;
                case OperandType.ShortInlineR:
                    obj = Convert.ToSingle(oprandstr);
                    break;
            }

            return obj == null
                ? throw new Exception("Unknown OperandType or Wrong operand")
                : new CodeInstruction(opcode, obj);
        }

        public static List<CodeInstruction> ParseMutiple(string str)
        {
            var codes = str.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            var result = new List<CodeInstruction>(codes.Length);
            foreach (var s in codes)
            {
                result.Add(Parse(s));
            }

            return result;
        }

        public static bool IsMatchWith(this CodeInstruction CodeWithMatchOption, CodeInstruction instr)
        {
            var result = AnyOpcode == CodeWithMatchOption.opcode || CodeWithMatchOption.opcode == instr.opcode;
            result &= AnyOprand == CodeWithMatchOption.operand ||
                      (CodeWithMatchOption.operand?.Equals(instr.operand) ?? null == instr.operand);
            return result;
        }
    }
}