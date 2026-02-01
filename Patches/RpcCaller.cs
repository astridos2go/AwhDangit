using GameNetcodeStuff;
using StaticNetcodeLib;
using Unity.Netcode;

namespace AwhDangit.Patches;

[StaticNetcode]
public static class RpcCaller
{
    [ClientRpc]
    public static void PlayerMustSufferClientRpc(NetworkBehaviourReference playerReference)
    {
        var player = (PlayerControllerB)(NetworkBehaviour)playerReference;
        AwhDangit.Logger.LogDebug($"PlayerMustSufferClientRpc called for {player.playerUsername}");
        player.StartCoroutine(ExplosionCaller.DelayedExplosion(player));
    }

    [ServerRpc]
    public static void PlayerMustSufferServerRpc(NetworkBehaviourReference playerReference)
    {
        PlayerMustSufferClientRpc(playerReference);
    }
}