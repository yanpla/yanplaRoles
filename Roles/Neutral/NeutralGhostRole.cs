using MiraAPI.Roles;
using UnityEngine;


namespace yanplaRoles.Roles.Neutral;


public class NeutralGhostRole : CrewmateGhostRole, ICustomRole 
{
    public string RoleName => "Neutral Ghost";
    public string RoleDescription => "You're dead, enjoy the afterlife";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => Color.gray;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public CustomRoleConfiguration Configuration => new CustomRoleConfiguration(this)
    {
        HideSettings = true,
        TasksCountForProgress = false,
    };

    public override void SpawnTaskHeader(PlayerControl playerControl)
    {

    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return false;
    }

    public NeutralGhostRole()
    {
        this.SetCanDoTasks(false);
    }
}