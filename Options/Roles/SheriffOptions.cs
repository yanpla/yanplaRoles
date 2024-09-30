using yanplaRoles.Roles.Crewmate;
using MiraAPI.GameOptions;
using MiraAPI.Utilities;
using System;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace yanplaRoles.Options.Roles;

public class SheriffOptions : AbstractOptionGroup
{ 
    public override string GroupName => "Sheriff";
    public override Color GroupColor => Color.yellow;
    public override Type AdvancedRole => typeof(Sheriff);

    [ModdedNumberOption("Shoot Cooldown", 5, 60, 2.5f, MiraNumberSuffixes.Seconds)]
    public float ShootCooldown { get; set; } = 30;
}