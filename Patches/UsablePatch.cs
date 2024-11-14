using HarmonyLib;
using MiraAPI.Roles;
using yanplaRoles;

[HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
    public static class ConsoleCanUsePatch
    {
        public static bool Prefix(ref float __result, Console __instance, [HarmonyArgument(0)] NetworkedPlayerInfo pc, [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse)
        {
            canUse = couldUse = false;
            if (pc.Role is not ICustomRole role) return true;
            bool canDoTasks = role.GetCanDoTasks();
            if (__instance.AllowImpostor || canDoTasks) return true;
            __result = float.MaxValue;
            return false;
        }
    }