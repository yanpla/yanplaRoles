using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
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
        return (int)OptionGroupSingleton<ModifierOptions>.Instance.CaptainChance;
    }
}