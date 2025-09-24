using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Rewired;
using UnityEngine;
using yanplaRoles.rpc;

namespace yanplaRoles.Buttons.Amnesiac;

public class RememberButton : CustomActionButton<DeadBody>
{
    public override string Name => "";
    public override float Cooldown => 0f;
    public override float EffectDuration => 0f;
    public override int MaxUses => 0;
    public override MiraKeybind Keybind => MiraGlobalKeybinds.PrimaryAbility;
    public override ButtonLocation Location => ButtonLocation.BottomRight;

    public override LoadableAsset<Sprite> Sprite => Assets.Remember;

    protected override void OnClick()
    {
        if (Target != null)
        {
            if (PlayerControl.LocalPlayer.Data.Role is Roles.Neutral.Amnesiac amnesiacRole) {amnesiacRole.DestroyArrow();};
            PlayerControl.LocalPlayer.RpcAmnesiacRemember(Target.ParentId);
        }
    }

    public override DeadBody? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetNearestDeadBody(1f);
    }

    public override void SetOutline(bool active)
    {
        var amnesiacRole = new Roles.Neutral.Amnesiac();
        Target?.bodyRenderers[0].material.SetFloat("_Outline", active ? 1 : 0);
        Target?.bodyRenderers[0].material.SetColor("_OutlineColor", amnesiacRole.RoleColor);
    }

    public override bool IsTargetValid(DeadBody? target)
    {
        return true;
    }

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is Roles.Neutral.Amnesiac;
    }
}