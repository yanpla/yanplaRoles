using MiraAPI.GameOptions;
using MiraAPI.Roles;
using UnityEngine;
using yanplaRoles.CustomGameOverReasons;
using yanplaRoles.Options.Roles;

namespace yanplaRoles.Roles.Jester;

[RegisterCustomRole]
public class Jester : CrewmateRole, ICustomRole
{
    public string RoleName => "Jester";
    public string RoleDescription => "Get voted out to win";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => new Color32(236, 98, 165, byte.MaxValue);
    public ModdedRoleTeams Team => ModdedRoleTeams.Neutral;

    public CustomRoleConfiguration Configuration => new CustomRoleConfiguration(this)
    {
        OptionsScreenshot = Assets.JesterBanner,
        TasksCountForProgress = false,
        CanUseVent = OptionGroupSingleton<JesterOptions>.Instance.JesterCanVent.Value,
    };

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
}