using AmongUs.GameOptions;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using UnityEngine;
using yanplaRoles.CustomGameOverReasons;
using yanplaRoles.Modifiers;
using yanplaRoles.Options.Roles;

namespace yanplaRoles.Roles.Neutral;

[RegisterCustomRole]
public class Jester : CrewmateRole, ICustomRole
{
    public string RoleName => "Jester";
    public string RoleDescription => "Get voted out to win";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => new Color32(236, 98, 165, byte.MaxValue);
    public ModdedRoleTeams Team => ModdedRoleTeams.Neutral;
    public int MaxPlayers => 1;

    public CustomRoleConfiguration Configuration => new CustomRoleConfiguration(this)
    {
        OptionsScreenshot = Assets.JesterBanner,
        TasksCountForProgress = false,
        CanUseVent = OptionGroupSingleton<JesterOptions>.Instance.JesterCanVent.Value,
        CanGetKilled = true,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };

    public Jester()
    {
        this.SetCanDoTasks(false);
    }

    public string GetCustomEjectionMessage(NetworkedPlayerInfo player)
    {
        return $"The joke's on you! {player.PlayerName} fooled everyone!";
    }

    public override void OnDeath(DeathReason reason)
    {
        if (reason == DeathReason.Exile)
        {
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReasonsEnum.JesterByVote, false);
        }
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return gameOverReason == (GameOverReason)CustomGameOverReasonsEnum.JesterByVote;
    }

    public bool IsModifierApplicable(BaseModifier modifier){
        return modifier is not ExecutionerTarget;
    }
}