using HarmonyLib;
using UnityEngine;
using yanplaRoles.Roles.Crewmate;

namespace yanplaRoles.Patches;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
public static class ShowRolesInMeetingPatch
{
    public static void Postfix(MeetingHud __instance)
    {
        var player = PlayerControl.LocalPlayer;

        foreach (PlayerVoteArea playerVoteArea in __instance.playerStates)
        {
            var targetPlayer = Utils.PlayerById(playerVoteArea.TargetPlayerId);
            var role = targetPlayer.Data.IsDead ? Utils.GetPlayerLastRole(targetPlayer.PlayerId) : targetPlayer.Data.Role;
            playerVoteArea.ColorBlindName.transform.localPosition = new Vector3(-0.93f, -0.2f, -0.1f);

            if (
                playerVoteArea.TargetPlayerId == player.PlayerId && 
                !player.Data.IsDead || 
                player.Data.IsDead || 
                player.Data.Role.IsImpostor &&
                targetPlayer.Data.Role is Snitch snitch && snitch.revealed
            ){
            playerVoteArea.NameText.color = role.NameColor;
            playerVoteArea.NameText.text = targetPlayer.Data.PlayerName + "\n" + role.NiceName;
            }
        }
    }
}