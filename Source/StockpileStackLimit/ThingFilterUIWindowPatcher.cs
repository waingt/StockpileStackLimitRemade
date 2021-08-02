using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(ThingFilterUI), "DoThingFilterConfigWindow")]
    internal class ThingFilterUIWindowPatcher
    {
        private const int max = 9999999;
        private static string buffer = "";
        private static StorageSettings oldSettings;

        public static void Prefix(ref Rect rect)
        {
            var tab = ITab_StorageFillTabsPatcher.currentTab;
            if (tab == null)
            {
                return;
            }

            rect.yMin += 32f;
        }

        public static void Postfix(ref Rect rect)
        {
            var tab = ITab_StorageFillTabsPatcher.currentTab;
            if (tab == null)
            {
                return;
            }

            var storeSettingsParent = (IStoreSettingsParent) typeof(ITab_Storage)
                .GetProperty("SelStoreSettingsParent", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetGetMethod(true).Invoke(tab, new object[0]);
            var settings = storeSettingsParent?.GetStoreSettings();

            var limit = Limits.GetLimit(settings);
            var hasLimit = limit != -1;

            Widgets.CheckboxLabeled(new Rect(rect.xMin, rect.yMin - 48f - 3f - 32f, rect.width / 2, 24f),
                "Total stack limit", ref hasLimit);

            if (hasLimit)
            {
                if (oldSettings != settings)
                {
                    buffer = limit.ToString();
                }

                Widgets.TextFieldNumeric(
                    new Rect(rect.xMin + (rect.width / 2) + 60f, rect.yMin - 48f - 3f - 32f, (rect.width / 2) - 60f,
                        24f), ref limit, ref buffer, 0, max);
            }

            Limits.SetLimit(settings, hasLimit ? limit : -1);

            oldSettings = settings;
        }
    }
}