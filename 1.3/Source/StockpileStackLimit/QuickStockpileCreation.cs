using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartStockpileCreation.RimObjs.SmartStockpile;
using UnityEngine;
using Verse;
using RimWorld;
using HarmonyLib;

namespace StockpileLimit
{
    [HarmonyPatch(typeof(Thing), nameof(Thing.GetGizmos))]
    public static class Thing_GetGizmosPatch
    {
        static bool Prepare()
        {
            if (ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name == "Quick Stockpile Creation"))
            {
                BufferStockpileDesignator.makeNewZone = AccessTools.MethodDelegate<Func<SmartStockpileDesignator, Zone>>(AccessTools.DeclaredMethod(typeof(SmartStockpileDesignator), "MakeNewZone"));
                BufferStockpileDesignator.finalizeDesignationSucceeded = AccessTools.MethodDelegate<Action<SmartStockpileDesignator>>(AccessTools.DeclaredMethod(typeof(SmartStockpileDesignator), "FinalizeDesignationSucceeded"));
                return true;
            }
            else return false;
        }

        public static void Postfix(Thing __instance, ref IEnumerable<Gizmo> __result)
        {
            if (__instance.def.EverHaulable)
            {
                __result = __result.AddItem(new BufferStockpileGizmo());
            }
        }
    }
    public class BufferStockpileGizmo : SmartStockpileGizmo
    {
        public BufferStockpileGizmo() : base()
        {
            defaultLabel = "Create buffer stockpile";
            defaultDesc = "Create a buffer stockpile for this item and set refill threshod to 0";
        }
        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            var des = Find.MapUI.designatorManager.SelectedDesignator as SmartStockpileDesignator;
            if (des != null)
            {
                BufferStockpileDesignator bufferStockpileDesignator = new BufferStockpileDesignator(des);
                Find.MapUI.designatorManager.Select(bufferStockpileDesignator);
            }
        }
    }
    public class BufferStockpileDesignator : Designator_ZoneAddStockpile
    {
        public SmartStockpileDesignator stockpileDesignator;
        public BufferStockpileDesignator(SmartStockpileDesignator des) => stockpileDesignator = des;
        public static Func<SmartStockpileDesignator, Zone> makeNewZone;
        public static Action<SmartStockpileDesignator> finalizeDesignationSucceeded;
        protected override Zone MakeNewZone()
        {
            var zone = makeNewZone(stockpileDesignator);
            ((Zone_Stockpile)zone).settings.SetRefillPercent(0);
            return zone;
        }
        protected override void FinalizeDesignationSucceeded() => finalizeDesignationSucceeded(stockpileDesignator);
    }
}
