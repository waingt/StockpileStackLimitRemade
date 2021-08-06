using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(ThingFilterUI), "DoThingFilterConfigWindow")]
    internal class ThingFilterUIWindowPatch
    {
        static string buffer;
        static StorageSettings prev_settings;
        static IStoreSettingsParent selected;

        public static void Prefix(ref Rect rect)
        {
            selected = Find.Selector.SingleSelectedObject as IStoreSettingsParent;
            if (selected != null) rect.yMin += 32f;
        }
        public static void Postfix(ref Rect rect)
        {
            if (selected == null) return;
            var settings = selected.GetStoreSettings();
            var limit = Limits.GetLimit(settings);
            var new_hasLimit = limit >= 0;

            Widgets.CheckboxLabeled(new Rect(rect.xMin, rect.yMin - 48f - 3f - 32f, rect.width / 2, 24f),
                "Total stack limit", ref new_hasLimit);

            // if no stack limit, limit is encoded as -1.
            var new_limit = -1;
            if (new_hasLimit)
            {
                new_limit = limit;
                if (prev_settings != settings) buffer = null;
                Widgets.TextFieldNumeric(
                    new Rect(rect.xMin + (rect.width / 2) + 60f, rect.yMin - 48f - 3f - 32f, (rect.width / 2) - 60f,
                        24f), ref new_limit, ref buffer, 0, Limits.max_limit);
            }
            // TODO notify haulables change
            if (new_limit != limit) { Limits.SetLimit(settings, new_limit); }

            prev_settings = settings;
        }
    }
}