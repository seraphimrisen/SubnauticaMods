using System;
using System.Reflection;
using Common;
using Harmony;
using NitrogenMod.Items;
using NitrogenMod.Patchers;
using SMLHelper.V2.Handlers;
using UnityEngine;

namespace NitrogenMod
{
    public class Main
    {
        public const string modName = "[NitrogenMod]";

        private const string modFolder = "./QMods/NitrogenMod/";
        private const string assetFolder = modFolder + "Assets/";
        private const string assetBundle = assetFolder + "n2warning";
        public static GameObject N2HUD { get; set; }

        public static bool specialtyTanks = true;
        public static bool nitrogenEnabled = true;
        public static bool SeamothSafe = true;
        public static bool ExoSafe = true;
        public static bool SubSafe = false;

        public static void Patch()
        {
            SeraLogger.PatchStart(modName, "1.4.2");
            try
            {
                var harmony = HarmonyInstance.Create("seraphimrisen.nitrogenmod.mod");

                AssetBundle ab = AssetBundle.LoadFromFile(assetBundle);
                N2HUD = ab.LoadAsset("NMHUD") as GameObject;

                NitrogenOptions savedSettings = new NitrogenOptions();
                OptionsPanelHandler.RegisterModOptions(savedSettings);

                NitroDamagePatcher.Lethality(savedSettings.nitroLethal);
                NitroDamagePatcher.AdjustScaler(savedSettings.damageScaler);
                BreathPatcher.EnableCrush(savedSettings.crushEnabled);
                nitrogenEnabled = savedSettings.nitroEnabled;

                harmony.PatchAll(Assembly.GetExecutingAssembly());

                DummySuitItems.PatchDummyItems();
                ReinforcedSuitsCore.PatchSuits();
                if(specialtyTanks)
                    O2TanksCore.PatchTanks();
                
                SeraLogger.PatchComplete(modName);
            }
            catch (Exception ex)
            {
                SeraLogger.PatchFailed(modName, ex);
            }
        }
    }
}