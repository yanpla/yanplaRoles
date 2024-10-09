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
}