using MiraAPI.Utilities.Assets;

namespace yanplaRoles;

public static class Assets
{    
    public static LoadableResourceAsset JesterBanner { get; } = new("yanplaRoles.Resources.JesterBanner.png");
    public static LoadableResourceAsset ExampleButton { get; } = new("yanplaRoles.Resources.ExampleButton.png");
    public static LoadableResourceAsset SheriffBanner { get; } = new("yanplaRoles.Resources.SheriffBanner.png");
    public static LoadableResourceAsset EmergencyButton { get; } = new("yanplaRoles.Resources.EmergencyButton.png");
}