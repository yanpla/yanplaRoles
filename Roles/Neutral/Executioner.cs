using MiraAPI.Roles;
using UnityEngine;
using yanplaRoles.CustomGameOverReasons;
using MiraAPI.Modifiers;
using yanplaRoles.Modifiers;
using MiraAPI.Utilities;
using AmongUs.GameOptions;

namespace yanplaRoles.Roles.Neutral;

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
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };

    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        // remove existing task header.
    }

    public override bool CanUse(IUsable usable)
	{
		if (!GameManager.Instance.LogicUsables.CanUse(usable, Player))
		{
			return false;
		}
		Console console = usable.Cast<Console>();
		return !(console != null) || console.AllowImpostor;
	}

    PlayerControl target;

    public void HudUpdate(HudManager hudManager)
    {
        if (target == null){
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!player.HasModifier<ExecutionerTarget>()) continue;
                target = player;
                target.cosmetics.nameText.color = Color.black;
                break;
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