using System.Linq;
using GameNetcodeStuff;
using HarmonyLib;
using LethalCasino.Custom;
using Unity.Netcode;

namespace AwhDangit.Patches;

[HarmonyPatch(typeof(Blackjack))]
internal class BlackjackPatch : BasePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Blackjack.UpdateScrapValueClientRpc))]
    public static void UpdateScrapValuePostfix(Blackjack __instance, NetworkBehaviourReference item)
    {
        UpdateScrapValue(__instance, item);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Blackjack.JoinGameServerRpc))]
    public static void JoinGamePostfix(Blackjack __instance, NetworkBehaviourReference playerRef, int playerIdx)
    {
        // If the instance isn't the server's or host's, return early
        if (__instance is { IsServer: false, IsHost: false })
            return;

        PlayerControllerB player = ((PlayerControllerB)(NetworkBehaviour)playerRef);
        // Keep track of all items in the player's inventory
        foreach (GrabbableObject scrap in __instance.gambledScrap[playerIdx])
        {
            previousScrapValues[scrap] = scrap.scrapValue;
            scrapOwners[scrap] = player;
        }
    }
}