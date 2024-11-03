using yanplaRoles.Roles.Neutral;
using MiraAPI.GameOptions;
using System;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace yanplaRoles.Options.Roles;

public class JesterOptions : AbstractOptionGroup
{ 
    public override string GroupName => "Jester";
    public override Color GroupColor => new Color32(236, 98, 165, byte.MaxValue);
    public override Type AdvancedRole => typeof(Jester);

    public ModdedToggleOption JesterCanVent { get; } = new("Jester Can Vent", false);
    public ModdedToggleOption JesterHasImpVision { get; } = new("Has Impostor vision", true);
}