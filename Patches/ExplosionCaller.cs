using System.Collections;
using GameNetcodeStuff;
using UnityEngine;

namespace AwhDangit.Patches;

public class ExplosionCaller
{
    public static IEnumerator DelayedExplosion(PlayerControllerB player)
    {
        yield return new WaitForSeconds(0.7f);
        Landmine.SpawnExplosion(player.transform.position + Vector3.up, true, 2.5f, 2.7f);
        // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
        player.causeOfDeath = CauseOfDeath.Inertia | CauseOfDeath.Gravity;
    }
}