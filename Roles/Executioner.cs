using MiraAPI.GameOptions;
using MiraAPI.Roles;
using UnityEngine;
using yanplaRoles.CustomGameOverReasons;
using yanplaRoles.Options.Roles;
using MiraAPI.Modifiers;
using yanplaRoles.Modifiers;
using MiraAPI.Utilities;
using Il2CppSystem.Runtime.InteropServices;

namespace yanplaRoles.Roles;

[RegisterCustomRole]
public class Executioner : CrewmateRole, ICustomRole
{
    public string RoleName => "Executioner";
    public string RoleDescription => "Get your target voted out";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => new Color(0.55f, 0.25f, 0.02f, 1f);
    public ModdedRoleTeams Team => ModdedRoleTeams.Neutral;
    public int MaxPlayers => 1;

    public CustomRoleConfiguration Configuration => new CustomRoleConfiguration(this)
    {
        TasksCountForProgress = false,
        CanGetKilled = true,
    };

    PlayerControl target;

    public void HudUpdate(HudManager hudManager)
    {
        if (target == null){
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!player.HasModifier<ExecutionerTarget>()) continue;

                target = player;
                target.SetName("(TARGET) " + player.Data.PlayerName);
                break;
            }
        } 
    }

    public bool IsModifierApplicable(BaseModifier modifier){
        return modifier is not ExecutionerTarget;
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return gameOverReason == (GameOverReason)CustomGameOverReasonsEnum.ExecutionerWin;
    }

}