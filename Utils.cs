using System.Collections;
using System.Collections.Generic;
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
}