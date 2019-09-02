using RimWorld;
using Verse;
using Harmony;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(StorageSettings), "ExposeData")]
	class StorageSettingsExposeDataPatcher
	{
		public static void Postfix(StorageSettings __instance)
		{
			Scribe_Values.Look<int>(ref Limits.GetLimit(__instance), "stacklimit", -1, false);
		}
	}
}
