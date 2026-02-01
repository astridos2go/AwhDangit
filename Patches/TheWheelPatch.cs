using HarmonyLib;
using LethalCasino.Custom;
using Unity.Netcode;

namespace AwhDangit.Patches;

[HarmonyPatch(typeof(TheWheel))]
internal class TheWheelPatch : BasePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(TheWheel.UpdateScrapValueClientRpc))]
    public static void UpdateScrapValuePostfix(TheWheel __instance, NetworkBehaviourReference item, int newValue,
        bool disablePoofsOnZero)
    {
        AwhDangit.Logger.LogDebug("UpdateScrapValueClientRpc Postfix for The Wheel");
        // Initial bet
        if (disablePoofsOnZero)
        {
            AwhDangit.Logger.LogDebug("Initial bet placed on The Wheel (initial price taken off scrap)");
            var scrap = (GrabbableObject)(NetworkBehaviour)item;
            var player = scrap.playerHeldBy;
            // The wheel is the only game where you lose money before you start
            StoreScrapInfo("The Wheel", player, scrap, scrap.scrapValue + __instance.minimumItemValue);
        }

        // Finished spin
        else
        {
            AwhDangit.Logger.LogDebug("The Wheel Spin finished (applying any profit to scrap)");
            var playerReference = UpdateScrapValueFromRef(__instance, item);
            if (playerReference == null) return;
            CheckGamblingProfitsFromRefs(__instance, (NetworkBehaviourReference)playerReference);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(TheWheel.UpdateScrapValueClientRpc))]
    public static void UpdateScrapValuePrefix(TheWheel __instance, NetworkBehaviourReference item, int newValue,
        bool disablePoofsOnZero)
    {
        // Initial spin or no risk of despawning chips
        if (disablePoofsOnZero || newValue != 0) return;

        var scrap = (GrabbableObject)(NetworkBehaviour)item;
        // Not dealing with Chips
        if (!scrap.name.Contains("ChipBag")) return;

        AwhDangit.Logger.LogDebug("The Wheel Spin finished (applying any profit to scrap)");
        
        // If the player exists, check their gambling profits
        var player = UpdateScrapValue(scrap, newValue);
        if (player == null) return;
        CheckGamblingProfitsFromRefs(__instance, (NetworkBehaviourReference)player);
    }
}