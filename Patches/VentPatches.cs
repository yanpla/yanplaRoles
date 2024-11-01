using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.GameOptions;
using UnityEngine;
using yanplaRoles.Options;

namespace yanplaRoles.Patches;

[HarmonyPatch(typeof(Vent))]
public static class ventPatch
{
    private static bool HideVentInFog = OptionGroupSingleton<GameOptions>.Instance.HideVentInFog;
    [HarmonyPrefix, HarmonyPatch(nameof(Vent.EnterVent))]
        public static bool EnterVentPrefix(Vent __instance, PlayerControl pc)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek || PlayerControl.LocalPlayer.Data.IsDead) return true;

            if (pc.AmOwner)
            {
                Vent.currentVent = __instance;
                ConsoleJoystick.SetMode_Vent();
            }
            if (!__instance.EnterVentAnim)
                return false;

            Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            Vector2 vector = pc.GetTruePosition() - truePosition;
            float magnitude = vector.magnitude;

            if (pc != null && !HideVentInFog || HideVentInFog && magnitude < PlayerControl.LocalPlayer.lightSource.viewDistance &&
                !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, magnitude, Constants.ShipAndObjectsMask))
                __instance.myAnim.Play(__instance.EnterVentAnim, 1f);

            if (pc.AmOwner && Constants.ShouldPlaySfx())
            {
                SoundManager.Instance.StopSound(ShipStatus.Instance.VentEnterSound);
                SoundManager.Instance.PlaySound(ShipStatus.Instance.VentEnterSound, false, 1f).pitch = FloatRange.Next(0.8f, 1.2f);
            }

            return false;
        }

        [HarmonyPrefix, HarmonyPatch(nameof(Vent.ExitVent))]
        public static bool ExitVentPrefix(Vent __instance, PlayerControl pc)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek || PlayerControl.LocalPlayer.Data.IsDead) return true;

            if (pc.AmOwner)
            {
                Vent.currentVent = null;
            }
            if (!__instance.ExitVentAnim)
                return false;

            Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            Vector2 vector = pc.GetTruePosition() - truePosition;
            float magnitude = vector.magnitude;

            if (pc != null && !HideVentInFog || HideVentInFog && magnitude < PlayerControl.LocalPlayer.lightSource.viewDistance &&
                !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, magnitude, Constants.ShipAndObjectsMask))
                __instance.myAnim.Play(__instance.ExitVentAnim, 1f);

            if (pc.AmOwner && Constants.ShouldPlaySfx())
            {
                SoundManager.Instance.StopSound(ShipStatus.Instance.VentEnterSound);
                SoundManager.Instance.PlaySound(ShipStatus.Instance.VentEnterSound, false, 1f).pitch = FloatRange.Next(0.8f, 1.2f);
            }

            return false;
        }
}