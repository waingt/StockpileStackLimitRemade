using HarmonyLib;
using RimWorld;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(ITab_Storage), "FillTab")]
    internal class ITab_StorageFillTabsPatcher
    {
        public static ITab_Storage currentTab;

        public static void Prefix(ITab_Storage __instance)
        {
            currentTab = __instance;
        }

        public static void Postfix()
        {
            currentTab = null;
        }
    }
}