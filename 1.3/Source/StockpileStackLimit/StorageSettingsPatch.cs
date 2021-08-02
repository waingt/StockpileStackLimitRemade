using HarmonyLib;
using RimWorld;
using Verse;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(StorageSettings), nameof(StorageSettings.ExposeData))]
    internal class StorageSettingsExposeDataPatch
    {
        public static void Postfix(StorageSettings __instance)
        {
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                var limit = Limits.GetLimit(__instance);
                if (limit >= 0) Scribe.saver.WriteElement("stacklimit", limit.ToString());
            }
            else if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                Limits.SetLimit(__instance,
                    ScribeExtractor.ValueFromNode(Scribe.loader.curXmlParent["stacklimit"], -1));
            }
        }
    }
}