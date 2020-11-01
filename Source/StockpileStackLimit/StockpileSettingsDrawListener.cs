using RimWorld;
using System.Reflection;
using UnityEngine;
using Verse;
using HarmonyLib;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(ThingFilterUI), "DoThingFilterConfigWindow")]
    class ThingFilterUIWindowPatcher
    {
        static string buffer = "";
        static StorageSettings oldSettings = null;

        const int max = 9999999;

        public static void Prefix(ref Rect rect)
        {
            ITab_Storage tab = ITab_StorageFillTabsPatcher.currentTab;
            if (tab == null)
            {
                return;
            }

            rect.yMin += 32f;
        }

        public static void Postfix(ref Rect rect)
        {
            ITab_Storage tab = ITab_StorageFillTabsPatcher.currentTab;
            if (tab == null)
            {
                return;
            }

            var storeSettingsParent = (IStoreSettingsParent)typeof(ITab_Storage).GetProperty("SelStoreSettingsParent", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(tab, new object[0]);
            StorageSettings settings = storeSettingsParent.GetStoreSettings();

            var limit = Limits.GetLimit(settings);
            var hasLimit = limit != -1;

            Widgets.CheckboxLabeled(new Rect(rect.xMin, rect.yMin - 24f - 3f - 32f, rect.width / 2, 24f), "Total stack limit", ref hasLimit);

            if (hasLimit)
            {
                if (oldSettings != settings)
                {
                    buffer = limit.ToString();
                }

                Widgets.TextFieldNumeric<int>(new Rect(rect.xMin + (rect.width / 2) + 60f, rect.yMin - 24f - 3f - 32f, (rect.width / 2) - 60f, 24f), ref limit, ref buffer, 0, max);
            }

            Limits.SetLimit(settings, hasLimit ? limit : -1);

            oldSettings = settings;
        }
    }
    [HarmonyPatch(typeof(ITab_Storage), "FillTab")]
    class ITab_StorageFillTabsPatcher
    {
        public static ITab_Storage currentTab = null;

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
