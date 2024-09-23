using MiraAPI.Roles;
using UnityEngine;
using yanplaRoles.Options.Roles;
using MiraAPI.GameOptions;


namespace yanplaRoles.Roles.Jackal;

[RegisterCustomRole]
public class Jackal : ImpostorRole, ICustomRole
{
    public string RoleName => "Jackal";
    public string RoleDescription => "Neutral who can kill.";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => Color.cyan;
    public ModdedRoleTeams Team => ModdedRoleTeams.Neutral;

    public CustomRoleConfiguration Configuration => new CustomRoleConfiguration(this)
    {
        UseVanillaKillButton = true,
        CanGetKilled = true,
        CanUseVent = OptionGroupSingleton<JackalOptions>.Instance.JackalCanVent.Value,
    };

    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        // remove existing task header.
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return GameManager.Instance.DidHumansWin(gameOverReason);
    }
}