using MiraAPI.Hud;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using UnityEngine;
using yanplaRoles.Options.Roles;
using MiraAPI.GameOptions;

namespace yanplaRoles.Buttons.Sheriff;

[RegisterButton]
public class SheriffButton : CustomActionButton<PlayerControl>
{
    public override string Name => "Shoot";
    public override float Cooldown => OptionGroupSingleton<SheriffOptions>.Instance.ShootCooldown;
    public override float EffectDuration => 0f;
    public override int MaxUses => 0; // 0 means unlimited uses.
    public override LoadableAsset<Sprite> Sprite => Assets.ExampleButton;

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is Roles.Sheriff; // only show button when the player has Sheriff role.
    }
    protected override void OnClick()
    {
        Debug.Log("Sheriff button clicked.");
        
        if (Target == null)
        {
            Debug.LogWarning("No target selected.");
            return;
        }

        if (Target.Data.Role.TeamType == RoleTeamTypes.Impostor)
        {
            Debug.Log("Target is an Impostor. Sheriff is shooting the Impostor.");
            PlayerControl.LocalPlayer.RpcMurderPlayer(Target, true);
        }
        else
        {
            Debug.Log("Target is not an Impostor. Sheriff is shooting themselves.");
            PlayerControl.LocalPlayer.RpcMurderPlayer(PlayerControl.LocalPlayer, true);
        }
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestPlayer(true, Distance);
    }

    public override void SetOutline(bool active)
    {
        Target?.cosmetics.SetOutline(active, new Il2CppSystem.Nullable<Color>(Palette.ImpostorRed));
    }

     public override bool IsTargetValid(PlayerControl? target)
    {
        return true;
    }
}