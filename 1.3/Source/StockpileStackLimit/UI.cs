using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(ThingFilterUI), "DoThingFilterConfigWindow")]
    public class ThingFilterUIWindowPatch
    {
        public static string buffer;
        //static StorageSettings prev_settings;
        static ISlotGroupParent selected;

        static Dictionary<int, string> limitmenu = new Dictionary<int, string>()
        {
            [-1] = "No limit",
            [0] = "0",
            [1] = "1",
            [2] = "2",
            [5] = "5",
            [10] = "10",
        };
        static Dictionary<int, string> limittooltip = new Dictionary<int, string>()
        {
            [-1] = "No limit is default, like vanilla.",
            [0] = "Keep empty, like there's no stockpile",
        };
        static string otherlimit_text = "Other limit:";

        static Dictionary<int, string> refillmenu = new Dictionary<int, string>()
        {
            [100] = "Full",
            [50] = "Half",
            [0] = "Empty",
        };
        static Dictionary<int, string> refilltooltip = new Dictionary<int, string>()
        {
            [100] = "Full is default. Always keep fully stocked, like vanilla.",
            [0] = "Start refilling when empty",
        };
        static string otherrefill_text = "Other percent:";
        public static void Prefix(ref Rect rect)
        {
            selected = Find.Selector.SingleSelectedObject as ISlotGroupParent;
            if (selected != null) rect.yMin += 90f;
        }
        public static void Postfix(ref Rect rect)
        {
            if (selected == null) return;

            if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.Escape))
            {
                UI.UnfocusCurrentControl();
                Event.current.Use();
            }

            var settings = selected.GetStoreSettings();
            var limit = settings.GetStacklimit();

            Rect drawArea = new Rect(rect.xMin, rect.yMin - 48f - 3f - 90f, rect.width, 24f);
            Rect labelArea, buttonArea, inputArea;
            drawArea.SplitVertically(70f, out labelArea, out drawArea, 5f);
            drawArea.SplitVertically(100f, out buttonArea, out drawArea, 5f);
            inputArea = drawArea.LeftPartPixels(50f);
            Widgets.Label(labelArea, "Set limit");
            TooltipHandler.TipRegion(labelArea, "Set an upper limit for each tile in this stockpile. The limit is strictly in effect for pawns. Pawns will not haul items exceeding the upper limit.");
            if (limittooltip.ContainsKey(limit)) TooltipHandler.TipRegion(buttonArea, limittooltip[limit]);
            if (Widgets.ButtonText(buttonArea, limitmenu.ContainsKey(limit) ? limitmenu[limit] : otherlimit_text))
            {
                var options = new List<FloatMenuOption>(limitmenu.Select(p => new FloatMenuOption(p.Value, () =>
                settings.SetStacklimitAndNotifyChange(p.Key))));
                Find.WindowStack.Add(new FloatMenu(options));
            }
            //if (prev_settings != settings)
            buffer = null;
            var new_limit = limit;
            Widgets.TextFieldNumeric(inputArea, ref new_limit, ref buffer, -1, AdditionalStorageSettings.max_limit);
            if (new_limit != limit) settings.SetStacklimitAndNotifyChange(new_limit);
            //prev_settings = settings;

            var refillpercent = settings.GetRefillPercent();
            drawArea = new Rect(rect.xMin, rect.yMin - 48f - 3f - 60f, rect.width, 24f);
            drawArea.SplitVertically(70f, out labelArea, out drawArea, 5f);
            drawArea.SplitVertically(100f, out buttonArea, out drawArea, 5f);
            drawArea.SplitVertically(50f, out inputArea, out drawArea, 5f);
            Widgets.Label(labelArea, "Refill at");
            TooltipHandler.TipRegion(labelArea, "Set the refill threshold of *each tile* in this stockpile.This can be considered as the lower limit");
            if (refilltooltip.ContainsKey(refillpercent)) TooltipHandler.TipRegion(buttonArea, refilltooltip[refillpercent]);
            if (Widgets.ButtonText(buttonArea, refillmenu.ContainsKey(refillpercent) ? refillmenu[refillpercent] : otherrefill_text))
            {
                var options = new List<FloatMenuOption>(refillmenu.Select(p => new FloatMenuOption(p.Value, () =>
                settings.SetRefillPercent(p.Key))));
                Find.WindowStack.Add(new FloatMenu(options));
            }
            buffer = null;
            var new_refillpercent = refillpercent;
            Widgets.TextFieldNumeric(inputArea, ref new_refillpercent, ref buffer, 0, 100);
            if (new_refillpercent != refillpercent) settings.SetRefillPercent(new_refillpercent);
            Widgets.Label(drawArea, "%");

            drawArea = new Rect(rect.xMin, rect.yMin - 48f - 3f - 30f, rect.width, 24f);
            var checkOn = settings.IsRefillingDisabled();
            var new_checkOn = checkOn;
            TooltipHandler.TipRegion(drawArea, "Temporarily disable refilling for all tiles in this stockpile. It is just a handy functionality, not a part of storage settings.");
            Widgets.CheckboxLabeled(drawArea, "Pause refilling", ref new_checkOn, placeCheckboxNearText: true);
            if (new_checkOn != checkOn)
            {
                if (new_checkOn) settings.SetRefillingDisabled();
                else settings.UnsetRefillingDisabled();
            }
        }
    }
}