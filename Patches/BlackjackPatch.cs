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
        AwhDangit.Logger.LogDebug("UpdateScrapValueClientRpc Postfix for Blackjack");
        UpdateScrapValueFromRef(__instance, item);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Blackjack.JoinGameServerRpc))]
    public static void JoinGamePostfix(Blackjack __instance, NetworkBehaviourReference playerRef, int playerIdx)
    {
        AwhDangit.Logger.LogDebug("JoinGameServerRpc Postfix for Blackjack");
        // If the instance isn't the server's or host's, return early
        if (__instance is { IsServer: false, IsHost: false })
            return;

        // Keep track of all items in the player's inventory
        var player = ((PlayerControllerB)(NetworkBehaviour)playerRef);
        foreach (var scrap in __instance.gambledScrap[playerIdx])
            StoreScrapInfo("Blackjack", player, scrap);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Blackjack.GameFinished))]
    public static void GameFinishedPrefix(Blackjack __instance, PlayerControllerB[] __state)
    {
        AwhDangit.Logger.LogDebug("GameFinished Prefix for Blackjack");
        AwhDangit.Logger.LogDebug(__instance.gamblingPlayers.Length);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Blackjack.GameFinished))]
    public static void ProcessResultsPostfix(Blackjack __instance, PlayerControllerB[] __state)
    {
        AwhDangit.Logger.LogDebug("ProcessResultsPostfix Postfix for Blackjack");
        foreach (var player in __instance.gamblingPlayers)
        {
            if (player == null) continue;
            CheckGamblingProfitsFromRefs(__instance, (NetworkBehaviourReference)player);
        }
    }
}