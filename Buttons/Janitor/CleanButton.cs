using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using UnityEngine;
using yanplaRoles.Options.Roles;
using yanplaRoles.rpc;

namespace yanplaRoles.Buttons.Janitor;

[RegisterButton]
public class CleanButton : CustomActionButton<DeadBody>
{
    public override string Name => "";
    public override float Cooldown => OptionGroupSingleton<JanitorOptions>.Instance.CleanCooldown;
    public override float EffectDuration => 0f;
    public override int MaxUses => (int)OptionGroupSingleton<JanitorOptions>.Instance.CleanUses;
    public override LoadableAsset<Sprite> Sprite => Assets.CleanButton;

    protected override void OnClick()
    {
        if (Target != null)
        {
            PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
            PlayerControl.LocalPlayer.RpcCleanBody(Target.ParentId);
        }
    }

    public override DeadBody? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetNearestDeadBody(1f);
    }

    public override void SetOutline(bool active)
    {
        Target?.bodyRenderers[0].material.SetFloat("_Outline", active ? 1 : 0);
        Target?.bodyRenderers[0].material.SetColor("_OutlineColor", Palette.ImpostorRed);
    }

    public override bool IsTargetValid(DeadBody? target)
    {
        return true;
    }

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is Roles.Impostor.Janitor;
    }
}