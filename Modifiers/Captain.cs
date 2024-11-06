using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using MiraAPI.Utilities;
using yanplaRoles.Options.Modifiers;

namespace yanplaRoles.Modifiers;

[RegisterModifier]
public class Captain : GameModifier
{
    public override string ModifierName => "Captain";

    public override int GetAmountPerGame()
    {
        return 1;
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<CaptainOptions>.Instance.CaptainChance;
    }

    public override void OnDeath(DeathReason reason)
    {
        PlayerControl.LocalPlayer.RpcRemoveModifier<Captain>();
    }
}