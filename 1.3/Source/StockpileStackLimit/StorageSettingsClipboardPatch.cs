using HarmonyLib;
using RimWorld;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(StorageSettingsClipboard), nameof(StorageSettingsClipboard.Copy))]
    public static class CopyPatch
    {
        public static int clipboardLimit = -1;

        public static void Postfix(StorageSettings s)
        {
            clipboardLimit = Limits.GetLimit(s);
        }
    }

    [HarmonyPatch(typeof(StorageSettingsClipboard), nameof(StorageSettingsClipboard.PasteInto))]
    public static class PasteIntoPatch
    {
        public static void Postfix(StorageSettings s)
        {
            Limits.SetLimit(s, CopyPatch.clipboardLimit);
        }
    }
}