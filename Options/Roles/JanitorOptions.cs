using yanplaRoles.Roles.Impostor;
using MiraAPI.GameOptions;
using System;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;

namespace yanplaRoles.Options.Roles;

public class JanitorOptions : AbstractOptionGroup
{ 
    public override string GroupName => "Janitor";
    public override Color GroupColor => Palette.ImpostorRed;
    public override Type AdvancedRole => typeof(Janitor);

    public ModdedToggleOption JanitorCanVent { get; } = new("Can Vent", true);
    
    [ModdedNumberOption("Clean Cooldown", 5, 60, 2.5f, MiraNumberSuffixes.Seconds)]
    public float CleanCooldown { get; set; } = 30;

    [ModdedNumberOption("Clean Uses", 0, 10, 1f)]
    public float CleanUses { get; set; } = 0;
}