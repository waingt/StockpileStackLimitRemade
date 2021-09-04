using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace StockpileLimit
{
    [HarmonyPatch(typeof(Thing), nameof(Thing.GetGizmos))]
    public static class Thing_GetGizmosPatch
    {
        static bool Prepare()
        {
            if (ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name == "Quick Stockpile Creation"))
            {
                BufferStockpileGizmo.Smartstockpilegizmotype = AccessTools.TypeByName("SmartStockpileCreation.RimObjs.SmartStockpile.SmartStockpileGizmo");
                BufferStockpileDesignator.SmartStockpileDesignatortype = AccessTools.TypeByName("SmartStockpileCreation.RimObjs.SmartStockpile.SmartStockpileDesignator");

                BufferStockpileDesignator.makeNewZone = AccessTools.MethodDelegate<Func<Designator_ZoneAddStockpile, Zone>>(AccessTools.DeclaredMethod(BufferStockpileDesignator.SmartStockpileDesignatortype, "MakeNewZone"));
                BufferStockpileDesignator.finalizeDesignationSucceeded = AccessTools.MethodDelegate<Action<Designator_ZoneAddStockpile>>(AccessTools.DeclaredMethod(BufferStockpileDesignator.SmartStockpileDesignatortype, "FinalizeDesignationSucceeded"));

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
    public class BufferStockpileGizmo : Command
    {
        public static Type Smartstockpilegizmotype;
        public Command Smartstockpilegizmo;
        public BufferStockpileGizmo() : base()
        {
            Smartstockpilegizmo = (Command)Activator.CreateInstance(Smartstockpilegizmotype);
            icon = Smartstockpilegizmo.icon;
            defaultLabel = "Create buffer stockpile";
            defaultDesc = "Create a buffer stockpile for this item and set refill threshod to 0";
        }
        public override void ProcessInput(Event ev)
        {
            Smartstockpilegizmo.ProcessInput(ev);
            var des = Find.MapUI.designatorManager.SelectedDesignator;
            if (BufferStockpileDesignator.SmartStockpileDesignatortype.IsInstanceOfType(des))
            {
                BufferStockpileDesignator bufferStockpileDesignator = new BufferStockpileDesignator((Designator_ZoneAddStockpile)des);
                Find.MapUI.designatorManager.Select(bufferStockpileDesignator);
            }
        }
    }
    public class BufferStockpileDesignator : Designator_ZoneAddStockpile
    {
        public static Type SmartStockpileDesignatortype;
        public Designator_ZoneAddStockpile stockpileDesignator;
        public BufferStockpileDesignator(Designator_ZoneAddStockpile des) => stockpileDesignator = des;
        public static Func<Designator_ZoneAddStockpile, Zone> makeNewZone;
        public static Action<Designator_ZoneAddStockpile> finalizeDesignationSucceeded;
        protected override Zone MakeNewZone()
        {
            var zone = makeNewZone(stockpileDesignator);
            ((Zone_Stockpile)zone).settings.SetRefillPercent(0);
            return zone;
        }
        protected override void FinalizeDesignationSucceeded() => finalizeDesignationSucceeded(stockpileDesignator);
    }
}
