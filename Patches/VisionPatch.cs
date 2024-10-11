using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.GameOptions;
using yanplaRoles.Options.Roles;
using yanplaRoles.Roles.Neutral;

namespace yanplaRoles.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
public static class VisionPatch {
    public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] NetworkedPlayerInfo player, ref float __result) {
        if (player == null || player.IsDead)
		{
			__result = __instance.MaxLightRadius;
            return false;
		}
		if (player.Role.IsImpostor 
        || player.Role is Arsonist && OptionGroupSingleton<ArsonistOptions>.Instance.ArsonistHasImpVision.Value
        || player.Role is Jester && OptionGroupSingleton<JesterOptions>.Instance.JesterHasImpVision.Value)
		{
			__result = __instance.MaxLightRadius * GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.ImpostorLightMod);
            return false;
		}
        return true;
    }

}