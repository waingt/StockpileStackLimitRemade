using HarmonyLib;
using RimWorld;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(StorageSettingsClipboard), "Copy")]
    public static class CopyPatch
    {
        public static int clipboardLimit = -1;

        public static void Postfix(StorageSettings s)
        {
            clipboardLimit = Limits.GetLimit(s);
        }
    }
}