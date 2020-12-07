namespace NitrogenMod
{
    using System;
    using System.Reflection;
    using System.Collections.Generic;
    using HarmonyLib;
    using SMLHelper.V2.Handlers;
    using Common;
    using Items;
    using UnityEngine;
    using Patchers;
    using Logger = QModManager.Utility.Logger;

    public class Main
    {
        public const string modName = "[NitrogenMod]";

        private const string modFolder = "./QMods/NitrogenMod/";
        private const string assetFolder = modFolder + "Assets/";
        private const string assetBundle = assetFolder + "n2warning";
        public static GameObject N2HUD { get; set; }

        public static bool specialtyTanks = true;
        public static bool nitrogenEnabled = true;
        public static bool decompressionVehicles = false;

        private static Dictionary<TechType, float> diveSuitDepths = new Dictionary<TechType, float>()
        {
            { TechType.None, 200f },
            { TechType.RadiationSuit, 500f },
            { TechType.ReinforcedDiveSuit, 800f },
            { TechType.Stillsuit, 800f }
        };

        private static Dictionary<TechType, float> diveSuitMinTemps = new Dictionary<TechType, float>()
        {
            { TechType.None, 0f },
            { TechType.ReinforcedDiveSuit, 15f }
        };

        private static Dictionary<TechType, float> diveSuitBreathModifiers = new Dictionary<TechType, float>()
        {
            { TechType.RadiationSuit, 0.95f },
            { TechType.Stillsuit, 0.95f },
            { TechType.ReinforcedDiveSuit, 0.85f }
        };

        public static void AddDiveSuit(TechType diveSuit, float depth, float breathMultiplier = 1f, float minTempBonus = 0f)
        {
            Logger.Log(Logger.Level.Info, $"Received AddDiveSuit call with Dive Suit {diveSuit.ToString()}, depth {depth}, breathMultiplier {breathMultiplier} and minTempBonus {minTempBonus}");
            if (diveSuitDepths.TryGetValue(diveSuit, out float value)) // We don't actually care about the value here, just whether it exists
            {
                Logger.Log(Logger.Level.Error, $"Received duplicated AddDiveSuit call for TechType {diveSuit.ToString()}");
                return;
            }
            diveSuitDepths.Add(diveSuit, depth);
            if (minTempBonus > 0)
                diveSuitMinTemps.Add(diveSuit, minTempBonus);
            if (breathMultiplier != 1f)
                diveSuitBreathModifiers.Add(diveSuit, breathMultiplier);
        }

        public static float GetDiveSuitDepth(TechType diveSuit)
        {
            if (diveSuitDepths.TryGetValue(diveSuit, out float depth))
                return depth;

            return 200f;
        }

        public static float GetDiveSuitBreathMult(TechType diveSuit)
        {
            if (diveSuitBreathModifiers.TryGetValue(diveSuit, out float breath))
                return breath;
            return 1.0f;
        }

        public static float GetDiveSuitTempBonus(TechType diveSuit)
        {
            if (diveSuitMinTemps.TryGetValue(diveSuit, out float tempBonus))
                return tempBonus;

            return 0f;
        }

        // The previous functions are for getting just one attribute at a time; this function is for when you need multiple attributes in succession.
        // Make a single call here instead of multiple calls to the helpers above.
        // Returns false if the suit's parameters could not be found.
        public static bool GetDiveSuitParameters(TechType diveSuit, out float depth, out float minTemp, out float breathMult)
        {
            // The only mandatory parameter is depth. If it has a depth value, it's valid.
            if (!diveSuitDepths.TryGetValue(diveSuit, out depth))
            {
                minTemp = 0f;
                breathMult = 0f;
                return false;
            }

            if (!diveSuitMinTemps.TryGetValue(diveSuit, out minTemp))
                minTemp = 0f;

            if (!diveSuitBreathModifiers.TryGetValue(diveSuit, out breathMult))
                breathMult = 1.0f;

            return true;
        }

        public static void Patch()
        {
            SeraLogger.PatchStart(modName, "1.5.1");
            try
            {
                var harmony = new Harmony("seraphimrisen.nitrogenmod.mod");

                AssetBundle ab = AssetBundle.LoadFromFile(assetBundle);
                N2HUD = ab.LoadAsset("NMHUD") as GameObject;

                NitrogenOptions savedSettings = new NitrogenOptions();
                OptionsPanelHandler.RegisterModOptions(savedSettings);

                nitrogenEnabled = savedSettings.nitroEnabled;
                decompressionVehicles = savedSettings.decompressionVehicles;
                NitroDamagePatcher.Lethality(savedSettings.nitroLethal);
                NitroDamagePatcher.AdjustScaler(savedSettings.damageScaler);
                NitroDamagePatcher.SetDecomVeh(decompressionVehicles);
                BreathPatcher.EnableCrush(savedSettings.crushEnabled);

                harmony.PatchAll(Assembly.GetExecutingAssembly());

                DummySuitItems.PatchDummyItems();
                ReinforcedSuitsCore.PatchSuits();
                if(specialtyTanks)
                    O2TanksCore.PatchTanks();

                AddDiveSuit(ReinforcedSuitsCore.ReinforcedStillSuit, 1300f, 0.75f, 15f);
                AddDiveSuit(ReinforcedSuitsCore.ReinforcedSuit2ID, 1300f, 0.75f, 20f);
                AddDiveSuit(ReinforcedSuitsCore.ReinforcedSuit3ID, 8000f, 0.55f, 35f);
                Console.WriteLine(typeof(NitroDamagePatcher).AssemblyQualifiedName);
                SeraLogger.PatchComplete(modName);
            }
            catch (Exception ex)
            {
                SeraLogger.PatchFailed(modName, ex);
            }
        }
    }
}