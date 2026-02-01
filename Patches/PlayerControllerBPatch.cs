using AwhDangit.Custom;
using GameNetcodeStuff;
using HarmonyLib;

namespace AwhDangit.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal class PlayerControllerBPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(PlayerControllerB.Awake))]
    public static void Awake(PlayerControllerB __instance)
    {
        // Attach the Gambling Controller to the player when they join
        AwhDangit.Logger.LogDebug($"Attaching gambling profit player controller to {__instance.playerUsername}");
        __instance.gameObject.AddComponent<GamblingProfitPlayerController>();
    }
}