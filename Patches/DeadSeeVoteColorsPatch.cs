using HarmonyLib;
using UnityEngine;

namespace yanplaRoles.Patches
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.BloopAVoteIcon))]
    public static class DeadSeeVoteColorsPatch
    {
        public static bool Prefix(MeetingHud __instance, [HarmonyArgument(0)] NetworkedPlayerInfo voterPlayer, [HarmonyArgument(1)] int index, [HarmonyArgument(2)] Transform parent)
        {
            SpriteRenderer spriteRenderer = Object.Instantiate(__instance.PlayerVotePrefab);

            if (GameOptionsManager.Instance.currentNormalGameOptions.AnonymousVotes && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                PlayerMaterial.SetColors(Palette.DisabledGrey, spriteRenderer);
            }
            else
            {
                PlayerMaterial.SetColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);
            }

            spriteRenderer.transform.SetParent(parent);
            spriteRenderer.transform.localScale = Vector3.zero;

            PlayerVoteArea component = parent.GetComponent<PlayerVoteArea>();
            if (component != null)
            {
                spriteRenderer.material.SetInt(PlayerMaterial.MaskLayer, component.MaskLayer);
            }

            __instance.StartCoroutine(Effects.Bloop(index * 0.3f, spriteRenderer.transform, 1f, 0.5f));
            parent.GetComponent<VoteSpreader>().AddVote(spriteRenderer);
            return false;
        }
    }
}
