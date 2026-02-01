using GameNetcodeStuff;
using System.Collections;
using UnityEngine;

namespace AwhDangit.Patches;

public class ExplosionCaller
{
    public static IEnumerator DelayedExplosion(PlayerControllerB player)
    {
        AwhDangit.Logger.LogDebug($"Waiting 0.7f until explosion for player {player.playerUsername}");
        yield return new WaitForSeconds(0.7f);
        AwhDangit.Logger.LogDebug($"Spawning explosion on player {player.playerUsername}");
        Landmine.SpawnExplosion(player.transform.position + Vector3.up, true, 2.5f, 2.7f);
        player.causeOfDeath = CauseOfDeath.Inertia | CauseOfDeath.Gravity;
    }
}