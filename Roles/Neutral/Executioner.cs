using MiraAPI.Roles;
using UnityEngine;
using yanplaRoles.CustomGameOverReasons;
using MiraAPI.Modifiers;
using yanplaRoles.Modifiers;
using MiraAPI.Utilities;
using AmongUs.GameOptions;
using Reactor.Utilities.Extensions;
using System.Linq;
using yanplaRoles.rpc;

namespace yanplaRoles.Roles.Neutral;

[RegisterCustomRole]
public class Executioner : CrewmateRole, ICustomRole
{
    public string RoleName => "Executioner";
    public string RoleDescription => "Get your target voted out";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => new Color(0.55f, 0.25f, 0.02f, 1f);
    public ModdedRoleTeams Team => ModdedRoleTeams.Neutral;

    public CustomRoleConfiguration Configuration => new CustomRoleConfiguration(this)
    {
        TasksCountForProgress = false,
        CanGetKilled = true,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        MaxRoleCount = 1
    };

    public Executioner()
    {
        this.SetCanDoTasks(false);
    }

    PlayerControl target;

    public void HudUpdate(HudManager hudManager)
    {
        if (target == null)
        {
            var aliveCrewmates = PlayerControl.AllPlayerControls.ToArray().Where(p => !p.Data.IsDead && p.Data.Role.TeamType == RoleTeamTypes.Crewmate).ToArray();
            target = aliveCrewmates.Where(p => p.HasModifier<ExecutionerTarget>()).FirstOrDefault();
            if (target != null) target.cosmetics.nameText.color = Color.black;
            else
            {
                aliveCrewmates = aliveCrewmates.Where(p => !(p.Data.Role is ICustomRole role) || role.IsModifierApplicable(new ExecutionerTarget())).ToArray();
                if (aliveCrewmates.Length > 0)
                {
                    target = aliveCrewmates.Random<PlayerControl>();
                    target.RpcAddModifier<ExecutionerTarget>();
                    target.cosmetics.nameText.color = Color.black;
                }
                else
                {
                    PlayerControl.LocalPlayer.RpcChangeRole(0); // Crewmate
                }
            }
            
        }
        else if (MeetingHud.Instance != null)
        {
            UpdateMeeting(MeetingHud.Instance, target);
        }
    }

    private static void UpdateMeeting(MeetingHud __instance, PlayerControl target)
    {
        foreach (var player in __instance.playerStates)
            if (player.TargetPlayerId == target.PlayerId)
                player.NameText.color = Color.black;
    }

    public bool IsModifierApplicable(BaseModifier modifier){
        return modifier is not ExecutionerTarget;
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return gameOverReason == (GameOverReason)CustomGameOverReasonsEnum.ExecutionerWin;
    }

}