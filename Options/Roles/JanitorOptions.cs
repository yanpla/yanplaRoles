using yanplaRoles.Roles.Impostor;
using MiraAPI.GameOptions;
using System;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace yanplaRoles.Options.Roles;

public class JanitorOptions : AbstractOptionGroup<Janitor>
{ 
    public override string GroupName => "Janitor";
    public override Color GroupColor => Palette.ImpostorRed;

    public ModdedToggleOption JanitorCanVent { get; } = new("Can Vent", true);
}