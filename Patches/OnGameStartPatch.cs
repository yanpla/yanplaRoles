using HarmonyLib;

namespace yanplaRoles.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnGameStart))]
public static class OnGameStartPatch
{
    public static void Postfix(PlayerControl __instance)
    {
        var player = __instance;
        if (player.Data != null)
        {
            Utils.SavePlayerRole(player.Data.PlayerId, player.Data.Role);
        }
    }
}