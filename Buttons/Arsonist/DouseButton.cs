using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using UnityEngine;
using yanplaRoles.Options.Roles;

namespace yanplaRoles.Buttons.Arsonist;

[RegisterButton]
public class DouseButton : CustomActionButton<PlayerControl>
{
    public override string Name => "";
    public override float Cooldown => OptionGroupSingleton<ArsonistOptions>.Instance.DouseCooldown;
    public override float EffectDuration => 0f;
    public override int MaxUses => 0;
    public override LoadableAsset<Sprite> Sprite => Assets.Douse;
    public override ButtonLocation Location => ButtonLocation.BottomRight;

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is Roles.Neutral.Arsonist;
    }

    public override void FixedUpdateHandler(PlayerControl playerControl)
    {
        var arsonist = playerControl.Data.Role as Roles.Neutral.Arsonist;
        if (arsonist.ResetTimer.Douse){
            ResetCooldownAndOrEffect();
            arsonist.ResetTimer.Douse = false;
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
        if (Target == null) return;

        var arsonist = PlayerControl.LocalPlayer.Data.Role as Roles.Neutral.Arsonist;

        arsonist.dousedPlayers.Add(Target.PlayerId);
        arsonist.ResetTimer.Ignite = true;
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
        if (arsonist.dousedPlayers.Contains(target.PlayerId)) return false;
        if (arsonist.DousedAlive >= OptionGroupSingleton<ArsonistOptions>.Instance.MaxDousedPlayers) return false;
        return true;
    }
}