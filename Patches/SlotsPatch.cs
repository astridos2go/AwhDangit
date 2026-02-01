using HarmonyLib;
using Unity.Netcode;
using LethalCasino.Custom;

namespace AwhDangit.Patches;

[HarmonyPatch(typeof(SlotMachine))]
internal class SlotsPatch : BasePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(SlotMachine.UpdateScrapValueClientRpc))]
    public static void UpdateScrapValuePrefix(Roulette __instance, NetworkBehaviourReference item, int newValue)
    {
        // Because chips get despawned when UpdateScrapValueClientRpc is called, we have to handle them beforehand
        AwhDangit.Logger.LogDebug("UpdateScrapValueClientRpc Prefix for Slots");
        if (newValue != 0) return;

        var scrap = (GrabbableObject)(NetworkBehaviour)item;
        if (!scrap.name.Contains("ChipBag")) return;
        var player = UpdateScrapValue(scrap, newValue);
        CheckGamblingProfitsFromRefs(__instance, (NetworkBehaviourReference)player);
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(SlotMachine.UpdateScrapValueClientRpc))]
    public static void UpdateScrapValuePostfix(SlotMachine __instance, NetworkBehaviourReference item)
    {
        AwhDangit.Logger.LogDebug("UpdateScrapValueClientRpc Postfix for Slots");
        var playerReference = UpdateScrapValueFromRef(__instance, item);
        
        if (playerReference == null) return;
        CheckGamblingProfitsFromRefs(__instance, (NetworkBehaviourReference)playerReference);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(SlotMachine.StartGambleServerRpc))]
    public static void StartGamblePostfix(
        SlotMachine __instance,
        NetworkBehaviourReference playerRef,
        NetworkBehaviourReference scrapRef)
    {
        AwhDangit.Logger.LogDebug("StartGambleServerRpc Postfix for Slots");
        StoreScrapInfoFromRefs(playerRef, scrapRef, "Slots");
    }
}