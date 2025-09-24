using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Rewired;
using UnityEngine;
using yanplaRoles.rpc;

namespace yanplaRoles.Buttons.Janitor;

public class CleanButton : CustomActionButton<DeadBody>
{
    public override string Name => "";
    public override float Cooldown => 30f;
    public override float EffectDuration => 0f;
    public override int MaxUses => 0;
    public override MiraKeybind Keybind => MiraGlobalKeybinds.PrimaryAbility;
    public override ButtonLocation Location => ButtonLocation.BottomRight;
    public override LoadableAsset<Sprite> Sprite => Assets.CleanButton;

    public override void FixedUpdateHandler(PlayerControl playerControl)
    {
        if (Timer >= 0)
        {
            Timer = PlayerControl.LocalPlayer.killTimer;
        }
        else if (HasEffect && EffectActive)
        {
            EffectActive = false;
            Timer = Cooldown;
            OnEffectEnd();
        }

        if (CanUse())
        {
            Button?.SetEnabled();
        }
        else
        {
            Button?.SetDisabled();
        }

        Button?.SetCoolDown(Timer, EffectActive ? EffectDuration : Cooldown);

        FixedUpdate(playerControl);
    }

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