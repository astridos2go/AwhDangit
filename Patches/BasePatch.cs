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

    internal static PlayerControllerB? UpdateScrapValueFromRef(NetworkBehaviour gameInstance,
        NetworkBehaviourReference scrapRef)
    {
        // If the instance isn't the server's or host's, return early
        if (gameInstance is { IsServer: false, IsHost: false })
            return null;

        // Get the object that was just gambled
        var scrap = (GrabbableObject)(NetworkBehaviour)scrapRef;
        // No object or we don't know the previous scrap value, return early
        if (scrap == null || !previousScrapValues.ContainsKey(scrap))
            return null;

        return UpdateScrapValue(scrap);
    }

    internal static PlayerControllerB? UpdateScrapValue(GrabbableObject scrap, int? newScrapValue = null)
    {
        var scrapValue = newScrapValue ?? scrap.scrapValue;
        // The player wasn't found or the reference is null, return early
        scrapOwners.TryGetValue(scrap, out var player);
        if (player == null) return null;

        // Get the profit controller for the player
        player.TryGetComponent<GamblingProfitPlayerController>(out var playerController);
        if (playerController == null) return null;

        // Update the players net gambling profit
        var difference = scrapValue - previousScrapValues[scrap];
        playerController.gambleProfit += difference;

        AwhDangit.Logger.LogDebug($"{player.playerUsername} made {difference} on {scrap.name}!");
        AwhDangit.Logger.LogDebug($"{player.playerUsername} has a net profit of {playerController.gambleProfit}");

        // Clean up the dictionaries
        previousScrapValues.Remove(scrap);
        scrapOwners.Remove(scrap);

        return player;
    }

    internal static void StoreScrapInfoFromRefs(NetworkBehaviourReference playerRef,
        NetworkBehaviourReference scrapRef, string gameName)
    {
        // Convert network behavior references to useful objects
        var scrap = (GrabbableObject)(NetworkBehaviour)scrapRef;
        var player = (PlayerControllerB)(NetworkBehaviour)playerRef;

        StoreScrapInfo(gameName, player, scrap);
    }

    internal static void StoreScrapInfo(string gameName, PlayerControllerB player, GrabbableObject scrap,
        int? value = null
    )
    {
        // If no scrap value is passed, use the internal scrap value
        var scrapValue = value ?? scrap.scrapValue;

        // Check to make sure we're not doing a redundant update
        var scrapValueExists = previousScrapValues.TryGetValue(scrap, out int previousScrapValue);
        scrapOwners.TryGetValue(scrap, out PlayerControllerB previousPlayer);
        if ((scrapValueExists && previousScrapValue == scrapValue) && (previousPlayer == player)) return;

        // Store information in the dictionaries
        AwhDangit.Logger.LogDebug($"Setting previous scrap value of {scrap.name} to {scrapValue}");
        previousScrapValues[scrap] = scrapValue;
        AwhDangit.Logger.LogDebug($"Setting {player.playerUsername} as the owner of {scrap.name}");
        scrapOwners[scrap] = player;

        // Log a message about their gamble
        // If the provided scrapValue isn't the same as the internal value, then they didn't gamble all of it
        var gambleAmount = (scrapValue != scrap.scrapValue) ? (scrapValue - scrap.scrapValue) : scrapValue;
        AwhDangit.Logger.LogDebug($"{player.playerUsername} just bet {gambleAmount} from {scrap.name} on {gameName}!");
    }

    internal static void CheckGamblingProfitsFromRefs(NetworkBehaviour gameInstance,
        NetworkBehaviourReference playerRef)
    {
        var player = (PlayerControllerB)(NetworkBehaviour)playerRef;
        if (!player.TryGetComponent(out GamblingProfitPlayerController controller)) return;
        controller.CheckGamblingProfits(gameInstance);
    }
}