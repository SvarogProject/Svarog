using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class RandomCharacterNetworkManager : NetworkManager {
    
    public Transform[] SpawnPositions; // 角色出生点（在游戏布局中设定好）
    
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        GameObject go;

        if (spawnPrefabs.Count > 0)
            go = spawnPrefabs[Random.Range(0, spawnPrefabs.Count + 1)]; // 随机角色
        else
            go = playerPrefab;

        var player = Instantiate(go, SpawnPositions[numPlayers].position, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }
}