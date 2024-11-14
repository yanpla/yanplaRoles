using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MiraAPI.Roles;
using UnityEngine;

namespace yanplaRoles;
public static class Utils{
    public static PlayerControl PlayerById(byte id)
    {
        foreach (var player in PlayerControl.AllPlayerControls)
            if (player.PlayerId == id)
                return player;

        return null;
    }
    
    public static IEnumerator FlashCoroutine(Color color, float waitfor = 1f, float alpha = 0.3f)
    {
        color.a = alpha;
        if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
        {
            var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
            fullscreen.enabled = true;
            fullscreen.gameObject.active = true;
            fullscreen.color = color;
        }

        yield return new WaitForSeconds(waitfor);

        if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
        {
            var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
            if (fullscreen.color.Equals(color))
            {
                fullscreen.color = new Color(1f, 0f, 0f, 0.37254903f);
                fullscreen.enabled = false;
            }
        }
    }
    
    public static void DestroyAll(this IEnumerable<Component> listie)
    {
        foreach (var item in listie)
        {
            if (item == null) continue;
            Object.Destroy(item);
            if (item.gameObject == null) return;
            Object.Destroy(item.gameObject);
        }
    }

    private static Dictionary<byte, List<RoleBehaviour>> playerRolesHistory = new Dictionary<byte, List<RoleBehaviour>>();

    public static void SavePlayerRole(byte playerId, RoleBehaviour role)
    {
        if (!playerRolesHistory.ContainsKey(playerId))
        {
            playerRolesHistory[playerId] = new List<RoleBehaviour>();
        }
        playerRolesHistory[playerId].Add(role);
    }

    public static List<RoleBehaviour> GetPlayerRolesHistory(byte playerId)
    {
        if (playerRolesHistory.ContainsKey(playerId))
        {
            return playerRolesHistory[playerId];
        }
        return new List<RoleBehaviour>();
    }

    public static RoleBehaviour GetPlayerLastRole(byte playerId)
    {
        if (playerRolesHistory.ContainsKey(playerId)) return playerRolesHistory[playerId].Last();
        return null;
    }

    public static void ClearPlayerRolesHistory() => playerRolesHistory.Clear();

    public static UnityEngine.SpriteRenderer myRend(this PlayerControl p) => p.cosmetics.currentBodySprite.BodySprite;

}

public static class RoleExtensions
{
    private static readonly Dictionary<ICustomRole, bool> roleCanDoTasks = new Dictionary<ICustomRole, bool>();

    public static void SetCanDoTasks(this ICustomRole role, bool value)
    {
        roleCanDoTasks[role] = value;
    }

    public static bool GetCanDoTasks(this ICustomRole role)
    {
        return roleCanDoTasks.TryGetValue(role, out var value) ? value : true;
    }
}