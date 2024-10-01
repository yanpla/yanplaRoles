using HarmonyLib;
using MiraAPI.Roles;

namespace yanplaRoles.Patches
{
    [HarmonyPatch(typeof(HauntMenuMinigame), nameof(HauntMenuMinigame.SetFilterText))]
    public static class HauntMenuMinigamePatch
    {
        public static bool Prefix(HauntMenuMinigame __instance)
        {
            var role = __instance.HauntTarget.Data.Role;
            if (role is ICustomRole customRole)
            {
                __instance.FilterText.text = customRole.RoleName;
                return false;
            }
            __instance.FilterText.text = __instance.HauntTarget.Data.Role.NiceName;
            return false;
        }
    }
}