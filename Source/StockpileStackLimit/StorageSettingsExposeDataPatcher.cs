using HarmonyLib;
using RimWorld;
using Verse;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(StorageSettings), "ExposeData")]
    internal class StorageSettingsExposeDataPatcher
    {
        public static void Postfix(StorageSettings __instance)
        {
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                Scribe.saver.WriteElement("stacklimit", Limits.GetLimit(__instance).ToString());
            }
            else if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                Limits.SetLimit(__instance,
                    ScribeExtractor.ValueFromNode(Scribe.loader.curXmlParent["stacklimit"], -1));
            }
        }
    }
}