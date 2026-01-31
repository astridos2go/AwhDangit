using GameNetcodeStuff;
using StaticNetcodeLib;
using Unity.Netcode;
using UnityEngine;

namespace AwhDangit.Patches;

[StaticNetcode]
public static class RpcCaller
{
    [ClientRpc]
    public static void PlayerMustSufferClientRpc(NetworkBehaviourReference playerReference)
    {
        PlayerControllerB player = (PlayerControllerB)(NetworkBehaviour)playerReference;
        if (player.GetComponentInChildren<AudioSource>() == null)
            player.gameObject.AddComponent<AudioSource>();

        // Blow em up!
        player.StartCoroutine(ExplosionCaller.DelayedExplosion(player));
    }

    [ServerRpc]
    public static void PlayerMustSufferServerRpc(NetworkBehaviourReference playerReference)
    {
        RpcCaller.PlayerMustSufferClientRpc(playerReference);
    }
}