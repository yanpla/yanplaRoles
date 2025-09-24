using MiraAPI.GameOptions;
using MiraAPI.Utilities;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;
using yanplaRoles.Modifiers;

namespace yanplaRoles.Options.Modifiers;

public class CaptainOptions : AbstractOptionGroup<Captain>
{ 
    public override string GroupName => "Captain";
    public override Color GroupColor => new Color32(255, 215, 0, byte.MaxValue);
        
    [ModdedNumberOption("Captain Chance", min: 0, max: 100, 10f, MiraNumberSuffixes.Percent)]
    public float CaptainChance { get; set; } = 100f;

    [ModdedNumberOption("Captain Ability uses", min: 1, max: 10, 1f)]
    public float CaptainAbilityUses { get; set; } = 1f;
}