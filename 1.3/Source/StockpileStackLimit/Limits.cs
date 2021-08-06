using System.Collections.Generic;
using RimWorld;
using Verse;

namespace StockpileStackLimit
{
    public static class Limits
    {
        public static readonly Dictionary<StorageSettings, int> limits = new Dictionary<StorageSettings, int>();
        public const int max_limit = 99999;

        public static int CalculateStackLimit(Thing t)
        {
            var slotgroup = t.GetSlotGroup();
            if (slotgroup == null) return t.def.stackLimit;
            var limit = GetLimit(slotgroup.Settings);
            return limit >= 0 ? limit : t.def.stackLimit;
        }

        public static int CalculateStackLimit(SlotGroup slotgroup)
        {
            if (slotgroup == null) return max_limit;
            var setting = slotgroup.Settings;
            var limit = GetLimit(setting);
            return limit >= 0 ? limit : max_limit;
        }

        public static int CalculateStackLimit(Map map, IntVec3 cell)
        {
            var slotgroup = map.haulDestinationManager.SlotGroupAt(cell);
            if (slotgroup == null) return max_limit;
            var limit = GetLimit(slotgroup.Settings);
            return limit >= 0 ? limit : max_limit;
        }


        public static bool TryGetLimit(SlotGroup slotgroup, out int limit)
        {
            var setting = slotgroup.Settings;
            bool result = limits.TryGetValue(setting, out limit);
            if (!result) limit = -1;
            return result;
        }

        public static int GetLimit(StorageSettings settings)
        {
            return !limits.ContainsKey(settings) ? -1 : limits[settings];
        }

        public static void SetLimit(StorageSettings settings, int limit)
        {
            if (limit < 0) limits.Remove(settings);
            else if (!limits.TryAdd(settings, limit))
                limits[settings] = limit;
        }
    }
}