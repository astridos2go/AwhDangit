using System.Reflection;
using AwhDangit.Config;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Enums;
using UnityEngine;

namespace AwhDangit;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("BMX.LobbyCompatibility", "1.5.0")]
[BepInDependency("mrgrm7.LethalCasino", "1.1.0")]
[BepInDependency("com.sigurd.csync", "5.0.0")]
[BepInDependency("Xilophor.StaticNetcodeLib", "1.2.0")]
[LobbyCompatibility(CompatibilityLevel.Everyone, VersionStrictness.Minor)]
public class AwhDangit : BaseUnityPlugin
{
    public static AwhDangit Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }
    internal static AwhDangitConfig BoundConfig { get; private set; } = null!;

    private void Awake()
    {
        Logger = BepInEx.Logging.Logger.CreateLogSource(MyPluginInfo.PLUGIN_GUID);
        Instance = this;
        
        // Load the configuration
        BoundConfig = new AwhDangitConfig(base.Config);
        Patch();
        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    internal static void Patch()
    {
        Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);
        Logger.LogDebug("Patching...");
        Harmony.PatchAll();
        Logger.LogDebug("Finished patching!");
    }

    internal static void Unpatch()
    {
        Logger.LogDebug("Unpatching...");
        Harmony?.UnpatchSelf();
        Logger.LogDebug("Finished unpatching!");
    }
}