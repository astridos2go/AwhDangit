using GameNetcodeStuff;
using HarmonyLib;
using LethalCasino.Custom;
using Unity.Netcode;

namespace AwhDangit.Patches;

[HarmonyPatch(typeof(Roulette))]
internal class RoulettePatch : BasePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Roulette.UpdateScrapValueClientRpc))]
    public static void UpdateScrapValuePostfix(Roulette __instance, NetworkBehaviourReference item)
    {
        UpdateScrapValue(__instance, item);
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Roulette.PlaceBetServerRpc))]
    public static void PlaceBetPostfix(Roulette __instance, NetworkBehaviourReference playerRef,
        NetworkBehaviourReference scrapRef)
    {
        StoreScrapInfoFromRefs(playerRef, scrapRef, "Roulette");
    }
}