﻿namespace NitrogenMod.Patchers
{
    using HarmonyLib;
    using Items;
    using System.Collections;
    using System.Collections.Generic;

    // Code provided by AlexejheroYTB to remove a destructive prefix
    [HarmonyPatch(typeof(Player), nameof(Player.HasReinforcedSuit))]
    internal static class HasReinforcedSuitPatcher
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            var main = Inventory.main.equipment;
            __result = __result || main.GetCount(ReinforcedSuitsCore.ReinforcedStillSuit) + main.GetCount(ReinforcedSuitsCore.ReinforcedSuit2ID) + main.GetCount(ReinforcedSuitsCore.ReinforcedSuit3ID) > 0;
        }
    }

    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("UpdateReinforcedSuit")]
    internal class UpdateReinforcedSuitPatcher
    {
        [HarmonyPrefix]
        public static bool Prefix (ref Player __instance)
        {
            __instance.temperatureDamage.minDamageTemperature = 49f;
            TechType bodySlot = Inventory.main.equipment.GetTechTypeInSlot("Body");
            float crushDepth = Main.GetDiveSuitDepth(bodySlot);
            float minTempBonus = Main.GetDiveSuitTempBonus(bodySlot);
            /*if (bodySlot == TechType.RadiationSuit)
                crushDepth = 500f;
            else if (bodySlot == TechType.ReinforcedDiveSuit)
            {
                __instance.temperatureDamage.minDamageTemperature += 15f;
                crushDepth = 800f;
            }
            else if (bodySlot == TechType.Stillsuit)
                crushDepth = 800f;
            else if (bodySlot == ReinforcedSuitsCore.ReinforcedStillSuit)
            {
                __instance.temperatureDamage.minDamageTemperature += 15f;
                crushDepth = 1300f;
            }
            else if (bodySlot == ReinforcedSuitsCore.ReinforcedSuit2ID)
            {
                __instance.temperatureDamage.minDamageTemperature += 20f;
                crushDepth = 1300f;
            }
            else if (bodySlot == ReinforcedSuitsCore.ReinforcedSuit3ID)
            {
                __instance.temperatureDamage.minDamageTemperature += 35f;
                crushDepth = 8000f;
            }*/

            if (__instance.HasReinforcedGloves())
            {
                minTempBonus += 6f;
            }
            PlayerGetDepthClassPatcher.divingCrushDepth = crushDepth;
            __instance.temperatureDamage.minDamageTemperature += minTempBonus;

            if (crushDepth < 8000f)
                ErrorMessage.AddMessage("Safe diving depth now " + crushDepth.ToString() + ".");
            else
                ErrorMessage.AddMessage("Safe diving depth now unlimited.");
            
            return false;
        }
    }
}
