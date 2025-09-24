using System.Linq;
using UnityEngine;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MiraAPI.Modifiers;
using yanplaRoles.rpc;
using MiraAPI.Networking;

namespace yanplaRoles.Modifiers.Guesser;

public class GuesserKill
{
    public static void RpcMurderPlayer(PlayerControl player, PlayerControl guesser)
    {
        PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
            x => x.TargetPlayerId == player.PlayerId
        );
        RpcMurderPlayer(voteArea, player, guesser);
    }
    public static void RpcMurderPlayer(PlayerVoteArea voteArea, PlayerControl player, PlayerControl guesser)
    {
        AssassinKillCount(player, guesser);
        guesser.RpcGuesserKill(player.PlayerId);
    }

    public static void MurderPlayer(PlayerControl player, bool checkLover = true)
    {
        PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
            x => x.TargetPlayerId == player.PlayerId
        );
        MurderPlayer(voteArea, player, checkLover);
    }
    public static void AssassinKillCount(PlayerControl player, PlayerControl guesser)
    {
        var assassinPlayer = guesser.GetModifier<Guesser>();
        if (player == guesser) assassinPlayer.IncorrectAssassinKills += 1;
        else assassinPlayer.CorrectAssassinKills += 1;
    }
    public static void MurderPlayer(
        PlayerVoteArea voteArea,
        PlayerControl player,
        bool checkLover = true
    )
    {
        var hudManager = DestroyableSingleton<HudManager>.Instance;
        if (checkLover)
        {
            SoundManager.Instance.PlaySound(player.KillSfx, false, 0.8f);
            hudManager.KillOverlay.ShowKillAnimation(player.Data, player.Data);
        }
        
        player.CustomMurder(player, MurderResultFlags.Succeeded, false, false, false, false, false);

        if (voteArea == null) return;
        if (voteArea.DidVote) voteArea.UnsetVote();
        voteArea.AmDead = true;
        voteArea.Overlay.gameObject.SetActive(true);
        voteArea.Overlay.color = Color.white;
        voteArea.XMark.gameObject.SetActive(true);
        voteArea.XMark.transform.localScale = Vector3.one;

        var meetingHud = MeetingHud.Instance;
        if (player.AmOwner)
        {
            meetingHud.SetForegroundForDead();
        }

        foreach (var playerVoteArea in meetingHud.playerStates)
        {
            if (playerVoteArea.VotedFor != player.PlayerId) continue;
            playerVoteArea.UnsetVote();
            var voteAreaPlayer = Utils.PlayerById(playerVoteArea.TargetPlayerId);
            if (!voteAreaPlayer.AmOwner) continue;
            meetingHud.ClearVote();
        }

        if (AmongUsClient.Instance.AmHost) meetingHud.CheckForEndVoting();
    }
}