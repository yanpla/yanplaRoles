using System.Collections;
using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP.Utils;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities.Assets;
using UnityEngine;
using yanplaRoles.Modifiers;
using yanplaRoles.Options.Roles;

namespace yanplaRoles.Roles.Neutral;

public class Amnesiac : CrewmateRole, ICustomRole
{
    public string RoleName => "Amnesiac";
    public string RoleDescription => "Remember A Role Of A Deceased Player";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => new Color(0.5f, 0.7f, 1f, 1f);
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public ArrowBehaviour arrow;
    private Coroutine bodyCoroutine;


    public CustomRoleConfiguration Configuration => new CustomRoleConfiguration(this)
    {
        TasksCountForProgress = false,
        CanGetKilled = true,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };

    public bool IsModifierApplicable(BaseModifier modifier){
        return modifier is not ExecutionerTarget;
    }

    public override bool DidWin(GameOverReason gameOverReason) { return false; }

    public void HudUpdate(HudManager hudManager)
    {
        var nearestBody = MiraAPI.Utilities.Extensions.GetNearestDeadBody(PlayerControl.LocalPlayer, 500f);
        if (nearestBody != null){
            if (bodyCoroutine == null) bodyCoroutine = PlayerControl.LocalPlayer.StartCoroutine(AwaitAndAttachArrow(nearestBody));
        }
        else{
            DestroyArrow();
        }
    }

    public void DestroyArrow()
    {
        if (bodyCoroutine != null){
            PlayerControl.LocalPlayer.StopCoroutine(bodyCoroutine);
            bodyCoroutine = null;
        }
        if (arrow != null){
            Object.Destroy(arrow);
            Object.Destroy(arrow.gameObject);
            arrow = null;
        }
    }

    public override void OnDeath(DeathReason reason)
    {
        DestroyArrow();
    }

    private IEnumerator AwaitAndAttachArrow(DeadBody body)
    {
        yield return new WaitForSeconds(OptionGroupSingleton<AmnesiacOptions>.Instance.ArrowAppearDelay);
        LoadableAsset<Sprite> ArrowSprite = Assets.Arrow;

        if (arrow == null)
        {
            var gameObj = new GameObject();
            arrow = gameObj.AddComponent<ArrowBehaviour>();
            gameObj.transform.parent = body.gameObject.transform;
            var renderer = gameObj.AddComponent<SpriteRenderer>();
            renderer.sprite = ArrowSprite.LoadAsset();
            arrow.image = renderer;
            gameObj.layer = 5;
        }
        arrow.target = body.transform.position;
    }
}