using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Rewired;
using UnityEngine;
using yanplaRoles.Options.Roles;

namespace yanplaRoles.Buttons.Arsonist;

public class IgniteButton : CustomActionButton<PlayerControl>
{
    public override string Name => "";
    public override float Cooldown => OptionGroupSingleton<ArsonistOptions>.Instance.DouseCooldown;
    public override float EffectDuration => 0f;
    public override int MaxUses => 0;
    public override MiraKeybind Keybind => MiraGlobalKeybinds.SecondaryAbility;
    public override ButtonLocation Location => ButtonLocation.BottomRight;

    public override LoadableAsset<Sprite> Sprite => Assets.Ignite;

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is Roles.Neutral.Arsonist;
    }

    public override void FixedUpdateHandler(PlayerControl playerControl)
    {
        var arsonist = playerControl.Data.Role as Roles.Neutral.Arsonist;
        if (arsonist.ResetTimer.Ignite){
            ResetCooldownAndOrEffect();
            arsonist.ResetTimer.Ignite = false;
        }
        if (Timer >= 0)
        {
            Timer -= Time.deltaTime;
        }
        else if (HasEffect && EffectActive)
        {
            EffectActive = false;
            Timer = Cooldown;
            OnEffectEnd();
        }

        if (CanUse() && arsonist.DousedAlive > 0)
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
        if (Target == null) return;
        var arsonist = PlayerControl.LocalPlayer.Data.Role as Roles.Neutral.Arsonist;
        arsonist.Ignite();
        arsonist.ResetTimer.Douse = true;
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestPlayer(true, Distance);
    }

    public override void SetOutline(bool active)
    {
        Target?.cosmetics.SetOutline(active, new Il2CppSystem.Nullable<Color>(CustomRoleSingleton<Roles.Neutral.Arsonist>.Instance.RoleColor));
    }

     public override bool IsTargetValid(PlayerControl? target)
    {
        if (target == null) return false;
        var arsonist = PlayerControl.LocalPlayer.Data.Role as Roles.Neutral.Arsonist;
        return arsonist.dousedPlayers.Contains(target.PlayerId);
    }
}