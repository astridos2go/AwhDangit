using AwhDangit.Patches;
using GameNetcodeStuff;
using Unity.Netcode;

namespace AwhDangit.Custom;

internal class GamblingProfitPlayerController : NetworkBehaviour
{
    public int gambleProfit = 0;
    private PlayerControllerB player = null!;

    private void Start() => this.player = this.GetComponent<PlayerControllerB>();

    public void CheckGamblingProfits(NetworkBehaviour gameInstance)
    {
        AwhDangit.Logger.LogDebug($"Checking gambling profits for player {player.playerUsername}");
        // Player is still good to gamble!
        if (gambleProfit > AwhDangit.BoundConfig.MaxLossAmount.Value)
            return;

        // Explode them!
        TryShowWarning(gameInstance, "Uh oh!", "Time to pay for your debts...", true);
        AwhDangit.Logger.LogInfo($"{player.playerUsername} will be exploded for their debts");
        RpcCaller.PlayerMustSufferServerRpc((NetworkBehaviourReference)(NetworkBehaviour)player);

        // Reset profit if enabled in the config
        if (!AwhDangit.BoundConfig.ResetWhenKilled.Value) return;
        AwhDangit.Logger.LogDebug($"{player.playerUsername}'s debts will been forgiven in death");
        gambleProfit = 0;
    }

    private void TryShowWarning(NetworkBehaviour instance, string title, string message, bool isWarning = false)
    {
        var methodInfo = instance.GetType().GetMethod("ShowWarningMessageClientRpc");
        if (methodInfo == null) return;
        
        AwhDangit.Logger.LogDebug($"Showing warning message to {player.playerUsername}");
        object[] parameters = [(NetworkBehaviourReference)player, title, message, isWarning];
        methodInfo.Invoke(instance, parameters);
    }
}