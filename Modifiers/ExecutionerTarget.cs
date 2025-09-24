using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using yanplaRoles.CustomGameOverReasons;
using yanplaRoles.Roles.Neutral;
using MiraAPI.Roles;
using AmongUs.GameOptions;
using MiraAPI.Utilities;
using yanplaRoles.rpc;

namespace yanplaRoles.Modifiers;

public class ExecutionerTarget : GameModifier
{
    public override string ModifierName => "Executioner Target";
    public override bool HideOnUi => true;
    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return role.TeamType == RoleTeamTypes.Crewmate;
    }

    public override int GetAmountPerGame()
    {
        return 1;
    }

    public override int GetAssignmentChance()
    {
        return 0;
    }

    public override void OnDeath(DeathReason reason)
    {
        PlayerControl executioner = null;

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.Data.Role is Executioner)
            {
                executioner = player;
                break;
            }
        }

        if (executioner != null)
        {
            if (reason == DeathReason.Exile)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReasonsEnum.ExecutionerWin, false);
                return;
            }
            executioner.RpcChangeRole(RoleId.Get<Amnesiac>());
        }
    }


}