using UnityEngine.Networking;

// 未使用
public class PlayerNetworkSetup : NetworkBehaviour {
    private void Start() {
        if (!isLocalPlayer) { // 如果不是本地角色，把控制器关掉
            GetComponent<InputHandler>().enabled = false;
            GetComponent<MovementHandler>().enabled = false;
            GetComponent<PlayerAnimationHandler>().enabled = false;
            GetComponent<PlayerStateManager>().enabled = false;
        }
    }
}