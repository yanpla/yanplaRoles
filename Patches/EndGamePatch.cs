using System;
using System.Collections.Generic;
using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppSystem.Text;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace yanplaRoles.Patches;

static class AdditionalTempData {
        public static List<PlayerRoleInfo> playerRoleHistory = new List<PlayerRoleInfo>();

        public static void clear() {
            playerRoleHistory.Clear();
        }

        internal class PlayerRoleInfo
        {
            public string PlayerName { get; set; }
            public string Role { get; set; }
        }
    }


[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
public class OnGameEndPatch {
    public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] EndGameResult endGameResult)
    {
        AdditionalTempData.clear();
        foreach (var playerControl in PlayerControl.AllPlayerControls)
        {
            string playerRole = "";
            foreach (var role in Utils.GetPlayerRolesHistory(playerControl.PlayerId))
            {
                playerRole += $"<color=#{role.NameColor.ToHtmlStringRGBA()}>{role.NiceName}</color> > ";
            }
            playerRole = playerRole.Remove(playerRole.Length - 3);
            AdditionalTempData.playerRoleHistory.Add(new AdditionalTempData.PlayerRoleInfo { PlayerName = playerControl.Data.PlayerName, Role = playerRole });
        }
        Utils.ClearPlayerRolesHistory();
    }
}

[HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
public static class EndGameManagerSetUpPatch{
    public static void Postfix(EndGameManager __instance){
        if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return;

        var position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));
        GameObject roleSummary = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
        roleSummary.transform.position = new Vector3(__instance.Navigation.ExitButton.transform.position.x + 0.1f, position.y - 0.1f, -14f); 
        roleSummary.transform.localScale = new Vector3(1f, 1f, 1f);

        var roleSummaryText = new StringBuilder();
        roleSummaryText.AppendLine("End game summary:");
        foreach(var data in AdditionalTempData.playerRoleHistory) {
            var role = string.Join(" ", data.Role);
            roleSummaryText.AppendLine($"{data.PlayerName} - {role}");
        }

        TMPro.TMP_Text roleSummaryTextMesh = roleSummary.GetComponent<TMPro.TMP_Text>();
        roleSummaryTextMesh.alignment = TMPro.TextAlignmentOptions.TopLeft;
        roleSummaryTextMesh.color = Color.white;
        roleSummaryTextMesh.fontSizeMin = 1.5f;
        roleSummaryTextMesh.fontSizeMax = 1.5f;
        roleSummaryTextMesh.fontSize = 1.5f;
            
        var roleSummaryTextMeshRectTransform = roleSummaryTextMesh.GetComponent<RectTransform>();
        roleSummaryTextMeshRectTransform.anchoredPosition = new Vector2(position.x + 3.5f, position.y - 0.1f);
        roleSummaryTextMesh.text = roleSummaryText.ToString();
    }
}