using System;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using UnityEngine;
using yanplaRoles.Modifiers.Guesser;

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
}
