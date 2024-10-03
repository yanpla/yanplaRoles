using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using MiraAPI.PluginLoading;
using Reactor;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using Reactor.Utilities;

namespace yanplaRoles;

[BepInAutoPlugin("yanplaRoles", "yanplaRoles")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
public partial class YanplaRolesPlugin : BasePlugin, IMiraPlugin
{
    public Harmony Harmony { get; } = new(Id);
    public const string VersionString = "0.0.7";
    public string OptionsTitleText => "yanplaRoles";
    public ConfigFile GetConfigFile() => Config;
    public override void Load()
    {
        ReactorCredits.Register("yanplaRoles", "v" + VersionString, false, ReactorCredits.AlwaysShow);
        Harmony.PatchAll();
    }
}