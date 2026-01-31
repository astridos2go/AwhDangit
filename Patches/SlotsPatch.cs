using System.Collections.Generic;
using AwhDangit.Custom;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using LethalCasino.Custom;

namespace AwhDangit.Patches;

[HarmonyPatch(typeof (SlotMachine))]
internal class SlotsPatch : BasePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(SlotMachine.UpdateScrapValueClientRpc))]
    public static void UpdateScrapValuePostfix(SlotMachine __instance, NetworkBehaviourReference item)
    {
        UpdateScrapValue(__instance, item);
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(SlotMachine.StartGambleServerRpc))]
    public static void StartGamblePostfix(
        SlotMachine __instance,
        NetworkBehaviourReference playerRef,
        NetworkBehaviourReference scrapRef)
    {
        StoreScrapInfoFromRefs(playerRef, scrapRef, "Slots");
    }
}