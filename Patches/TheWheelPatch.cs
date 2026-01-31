using GameNetcodeStuff;
using HarmonyLib;
using LethalCasino.Custom;
using Unity.Netcode;

namespace AwhDangit.Patches;

[HarmonyPatch(typeof(TheWheel))]
internal class TheWheelPatch : BasePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(TheWheel.UpdateScrapValueClientRpc))]
    public static void UpdateScrapValuePostfix(TheWheel __instance, NetworkBehaviourReference item, int newValue, bool disablePoofsOnZero)
    {
        // Initial bet
        if (disablePoofsOnZero)
        {
            var scrap = (GrabbableObject) (NetworkBehaviour) item;
            var player = scrap.playerHeldBy;
            // The wheel is the only game where you lose money before you start
            var originalScrapValue = scrap.scrapValue + __instance.minimumItemValue;
            
            StoreScrapInfo(player, scrap, originalScrapValue, "The Wheel");
        }
        
        // Finished spin
        else
            UpdateScrapValue(__instance, item);
    }
}