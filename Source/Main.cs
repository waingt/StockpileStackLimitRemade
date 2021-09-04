using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;
[assembly: AssemblyVersion("1.3.3.*")]
namespace StockpileLimit
{
    public class StockpileStackLimitMod : Mod
    {
        public StockpileStackLimitMod(ModContentPack content) : base(content)
        {
#if DEBUG
            FileLog.Reset();
            Harmony.DEBUG = true;
#endif
            Utilities.HarmonyInstance.PatchAll();
            Utilities.Message("Patched");
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Rect buttonArea = new Rect(inRect.x, inRect.y, 300f, 24f);
            if (Widgets.ButtonText(buttonArea, "Set all small stockpile as buffer stockpile")) SetAllBuffer();
            TooltipHandler.TipRegion(buttonArea, "For each stockpile with less than 5 tiles in the current map, set its refill threshod to 0, that is, refill when empty");
        }
        public static void SetAllBuffer()
        {
            var map = Find.CurrentMap;
            if (map == null) return;
            foreach (var t in map.haulDestinationManager.AllHaulDestinations)
            {
                var stockpile = t as ISlotGroupParent;
                if (stockpile != null && stockpile.AllSlotCellsList().Count < 5)
                {
                    stockpile.GetStoreSettings().SetRefillPercent(0);
                }
            }
        }

        public override string SettingsCategory() => Utilities.Name;
    }
    public static class Utilities
    {
        public static string Name = Assembly.GetExecutingAssembly().GetName().Name;
        public static readonly Harmony HarmonyInstance = new Harmony(Name);
        public static void Foreach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            foreach (var i in source) action(i);
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