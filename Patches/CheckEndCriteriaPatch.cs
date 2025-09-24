using System.Linq;
using AmongUs.GameOptions;
using HarmonyLib;
using yanplaRoles.Roles.Neutral;

namespace yanplaRoles.Patches;

[HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
public static class CheckEndCriteriaPatch
{
    public static bool Prefix(LogicGameFlowNormal __instance)
    {
        if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek || DestroyableSingleton<TutorialManager>.InstanceExists) return true;
        if (!AmongUsClient.Instance.AmHost) return false;
        if (ShipStatus.Instance.Systems != null)
        {
            if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.LifeSupp))
            {
                var lifeSuppSystemType = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                if (lifeSuppSystemType.Countdown < 0f) return true;
            }

            if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Laboratory))
            {
                var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                if (reactorSystemType.Countdown < 0f) return true;
            }

            if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Reactor))
            {
                var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ICriticalSabotage>();
                if (reactorSystemType.Countdown < 0f) return true;
            }
        }

        if (GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks)
        {
            GameManager.Instance.RpcEndGame(GameOverReason.CrewmatesByTask, false);
            return false;
        }
        
        var result = true;
        var alives = PlayerControl.AllPlayerControls.ToArray()
            .Where(x => !x.Data.IsDead && !x.Data.Disconnected);

        var roleStopsEnd = alives.Any(player => player.Data.Role is Arsonist arsonist && !arsonist.GameEnd(__instance));
        if (roleStopsEnd) result = false;

        return result;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
    public static void Postfix(LogicGameFlowNormal __instance, ref bool __result)
    {
        __result = false;
    }
}

