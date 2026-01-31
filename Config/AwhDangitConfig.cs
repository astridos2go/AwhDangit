using System.Collections.Generic;
using System.Reflection;
using BepInEx.Configuration;
using CSync.Extensions;
using CSync.Lib;
using HarmonyLib;

namespace AwhDangit.Config;

public class AwhDangitConfig : SyncedConfig2<AwhDangitConfig>
{
    [SyncedEntryField] public readonly SyncedEntry<int> MaxLossAmount;
    [SyncedEntryField] public readonly SyncedEntry<bool> ResetEachRound;
    [SyncedEntryField] public readonly SyncedEntry<bool> ResetWhenKilled;

    public AwhDangitConfig(ConfigFile cfg) : base(MyPluginInfo.PLUGIN_GUID)
    {
        // Disable auto saving for initial binding
        cfg.SaveOnConfigSet = false;
        MaxLossAmount = cfg.BindSyncedEntry(
            "General",
            "MaxLossAmount",
            -100,
            "How much money a player has to be down at the Casino before facing consequences... (Setting this to a positive number may have unintended effects)");

        ResetEachRound = cfg.BindSyncedEntry(
            "General",
            "ResetEachRound",
            true,
            "Whether or not to forget players' gambling debts each time you leave the Company.");

        ResetWhenKilled = cfg.BindSyncedEntry(
            "General",
            "ResetWhenPunished",
            true,
            "Whether or not to forgive a player's debts when they suffer consequences. (Useful if you have mods that allow reviving).");

        ClearOrphanedEntries(cfg);

        // Manually save, and re-enable auto saving
        cfg.Save();
        cfg.SaveOnConfigSet = true;
        
        ConfigManager.Register(this);
    }

    static void ClearOrphanedEntries(ConfigFile cfg)
    {
        // Find the private property `OrphanedEntries` from the type `ConfigFile` //
        PropertyInfo orphanedEntriesProp = AccessTools.Property(typeof(ConfigFile), "OrphanedEntries");
        // And get the value of that property from our ConfigFile instance //
        var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(cfg);
        // And finally, clear the `OrphanedEntries` dictionary //
        orphanedEntries.Clear();
    }
}