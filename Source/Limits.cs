using Harmony;
using System;
using System.Collections.Generic;

namespace StockpileStackLimit
{
    class IntHolder
    {
        public int Int = -1;

        public IntHolder(int Int)
        {
            this.Int = Int;
        }
    }
    class Limits
    {
        private static Dictionary<RimWorld.StorageSettings, IntHolder> limits = new Dictionary<RimWorld.StorageSettings, IntHolder>();
        public static int CalculateStackLimit(Verse.Thing t)
        {
            if (!t.Spawned) return t.def.stackLimit;
            var slotgroup = t.Map.haulDestinationManager.SlotGroupAt(t.Position);//t.GetSlotGroup();
            if (slotgroup == null) return t.def.stackLimit;
            var setting = slotgroup.Settings;
            if (!Limits.HasLimit(setting)) return t.def.stackLimit;
            var limit = Limits.GetLimit(setting);
            return limit;
        }
        public static int CalculateStackLimit(RimWorld.SlotGroup slotgroup)
        {
            if (slotgroup == null) return 99999;
            var setting = slotgroup.Settings;
            if (!Limits.HasLimit(setting)) return 99999;
            var limit = Limits.GetLimit(setting);
            var t = limit > 0 ? limit : 99999;
            return t;
        }
        public static int CalculateStackLimit(Verse.Map map, Verse.IntVec3 cell)
        {
            var slotgroup = map.haulDestinationManager.SlotGroupAt(cell);
            if (slotgroup == null) return 0;
            var setting = slotgroup.Settings;
            if (!Limits.HasLimit(setting)) return 0;
            var limit = Limits.GetLimit(setting);
            var t = limit > 0 ? limit : 0;
            return t;
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
        public static ref int GetLimit(RimWorld.StorageSettings settings)
        {
            if (!limits.ContainsKey(settings))
                limits.Add(settings, new IntHolder(-1));

            return ref limits[settings].Int;
        }

        public static void SetLimit(RimWorld.StorageSettings settings, int limit)
        {
            var t = settings.owner as RimWorld.ISlotGroupParent;
            t.Map.listerHaulables.RecalcAllInCells(t.AllSlotCells());
            if (!limits.ContainsKey(settings))
                limits.Add(settings, new IntHolder(limit));
            else
                limits[settings].Int = limit;
        }

        public static bool HasLimit(RimWorld.StorageSettings settings) => limits.ContainsKey(settings) && limits[settings].Int != -1;
    }
}
