using MiraAPI.GameOptions;
using MiraAPI.Roles;
using UnityEngine;
using yanplaRoles.Options.Roles;

namespace yanplaRoles.Roles.Impostor;

[RegisterCustomRole]
public class Janitor : ImpostorRole, ICustomRole
{
    public string RoleName => "Janitor";
    public string RoleDescription => "Clean up bodies";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => Palette.ImpostorRed;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;

    public CustomRoleConfiguration Configuration => new CustomRoleConfiguration(this)
    {
        CanUseVent = OptionGroupSingleton<JanitorOptions>.Instance.JanitorCanVent.Value,
        OptionsScreenshot = Assets.JanitorBanner,
    };
}