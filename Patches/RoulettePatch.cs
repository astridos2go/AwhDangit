using HarmonyLib;
using LethalCasino.Custom;
using Unity.Netcode;

namespace AwhDangit.Patches;

[HarmonyPatch(typeof(Roulette))]
internal class RoulettePatch : BasePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Roulette.UpdateScrapValueClientRpc))]
    public static void UpdateScrapValuePrefix(Roulette __instance, NetworkBehaviourReference item, int newValue)
    {
        // Because chips get despawned when UpdateScrapValueClientRpc is called, we have to handle them beforehand
        AwhDangit.Logger.LogDebug("UpdateScrapValueClientRpc Prefix for Roulette");
        if (newValue != 0) return;

        var scrap = (GrabbableObject)(NetworkBehaviour)item;
        if (!scrap.name.Contains("ChipBag")) return;
        UpdateScrapValue(scrap, newValue);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Roulette.UpdateScrapValueClientRpc))]
    public static void UpdateScrapValuePostfix(Roulette __instance, NetworkBehaviourReference item)
    {
        AwhDangit.Logger.LogDebug("UpdateScrapValueClientRpc Postfix for Roulette");
        UpdateScrapValueFromRef(__instance, item);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Roulette.PlaceBetServerRpc))]
    public static void PlaceBetPostfix(Roulette __instance, NetworkBehaviourReference playerRef,
        NetworkBehaviourReference scrapRef)
    {
        AwhDangit.Logger.LogDebug("PlaceBetServerRpc Postfix for Roulette");
        StoreScrapInfoFromRefs(playerRef, scrapRef, "Roulette");
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Roulette.DisplayResultsClientRpc))]
    public static void DisplayResultsPostfix(Roulette __instance, NetworkBehaviourReference playerRef)
    {
        AwhDangit.Logger.LogDebug("DisplayResultsPostfix for Roulette");
        CheckGamblingProfitsFromRefs(__instance, playerRef);
    }
}