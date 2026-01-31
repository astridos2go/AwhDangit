using GameNetcodeStuff;
using Unity.Netcode;

namespace AwhDangit.Custom;

internal class GamblingProfitPlayerController : NetworkBehaviour
{
    public int gambleProfit = 0;
    private PlayerControllerB playerController = null!;
    
    private void Start() => this.playerController = this.GetComponent<PlayerControllerB>();
}