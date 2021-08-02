using HarmonyLib;
using Verse;

namespace StockpileStackLimit
{
    [StaticConstructorOnStartup]
    public static class Mod
    {
        public const string Name = "Stockpile Stack Limit";

        static Mod()
        {
            var HarmonyLib = new Harmony("StockpileStackLimit");
#if DEBUG
            FileLog.Reset();
            Harmony.DEBUG = false;
#endif
            HarmonyLib.PatchAll();
            Message("Patched");
        }

        public static void Debug(string s)
        {
#if DEBUG
            Log.Message($"[{Name}] {s}");
#endif
        }

        public static void Message(string s)
        {
            Log.Message($"[{Name}] {s}");
        }

        public static void Warning(string s)
        {
            Log.Warning($"[{Name}] {s}");
        }

        public static void Error(string s)
        {
            Log.Error($"[{Name}] {s}");
        }
    }
}