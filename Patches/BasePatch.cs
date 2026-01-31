using System;
using System.Collections.Generic;
using AwhDangit.Custom;
using GameNetcodeStuff;
using Unity.Netcode;

namespace AwhDangit.Patches;

internal class BasePatch
{
    internal static Dictionary<GrabbableObject, int> previousScrapValues = new Dictionary<GrabbableObject, int>();
    internal static Dictionary<GrabbableObject, PlayerControllerB> scrapOwners =
        new Dictionary<GrabbableObject, PlayerControllerB>();

    internal static void UpdateScrapValue(NetworkBehaviour __instance, NetworkBehaviourReference item)
    {
        // If the instance isn't the server's or host's, return early
        if (__instance is { IsServer: false, IsHost: false })
            return;

        // Get the object that was just gambled
        var scrap = (GrabbableObject)(NetworkBehaviour)item;
        // No object or we don't know the previous scrap value, return early
        if (scrap == null || !previousScrapValues.ContainsKey(scrap))
            return;

        // The player wasn't found or the reference is null, return early
        scrapOwners.TryGetValue(scrap, out var player);
        if (player == null)
            return;

        // Get the profit controller for the player
        player.TryGetComponent<GamblingProfitPlayerController>(out var playerController);
        if (playerController == null)
            return;

        // Update the players net gambling profit
        var difference = scrap.scrapValue - previousScrapValues[scrap];
        playerController.gambleProfit += difference;
        
        AwhDangit.Logger.LogDebug($"{player.playerUsername} made {difference} on {scrap.name}!");
        AwhDangit.Logger.LogDebug($"{player.playerUsername} has a net profit of {playerController.gambleProfit}");
        
        if (playerController.gambleProfit <= AwhDangit.BoundConfig.MaxLossAmount.Value)
        {
            // Explode them!
            TryShowWarningToClient(__instance, player, "Uh oh!", "Time to pay for your debts...", true);
            AwhDangit.Logger.LogInfo($"{player.playerUsername} will be exploded for their debts");
            RpcCaller.PlayerMustSufferServerRpc((NetworkBehaviourReference)(NetworkBehaviour)player);
            
            // Reset profit if enabled in the config
            if (AwhDangit.BoundConfig.ResetWhenKilled.Value)
            {
                AwhDangit.Logger.LogDebug($"{player.playerUsername}'s debts have been forgiven in death");
                playerController.gambleProfit = 0;
            }
        }
        // Clean up the dictionaries
        previousScrapValues.Remove(scrap);
        scrapOwners.Remove(scrap);
    }

    internal static void StoreScrapInfoFromRefs(NetworkBehaviourReference playerRef,
        NetworkBehaviourReference scrapRef, string gameName)
    {
        // Convert network behavior references to useful objects
        var scrap = (GrabbableObject)(NetworkBehaviour)scrapRef;
        var player = (PlayerControllerB)(NetworkBehaviour)playerRef;
        
        StoreScrapInfo(player, scrap, scrap.scrapValue, gameName);
    }

    internal static void StoreScrapInfo(PlayerControllerB player, GrabbableObject scrap, int scrapValue, string gameName)
    {
        // Check to make sure we're not doing a redundant update
        var scrapValueExists = previousScrapValues.TryGetValue(scrap, out int previousScrapValue);
        scrapOwners.TryGetValue(scrap, out PlayerControllerB previousPlayer);
        if ((scrapValueExists && previousScrapValue == scrapValue) && (previousPlayer == player))
            return;
        
        // Store information in the dictionaries
        AwhDangit.Logger.LogDebug($"Setting previous scrap value of {scrap.name} to {scrapValue}");
        previousScrapValues[scrap] = scrapValue;
        AwhDangit.Logger.LogDebug($"Setting {player.playerUsername} as the owner of {scrap.name}");
        scrapOwners[scrap] = player;

        // Log a message about their gamble
        // If the provided scrapValue isn't the same as the internal value, then they didn't gamble all of it
        var gambleAmount = (scrapValue != scrap.scrapValue) ? (scrapValue - scrap.scrapValue) : scrapValue;
        AwhDangit.Logger.LogDebug(
            $"{player.playerUsername} just bet {gambleAmount} from {scrap.name} on {gameName}!");
    }

    private static void TryShowWarningToClient(NetworkBehaviour instance, PlayerControllerB player, string title, string message, bool isWarning = false)
    {
        var methodInfo = instance.GetType().GetMethod("ShowWarningMessageClientRpc");
        if (methodInfo != null)
        {
            object[] parameters = [(NetworkBehaviourReference)player, title, message, isWarning];
            methodInfo.Invoke(instance, parameters);
        }
    }
}