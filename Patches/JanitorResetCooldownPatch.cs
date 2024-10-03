using HarmonyLib;
using MiraAPI.Roles;
using yanplaRoles.Roles.Impostor;
using yanplaRoles.Buttons.Janitor;
using MiraAPI.Hud;

namespace yanplaRoles.Patches;

[HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
public static class JanitorResetCooldownPatch
{
    public static void Postfix(KillButton __instance)
    {
        if (PlayerControl.LocalPlayer.Data.Role is Janitor)
        {
            CustomButtonSingleton<CleanButton>.Instance.ResetCooldownAndOrEffect();
        }
    }
}
