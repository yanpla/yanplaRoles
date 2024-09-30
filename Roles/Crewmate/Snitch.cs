using System;
using System.Collections.Generic;
using Il2CppSystem.Runtime.InteropServices;
using MiraAPI.Roles;
using UnityEngine;

namespace yanplaRoles.Roles.Crewmate;

[RegisterCustomRole]
public class Snitch : CrewmateRole, ICustomRole
{
    public string RoleName => "Snitch";
    public string RoleDescription => "Complete your tasks to reveal the Impostor(s).";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => new Color(0.83f, 0.69f, 0.22f, 1f);
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public CustomRoleConfiguration Configuration => new CustomRoleConfiguration(this)
    {
        CanUseVent = false,
    };
    
    bool revealed;
    private HashSet<byte> impostorIds = new HashSet<byte>();

    public void HudUpdate(HudManager hudManager)
    {
        if (Player.AllTasksCompleted()) 
        {
            if (!revealed){
                RevealImpostorsToPlayer(Player);
                revealed = true;
            }
            else if (MeetingHud.Instance != null)
            {
                UpdateMeeting(MeetingHud.Instance);
            }
        }
        
    }

    private void RevealImpostorsToPlayer(PlayerControl player)
    {
        foreach (var impostor in PlayerControl.AllPlayerControls)
        {
            if (impostor.Data.IsDead || impostor.Data.Role.TeamType != RoleTeamTypes.Impostor) continue;

            impostor.cosmetics.nameText.color = Palette.ImpostorRed;
            impostorIds.Add(impostor.PlayerId);
        }
    }

    private void UpdateMeeting(MeetingHud __instance)
    {
        foreach (var player in __instance.playerStates)
        {
            foreach (byte impostorId in impostorIds)
            {
                if (player.TargetPlayerId == impostorId)
                {
                    player.NameText.color = Palette.ImpostorRed;
                }
            }
        }
    }
}
