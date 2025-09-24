using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MiraAPI.Roles;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using UnityEngine;

namespace yanplaRoles.Roles.Crewmate;

public class Snitch : CrewmateRole, ICustomRole
{
    public string RoleName => "Snitch";
    public string RoleDescription => "Complete your tasks to reveal the Impostor(s).";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => new Color(0.83f, 0.69f, 0.22f, 1f);
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public CustomRoleConfiguration Configuration => new CustomRoleConfiguration(this)
    {
        CanUseVent = false,
    };
    
    public bool impostorsRevealed = false;
    public bool revealed = false;
    public List<ArrowBehaviour> ImpArrows = new List<ArrowBehaviour>();
    public Dictionary<byte, ArrowBehaviour> SnitchArrows = new Dictionary<byte, ArrowBehaviour>();

    public void HudUpdate(HudManager hudManager)
    {
        if (MeetingHud.Instance != null && impostorsRevealed)
        {
            UpdateMeeting(MeetingHud.Instance);
        }
    }

    private void UpdateMeeting(MeetingHud __instance)
    {
        foreach (var player in __instance.playerStates)
        {
            if (Utils.PlayerById(player.TargetPlayerId).Data.Role.IsImpostor)
            {
                player.NameText.color = Palette.ImpostorRed;
            }
        }
    }

    public void DestroyArrow(byte targetPlayerId)
    {
        var arrow = SnitchArrows.FirstOrDefault(x => x.Key == targetPlayerId);
        if (arrow.Value != null)
            Object.Destroy(arrow.Value);
        if (arrow.Value.gameObject != null)
            Object.Destroy(arrow.Value.gameObject);
        SnitchArrows.Remove(arrow.Key);
    }

    public override void OnDeath(DeathReason reason)
    {
        SnitchArrows.Values.DestroyAll();
        SnitchArrows.Clear();
        ImpArrows.DestroyAll();
        ImpArrows.Clear();
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
public class CompleteTask
{
    public static LoadableAsset<Sprite> ArrowSprite => Assets.Arrow;

    public static void Postfix(PlayerControl __instance)
    {
        if (__instance.Data.Role is Snitch snitch){
            if (__instance.Data.IsDead) return;
            var taskinfos = __instance.Data.Tasks.ToArray();

            var tasksLeft = taskinfos.Count(x => !x.Complete);
            var localRole = PlayerControl.LocalPlayer.Data.Role;

            if (tasksLeft == 1)
            {   
                if (localRole is Snitch){
                    Coroutines.Start(Utils.FlashCoroutine(snitch.RoleColor));
                }
                else if (localRole.IsImpostor)
                {
                    Coroutines.Start(Utils.FlashCoroutine(snitch.RoleColor));
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = ArrowSprite.LoadAsset();
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    snitch.ImpArrows.Add(arrow);
                }
                snitch.revealed = true;
            }

            else if (tasksLeft == 0)
            {
                if (localRole is Snitch)
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.green));
                    snitch.impostorsRevealed = true;
                    var impostors = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Data.Role.IsImpostor);
                    foreach (var imp in impostors)
                    {
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = ArrowSprite.LoadAsset();
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        snitch.SnitchArrows.Add(imp.PlayerId, arrow);
                    }
                }
                else if (localRole.IsImpostor)
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.green));
                }
            }
        }
    }
}

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public class UpdateArrows
{
    public static void Postfix(HudManager __instance)
    {
        if (PlayerControl.AllPlayerControls.Count <= 1) return;
        if (PlayerControl.LocalPlayer == null) return;
        if (PlayerControl.LocalPlayer.Data == null) return;

        foreach (var role in PlayerControl.AllPlayerControls)
        {
            if (role.Data.Role is Snitch snitch){
                if (PlayerControl.LocalPlayer.Data.IsDead || role.Data.IsDead)
                {
                    snitch.SnitchArrows.Values.DestroyAll();
                    snitch.SnitchArrows.Clear();
                    snitch.ImpArrows.DestroyAll();
                    snitch.ImpArrows.Clear();
                }

                foreach (var arrow in snitch.ImpArrows) arrow.target = role.transform.position;

                foreach (var arrow in snitch.SnitchArrows)
                {
                    var player = Utils.PlayerById(arrow.Key);
                    if (player == null || player.Data == null || player.Data.IsDead || player.Data.Disconnected)
                    {
                        snitch.DestroyArrow(arrow.Key);
                        continue;
                    }
                    arrow.Value.target = player.transform.position;
                }
            }
        }
    }
}