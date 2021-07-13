using HarmonyLib;
using RimWorld;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(StorageSettingsClipboard), "PasteInto")]
    public static class PasteIntoPatch
    {
        public static void Postfix(StorageSettings s)
        {
            Limits.SetLimit(s, CopyPatch.clipboardLimit);
        }
    }
}