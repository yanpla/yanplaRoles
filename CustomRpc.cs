using System;
using System.Linq;
using AmongUs.GameOptions;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using UnityEngine;
using yanplaRoles.Modifiers.Guesser;
using yanplaRoles.Roles.Impostor;

namespace yanplaRoles.rpc;
public static class CustomRpc
{

    [MethodRpc((uint) CustomRpcCalls.CleanBody)]
    public static void RpcCleanBody(this PlayerControl source, Byte target)
    {
        Debug.Log("Cleaning body.");
        var coroutineInstance = new Buttons.Janitor.Coroutine();
        Coroutines.Start(coroutineInstance.CleanBodyCoroutine(target));
    }

    [MethodRpc((uint) CustomRpcCalls.GuesserKill)]
    public static void RpcGuesserKill(this PlayerControl source, Byte target)
    {
        GuesserKill.MurderPlayer(Utils.PlayerById(target));
        GuesserKill.AssassinKillCount(Utils.PlayerById(target), source);
    }

    [MethodRpc((uint) CustomRpcCalls.AmnesiacRemember)]
    public static void RpcAmnesiacRemember(this PlayerControl source, Byte targetId)
    {
        var role = Utils.GetPlayerLastRole(targetId);
        var roleToSet = role != null ? (RoleTypes)RoleId.Get(role.GetType()) : role.Role;
        Debug.Log($"Remembering {role?.NiceName ?? "No Role"}");
        RoleManager.Instance.SetRole(source, roleToSet);
        Utils.SavePlayerRole(source.Data.PlayerId, role);
        PlayerControl.AllPlayerControls.ForEach((System.Action<PlayerControl>)PlayerNameColor.Set);
    }

    [MethodRpc((uint) CustomRpcCalls.Mine)]
    public static void RpcMine(this PlayerControl source, int ventId, Vector2 position, float zAxis)
    {
        var ventPrefab = UnityEngine.Object.FindObjectOfType<Vent>();
        var vent = UnityEngine.Object.Instantiate(ventPrefab, ventPrefab.transform.parent);

        vent.Id = ventId;
        vent.transform.position = new Vector3(position.x, position.y, zAxis);

        if (source.Data.Role is Miner miner){
            if (miner.Vents.Count > 0)
            {
                var leftVent = miner.Vents[^1];
                vent.Left = leftVent;
                leftVent.Right = vent;
            }
            else{
                vent.Left = null;
            }

            vent.Right = null;
            vent.Center = null;

            var allVents = ShipStatus.Instance.AllVents.ToList();
            allVents.Add(vent);
            ShipStatus.Instance.AllVents = allVents.ToArray();

            miner.Vents.Add(vent);
        }

    }
}
