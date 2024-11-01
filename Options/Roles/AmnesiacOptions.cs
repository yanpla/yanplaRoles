using yanplaRoles.Roles.Neutral;
using MiraAPI.GameOptions;
using System;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;

namespace yanplaRoles.Options.Roles;

public class AmnesiacOptions : AbstractOptionGroup
{ 
    public override string GroupName => "Amnesiac";
    public override Color GroupColor => new Color(0.5f, 0.7f, 1f, 1f);
    public override Type AdvancedRole => typeof(Amnesiac);

    [ModdedNumberOption("Arrow Appear Delay", 0, 30, 1f, MiraNumberSuffixes.Seconds)]
    public float ArrowAppearDelay { get; set; } = 5f;
}