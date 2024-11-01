using AmongUs.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using UnityEngine;
using yanplaRoles.Modifiers;

namespace yanplaRoles.Roles.Neutral;

[RegisterCustomRole]
public class Amnesiac : CrewmateRole, ICustomRole
{
    public string RoleName => "Amnesiac";
    public string RoleDescription => "Remember A Role Of A Deceased Player";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => new Color(0.5f, 0.7f, 1f, 1f);
    public ModdedRoleTeams Team => ModdedRoleTeams.Neutral;

    public CustomRoleConfiguration Configuration => new CustomRoleConfiguration(this)
    {
        TasksCountForProgress = false,
        CanGetKilled = true,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };

    public bool IsModifierApplicable(BaseModifier modifier){
        return modifier is not ExecutionerTarget;
    }

    public override bool DidWin(GameOverReason gameOverReason) { return false; }
}