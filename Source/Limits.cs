using Harmony;
using System;
using System.Collections.Generic;

namespace StockpileStackLimit
{
    public static class Limits
    {
        private static Dictionary<RimWorld.StorageSettings, int> limits = new Dictionary<RimWorld.StorageSettings, int>();
        public static int CalculateStackLimit(Verse.Thing t)
        {
            if (!t.Spawned) return t.def.stackLimit;
            var slotgroup = t.Map.haulDestinationManager.SlotGroupAt(t.Position);//t.GetSlotGroup();
            if (slotgroup == null) return t.def.stackLimit;
            var limit = Limits.GetLimit(slotgroup.Settings);
            return limit > 0 ? limit : t.def.stackLimit;
        }
        public static int CalculateStackLimit(RimWorld.SlotGroup slotgroup)
        {
            if (slotgroup == null) return 99999;
            var setting = slotgroup.Settings;
            var limit = Limits.GetLimit(setting);
            return limit > 0 ? limit : 99999;
        }
        public static int CalculateStackLimit(Verse.Map map, Verse.IntVec3 cell)
        {
            var slotgroup = map.haulDestinationManager.SlotGroupAt(cell);
            if (slotgroup == null) return 0;
            var limit = Limits.GetLimit(slotgroup.Settings);
            return limit > 0 ? limit : 0;
        }
        public static bool HasStackLimit(Verse.Thing t)
        {
            if (!t.Spawned) return false;
            var slotgroup = t.Map.haulDestinationManager.SlotGroupAt(t.Position); //t.GetSlotGroup();
            if (slotgroup == null) return false;
            var setting = slotgroup.Settings;
            if (!Limits.HasLimit(setting)) return false;
            return true;
        }
        public static bool HasStackLimit(RimWorld.SlotGroup slotgroup)
        {
            if (slotgroup == null) return false;
            var setting = slotgroup.Settings;
            if (!Limits.HasLimit(setting)) return false;
            return true;
        }
        public static int GetLimit(RimWorld.StorageSettings settings)
        {
            if (!limits.ContainsKey(settings)) return -1;
            return limits[settings];
        }

        public static void SetLimit(RimWorld.StorageSettings settings, int limit)
        {
            var t = settings.owner as RimWorld.ISlotGroupParent;
            //t.Map.listerHaulables.RecalcAllInCells(t.AllSlotCells());
            if (!limits.ContainsKey(settings))
                limits.Add(settings, limit);
            else
                limits[settings] = limit;
        }

        public static bool HasLimit(RimWorld.StorageSettings settings)
            => limits.ContainsKey(settings) && limits[settings] > 0;
    }
}
