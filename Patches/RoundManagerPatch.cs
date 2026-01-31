using AwhDangit.Custom;
using GameNetcodeStuff;
using HarmonyLib;
using LethalCasino;
using Unity.Netcode;

namespace AwhDangit.Patches;

[HarmonyPatch(typeof(RoundManager))]
internal class RoundManagerPatch : NetworkBehaviour
{
    [HarmonyPatch("Awake")]
    [HarmonyPostfix]
    public static void AwakePatch(RoundManager __instance)
    {
        AwhDangit.Logger.LogDebug((object) "RoundManagerPatch just woke up!");
    }
    
    [HarmonyPatch("LoadNewLevelWait")]
    [HarmonyPrefix]
    public static void LoadNewLevelWaitPatch(RoundManager __instance)
    {
        AwhDangit.Logger.LogDebug("Welcome back!");
        if (AwhDangit.BoundConfig.ResetEachRound.Value)
            ResetPlayerNetProfits(__instance);
    }

    [HarmonyPatch("DespawnPropsAtEndOfRound")]
    [HarmonyPostfix]
    public static void DespawnPropsAtEndOfRoundPatch(RoundManager __instance)
    {
        AwhDangit.Logger.LogDebug("Come back soon!");
        if (AwhDangit.BoundConfig.ResetEachRound.Value)
            ResetPlayerNetProfits(__instance);
    }

    private static void ResetPlayerNetProfits(RoundManager __instance)
    {
        // If we're not on Gordion (company planet) then we should exit early
        if (__instance.currentLevel.levelID != 3)
            return;
        
        AwhDangit.Logger.LogDebug("Resetting gambling profits for all players");
        // Iterate over each player in the current round
        foreach (PlayerControllerB player in __instance.playersManager.allPlayerScripts)
        {
            // Reset their net profits to 0
            AwhDangit.Logger.LogDebug("Resetting gambling profit for player: " + player.playerUsername);
            player.TryGetComponent<GamblingProfitPlayerController>(out var playerController);
            if (playerController != null)
                playerController.gambleProfit = 0;
        }
    }
}