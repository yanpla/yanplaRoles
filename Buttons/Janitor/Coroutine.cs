using System.Collections;
using MiraAPI.Utilities;
using UnityEngine;
using System;

namespace yanplaRoles.Buttons.Janitor
{
    public class Coroutine
    {
        public IEnumerator CleanBodyCoroutine(Byte target)
        {
            var body = Helpers.GetBodyById(target);
            SpriteRenderer rend = body.bodyRenderers[0];
            Color initialColor = rend.color;
            float duration = 2.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                rend.color = Color.Lerp(initialColor, Color.clear, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            rend.color = Color.clear;
            UnityEngine.Object.Destroy(body.gameObject);
        }
    }
}