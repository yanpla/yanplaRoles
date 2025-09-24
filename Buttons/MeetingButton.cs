using yanplaRoles.Modifiers;
using MiraAPI.Hud;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using Rewired;
using yanplaRoles.Options.Modifiers;
using UnityEngine;

namespace yanplaRoles.Buttons;

public class MeetingButton : CustomActionButton
{
    public override string Name => "Call Meeting";

    public override float Cooldown => 15;

    public override float EffectDuration => 0;

    public override int MaxUses => (int)OptionGroupSingleton<CaptainOptions>.Instance.CaptainAbilityUses;
    public override MiraKeybind Keybind => MiraGlobalKeybinds.ModifierPrimaryAbility;
    public override ButtonLocation Location => ButtonLocation.BottomRight;

    public override LoadableAsset<Sprite> Sprite => Assets.EmergencyButton;

    public override bool Enabled(RoleBehaviour? role)
    {
        return PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.HasModifier<Captain>();
    }

    protected override void OnClick()
    {
        var bt = ShipStatus.Instance.EmergencyButton;

        PlayerControl.LocalPlayer.NetTransform.Halt();
        var minigame = Object.Instantiate(bt.MinigamePrefab, Camera.main!.transform, false);

        var taskAdderGame = minigame as TaskAdderGame;
        if (taskAdderGame != null)
        {
            taskAdderGame.SafePositionWorld = bt.SafePositionLocal + (Vector2)bt.transform.position;
        }

        minigame.transform.localPosition = new Vector3(0f, 0f, -50f);
        minigame.Begin(null);

        if (UsesLeft == 0 && PlayerControl.LocalPlayer.HasModifier<Captain>())
        {
            PlayerControl.LocalPlayer.RpcRemoveModifier<Captain>();
        }
    }
}