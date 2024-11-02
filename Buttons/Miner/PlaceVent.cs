using MiraAPI.Hud;
using MiraAPI.Utilities.Assets;
using UnityEngine;
using yanplaRoles.Options.Roles;
using MiraAPI.GameOptions;
using System.Linq;
using yanplaRoles.rpc;

namespace yanplaRoles.Buttons.Sheriff;

[RegisterButton]
public class PlaceVent : CustomActionButton
{
    public override string Name => "Place Vent";
    public override float Cooldown => OptionGroupSingleton<MinerOptions>.Instance.MineCooldown;
    public override float EffectDuration => 0f;
    public override int MaxUses => (int)OptionGroupSingleton<MinerOptions>.Instance.MaxVents;
    public override LoadableAsset<Sprite> Sprite => Assets.Mine;

    public bool CanPlace { get; set; }
    public Vector2 VentSize { get; set; } = Vector2.zero;

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is Roles.Impostor.Miner;
    }

    protected override void FixedUpdate(PlayerControl player)
    {
        if (VentSize == Vector2.zero)
        {
            var vents = UnityEngine.Object.FindObjectsOfType<Vent>();
            VentSize = Vector2.Scale(vents[0].GetComponent<BoxCollider2D>().size, vents[0].transform.localScale) * 0.75f;
        }

        var hits = Physics2D.OverlapBoxAll(player.transform.position, VentSize, 0);
        hits = hits.ToArray().Where(c =>
                (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5)
            .ToArray();
        if (hits.Count == 0 && player.moveable == true)
        {
            CanPlace = true;
        }
        else
        {
            CanPlace = false;
        }
    }

    public override bool CanUse()
    {
        if (CanPlace) { return base.CanUse(); }
        return false;
    }

    protected override void OnClick()
    {
        var position = PlayerControl.LocalPlayer.transform.position;
        var id = GetAvailableId();
        PlayerControl.LocalPlayer.RpcMine(id, position, position.z + 0.001f);
    }

    private static int GetAvailableId()
    {
        var id = 0;

        while (true)
        {
            if (ShipStatus.Instance.AllVents.All(v => v.Id != id)) return id;
            id++;
        }
    }
}