using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace yanplaRoles.Options;

public class GameOptions : AbstractOptionGroup
{ 
    public override string GroupName => "Game";
    public override Color GroupColor => Color.white;
        
    [ModdedToggleOption("Hide Vent Animations in Fog")]
    public bool HideVentInFog { get; set; } = false;
}