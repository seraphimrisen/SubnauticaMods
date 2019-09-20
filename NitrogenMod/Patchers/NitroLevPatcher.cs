using Harmony;
using NitrogenMod.NMBehaviours;
using UnityEngine;

namespace NitrogenMod.Patchers
{
    [HarmonyPatch(typeof(NitrogenLevel))]
    [HarmonyPatch("Update")]
    internal class NitroDamagePatcher
    {
        private static bool lethal = true;
        private static bool _cachedActive;
        private static bool _cachedAnimating;

        private static float damageScaler = 1f;
        
        [HarmonyPrefix]
        public static bool Prefix (ref NitrogenLevel __instance)
        {
            if (__instance.nitrogenEnabled && GameModeUtils.RequiresOxygen() && Time.timeScale > 0f)
            {
                float depthOf = Ocean.main.GetDepthOf(Player.main.gameObject);

                if (depthOf < __instance.safeNitrogenDepth - 10f && Random.value < 0.0125f)
                {
                    Utils.Assert(depthOf < __instance.safeNitrogenDepth);
                    LiveMixin component = Player.main.gameObject.GetComponent<LiveMixin>();
                    float damage = 1f + damageScaler * (__instance.safeNitrogenDepth - depthOf) / 10f;
                    if (component.health - damage > 0f && IsInDanger(Player.main, true))
                        component.TakeDamage(damage);
                    else if (lethal && IsInDanger(Player.main, true))
                        component.TakeDamage(damage);
                }

                if (__instance.safeNitrogenDepth > 10f && IsInDanger(Player.main) && Random.value < 0.025f)
                {
                    float atmosPressure = __instance.safeNitrogenDepth - 10f;
                    if (atmosPressure < 0f)
                        atmosPressure = 0f;
                    LiveMixin component = Player.main.gameObject.GetComponent<LiveMixin>();
                    float damage = 1f + damageScaler * (__instance.safeNitrogenDepth - atmosPressure) / 10f;
                    if (component.health - damage > 0f)
                        component.TakeDamage(damage);
                    else if (lethal)
                        component.TakeDamage(damage);
                }

                float num;
                if (depthOf < __instance.safeNitrogenDepth && !IsInDanger(Player.main))
                {
                    num = Mathf.Clamp(2f - __instance.GetComponent<Rigidbody>().velocity.magnitude, 0f, 2f) * 1f;
                    __instance.safeNitrogenDepth = UWE.Utils.Slerp(__instance.safeNitrogenDepth, depthOf, __instance.kDissipateScalar * num * Time.deltaTime);
                }
                else if (IsInDanger(Player.main) && __instance.safeNitrogenDepth > 0f)
                {
                    float atmosPressure = __instance.safeNitrogenDepth - 10f;
                    if (atmosPressure < 0f)
                        atmosPressure = 0f;
                    __instance.safeNitrogenDepth = UWE.Utils.Slerp(__instance.safeNitrogenDepth, atmosPressure, __instance.kDissipateScalar * 2f * Time.deltaTime);
                }
                HUDController(__instance);
            }
            return false;
        }

        private static bool IsInDanger(Player player, bool checkVehiclesOnly = false)
        {
            if (Main.SeamothSafe && player.inSeamoth)
                return false;
            if (Main.ExoSafe && player.inExosuit)
                return false;
            if (Main.SubSafe && player.currentSub.isCyclops)
                return false;
            return checkVehiclesOnly || !player.IsSwimming();
        }

        public static void Lethality(bool isLethal)
        {
            lethal = isLethal;
        }

        public static void AdjustScaler(float val)
        {
            damageScaler = val;
        }

        private static void HUDController(NitrogenLevel nitrogenInstance)
        {
            if (nitrogenInstance.safeNitrogenDepth > 10f && !_cachedActive)
            {
                BendsHUDController.SetActive(true);
                _cachedActive = true;
            }
            else if (nitrogenInstance.safeNitrogenDepth < 10f && _cachedActive)
            {
                BendsHUDController.SetActive(false);
                _cachedActive = false;
            }
            if (nitrogenInstance.safeNitrogenDepth > 25f && !_cachedAnimating)
            {
                BendsHUDController.SetFlashing(true);
                _cachedAnimating = true;
            }
            else if (nitrogenInstance.safeNitrogenDepth < 15f && _cachedAnimating)
            {
                BendsHUDController.SetFlashing(false);
                _cachedAnimating = false;
            }
        }
    }

    [HarmonyPatch(typeof(NitrogenLevel))]
    [HarmonyPatch("Start")]
    internal class NitroStartPatcher
    {
        [HarmonyPostfix]
        public static void Postfix(ref NitrogenLevel __instance)
        {
            __instance.nitrogenEnabled = Main.nitrogenEnabled;
            __instance.safeNitrogenDepth = 0f;
            
            if (Main.specialtyTanks)
                Player.main.gameObject.AddComponent<SpecialtyTanks>();
        }
    }

    [HarmonyPatch(typeof(NitrogenLevel))]
    [HarmonyPatch("OnRespawn")]
    internal class RespawnPatcher
    {
        [HarmonyPostfix]
        public static void Postfix(ref NitrogenLevel __instance)
        {
            __instance.safeNitrogenDepth = 0f;
        }
    }
}