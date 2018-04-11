using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class RandomCharacterNetworkManager : NetworkManager {
    
    public Transform[] SpawnPositions; // 角色出生点（在游戏布局中设定好）
    private NetLevelUI _levelUi; 
    
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        GameObject go;

        if (spawnPrefabs.Count > 0)
            go = spawnPrefabs[Random.Range(0, spawnPrefabs.Count + 1)]; // 随机角色
        else
            go = playerPrefab;

        var player = Instantiate(go, SpawnPositions[numPlayers].position, Quaternion.identity);
        /*
        _levelUi = NetLevelUI.GetInstance();
        player.GetComponent<NetPlayerStateManager>().HealthSlider = _levelUi.HealthSliders[numPlayers].gameObject; // 绑定血条

        player.gameObject.layer = LayerMask.NameToLayer("Player") + numPlayers; // 角色分层
        foreach (var c in player.GetComponentsInChildren<BoxCollider2D>()) { 
            if (c.CompareTag("MovementCollider")) {
                c.gameObject.layer = LayerMask.NameToLayer("MovementCollider") + numPlayers; // 碰撞体设置不同的层级
            }                
        }*/
        
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        
    }
}