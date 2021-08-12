using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace StockpileStackLimit
{
    [HarmonyPatch(typeof(StorageSettings), nameof(StorageSettings.ExposeData))]
    public static class AdditionalStorageSettings
    {

        public static readonly Dictionary<StorageSettings, int> limits = new Dictionary<StorageSettings, int>();
        public static readonly Dictionary<StorageSettings, int> refillpercents = new Dictionary<StorageSettings, int>();
        public static readonly HashSet<StorageSettings> refillingdisabled = new HashSet<StorageSettings>();
        public const int max_limit = 99999;
        public static void Postfix(StorageSettings __instance)
        {
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                var limit = __instance.GetStacklimit();
                if (limit >= 0) Scribe.saver.WriteElement("stacklimit", limit.ToString());
                var refillpercent = __instance.GetRefillPercent();
                if (refillpercent < 100) Scribe.saver.WriteElement("refillpercent", refillpercent.ToString());
            }
            else if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                __instance.SetStacklimit(ScribeExtractor.ValueFromNode(Scribe.loader.curXmlParent["stacklimit"], -1));
                __instance.SetRefillPercent(ScribeExtractor.ValueFromNode(Scribe.loader.curXmlParent["refillpercent"], 100));
            }
        }
        public static int CalculateStackLimit(Thing t)
        {
            var slotgroup = t.GetSlotGroup();
            if (slotgroup == null) return t.def.stackLimit;
            var limit = GetStacklimit(slotgroup.Settings);
            return limit >= 0 ? limit : t.def.stackLimit;
        }

        public static int CalculateStackLimit(StorageSettings setting)
        {
            var limit = GetStacklimit(setting);
            return limit >= 0 ? limit : max_limit;
        }

        public static int CalculateStackLimit(Map map, IntVec3 cell)
        {
            var slotgroup = map.haulDestinationManager.SlotGroupAt(cell);
            if (slotgroup == null) return max_limit;
            var limit = GetStacklimit(slotgroup.Settings);
            return limit >= 0 ? limit : max_limit;
        }


        public static bool TryGetLimit(SlotGroup slotgroup, out int limit)
        {
            var setting = slotgroup.Settings;
            bool result = limits.TryGetValue(setting, out limit);
            if (!result) limit = -1;
            return result;
        }

        public static int GetStacklimit(this StorageSettings settings) => limits.ContainsKey(settings) ? limits[settings] : -1;
        public static void SetStacklimit(this StorageSettings settings, int limit)
        {
            if (limit < 0) limits.Remove(settings);
            else limits[settings] = limit;
        }
        /// <summary>
        /// create open instance delegate of RimWorld.StorageSettings.TryNotifyChanged()
        /// </summary>
        public static Action<StorageSettings> StorageSettings_TryNotifyChanged = HarmonyLib.AccessTools.MethodDelegate<Action<StorageSettings>>(HarmonyLib.AccessTools.DeclaredMethod(typeof(StorageSettings), "TryNotifyChanged"));
        public static void SetStacklimitAndNotifyChange(this StorageSettings settings, int limit)
        {
            if (limit < 0)
            {
                limits.Remove(settings);
            }
            else
            {
                if (limit < (limits.ContainsKey(settings) ? limits[settings] : max_limit))
                    StorageSettings_TryNotifyChanged(settings);
                limits[settings] = limit;
            }
        }
        public static int GetRefillPercent(this StorageSettings settings) => refillpercents.ContainsKey(settings) ? refillpercents[settings] : 100;
        public static void SetRefillPercent(this StorageSettings settings, int refillpercent)
        {
            if (refillpercent < 100) refillpercents[settings] = refillpercent;
            else refillpercents.Remove(settings);
        }
        public static bool IsRefillingDisabled(this StorageSettings settings) => refillingdisabled.Contains(settings);
        public static void SetRefillingDisabled(this StorageSettings settings) => refillingdisabled.Add(settings);
        public static void UnsetRefillingDisabled(this StorageSettings settings) => refillingdisabled.Remove(settings);
    }
}