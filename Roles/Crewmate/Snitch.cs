using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MiraAPI.Roles;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using UnityEngine;
using yanplaRoles.rpc;

namespace yanplaRoles.Roles.Crewmate;

[RegisterCustomRole]
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
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
public class CompleteTask
{
    public static LoadableAsset<Sprite> ArrowSprite => Assets.Arrow;

    public static void Postfix(PlayerControl __instance)
    {
        if (!(__instance.Data.Role is Snitch)) return;
        if (__instance.Data.IsDead) return;
        var taskinfos = __instance.Data.Tasks.ToArray();

        var tasksLeft = taskinfos.Count(x => !x.Complete);
        var role = CustomRoleSingleton<Snitch>.Instance;
        var localRole = PlayerControl.LocalPlayer.Data.Role;

        if (tasksLeft == 1)
        {   
            if (localRole is Snitch){
                Coroutines.Start(Utils.FlashCoroutine(role.RoleColor));
            }
            else if (localRole.TeamType == RoleTeamTypes.Impostor)
            {
                Coroutines.Start(Utils.FlashCoroutine(role.RoleColor));
                var gameObj = new GameObject();
                var arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = ArrowSprite.LoadAsset();
                arrow.image = renderer;
                gameObj.layer = 5;
                ((Snitch)role).ImpArrows.Add(arrow);
            }
        }

        else if (tasksLeft == 0)
        {
            if (localRole is Snitch)
            {
                Coroutines.Start(Utils.FlashCoroutine(Color.green));
                ((Snitch)localRole).impostorsRevealed = true;
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
                    ((Snitch)role).SnitchArrows.Add(imp.PlayerId, arrow);
                }
            }
            else if (localRole.IsImpostor)
            {
                Coroutines.Start(Utils.FlashCoroutine(Color.green));
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

        Snitch snitch = (Snitch)CustomRoleSingleton<Snitch>.Instance;
        if (PlayerControl.LocalPlayer.Data.IsDead)
        {
            snitch.SnitchArrows.Values.DestroyAll();
            snitch.SnitchArrows.Clear();
            snitch.ImpArrows.DestroyAll();
            snitch.ImpArrows.Clear();
        }

        PlayerControl snitchPlayer = null;

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.Data.Role is Snitch){
                snitchPlayer = player;
            }
        }

        if (snitchPlayer != null)
        {
            foreach (var arrow in snitch.ImpArrows) arrow.target = snitchPlayer.transform.position;
            if (snitchPlayer.Data.IsDead)
            {
                snitch.ImpArrows.DestroyAll();
                snitch.ImpArrows.Clear();
            }
        }

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