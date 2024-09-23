using yanplaRoles.Roles.Jackal;
using MiraAPI.GameOptions;
using System;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace yanplaRoles.Options.Roles;

public class JackalOptions : AbstractOptionGroup
{ 
    public override string GroupName => "Jackal";
    public override Color GroupColor => Color.cyan;
    public override Type AdvancedRole => typeof(Jackal);

    public ModdedToggleOption JackalCanVent { get; } = new("Jackal Can Vent", true);
}