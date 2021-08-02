using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace StockpileStackLimit
{
    [StaticConstructorOnStartup]
    public static class Main
    {
        public static void Foreach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            foreach (var i in source) action(i);
        }

        public const string Name = "StockpileStackLimit";
        public static readonly Harmony HarmonyLib = new Harmony(Name);
        static Main()
        {
#if DEBUG
            FileLog.Reset();
            Harmony.DEBUG = true;
#endif
            HarmonyLib.PatchAll();
            Message("Patched");
        }

        [Conditional("DEBUG")]
        public static void Debug(string s)
        {
            var caller = new StackFrame(1).GetMethod();
            Log.Message($"[{Name}] {s}, Caller is {caller.DeclaringType}::{caller.Name}");
        }

        public static void Message(string s)
        {
            Log.Message($"[{Name}] {s}");
        }

        public static void Error(string s)
        {
            var caller = new StackFrame(1).GetMethod();
            Log.Error($"[{Name}] {s}, Caller is {caller.DeclaringType}::{caller.Name}");
        }
    }
}