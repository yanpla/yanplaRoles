using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using System.Collections;
using BepInEx;
using BepInEx.Unity.IL2CPP.Utils;
using Twitch;

namespace yanplaRoles.Patches
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public class AutoUpdater
    {
        private static readonly string GitHubApiUrl = "https://api.github.com/repos/yanpla/yanplaRoles/releases/latest";
        private static string latestVersion = "";
        private static string downloadUrl = "";
        public static GenericPopup InfoPopup;
        private static readonly string modPath = Path.Combine(Paths.PluginPath, "yanplaRoles.dll");

        private static void Postfix(MainMenuManager __instance)
        {
            TwitchManager man = DestroyableSingleton<TwitchManager>.Instance;
            InfoPopup = Object.Instantiate(man.TwitchPopup);
            __instance.StartCoroutine(CheckForUpdatesWithUnity(__instance));
        }

        public static IEnumerator CheckForUpdatesWithUnity(MainMenuManager instance)
        {
            UnityWebRequest request = UnityWebRequest.Get(GitHubApiUrl);
            request.SetRequestHeader("User-Agent", "request");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var json = JObject.Parse(request.downloadHandler.text);
                latestVersion = json["tag_name"].ToString();
                downloadUrl = json["assets"][0]["browser_download_url"].ToString();

                if (latestVersion != YanplaRolesPlugin.VersionString)
                {
                    Debug.Log($"[yanplaRoles] New version {latestVersion} available! Downloading...");
                    InfoPopup.Show("Updating yanplaRoles...");
                    string tempPath = Path.Combine(Application.persistentDataPath, "yanplaRoles_new.dll");
                    instance.StartCoroutine(DownloadUpdate(downloadUrl, tempPath));
                }
                else
                {
                    Debug.Log("[yanplaRoles] Plugin is up to date.");
                }
            }
            else
            {
                Debug.LogError($"[yanplaRoles] Update check failed: {request.error}");
            }
        }

        public static IEnumerator DownloadUpdate(string downloadUrl, string tempFilePath)
        {
            UnityWebRequest request = UnityWebRequest.Get(downloadUrl);
            request.SetRequestHeader("User-Agent", "request");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Save to temp file first
                File.WriteAllBytes(tempFilePath, request.downloadHandler.data);
                Debug.Log("[yanplaRoles] Downloaded new version to temp file.");

                // Now replace the existing plugin file
                ReplaceOldMod(tempFilePath);

                // Notify the user to restart the game
                NotifyRestart();
            }
            else
            {
                Debug.LogError($"[yanplaRoles] Failed to download update: {request.error}");
            }
        }

        private static void ReplaceOldMod(string tempFilePath)
        {
            try
            {
                // Backup the old mod, just in case
                string backupPath = modPath + ".bak";
                if (File.Exists(modPath))
                {
                    File.Move(modPath, backupPath, true);
                }

                // Move the new file to the correct location
                File.Move(tempFilePath, modPath, true);
                Debug.Log("[yanplaRoles] Updated mod file successfully.");
            }
            catch (IOException ex)
            {
                Debug.LogError($"[yanplaRoles] Failed to replace mod file: {ex.Message}");
            }
        }

        private static void NotifyRestart()
        {
            // Display message in-game to notify the user to restart
            Debug.Log("[yanplaRoles] Update complete. Please restart the game to apply the new version.");

            InfoPopup.Show("Update complete. Please restart the game.");
        }

    }
}
