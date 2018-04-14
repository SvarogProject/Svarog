using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RandomCharacterNetworkManager : NetworkManager {
    
    /*
    public Transform[] SpawnPositions; // 角色出生点（在游戏布局中设定好）
    public GameObject[] Players;
    private NetLevelUI _levelUi; 
    
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        GameObject go;

        if (Players.Length > 0)
            go = Players[Random.Range(0, Players.Length + 1)]; // 随机角色
        else
            go = playerPrefab;

        var player = Instantiate(go, SpawnPositions[numPlayers].position, Quaternion.identity);
        
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        
    }
    */

    public void StartupHost() {
        bool flag = client == null || client.connection == null || client.connection.connectionId == -1;

        if (!IsClientConnected() && !NetworkServer.active && matchMaker == null) {
            if (flag) {
                SetPort();
                StartHost();
            }
        }
    }

    public void JoinGame() {
        SetIPAddress();
        SetPort();
        StartClient();
    }

    private void SetPort() {
        networkPort = 7777;
    }

    private void SetIPAddress() {
        var ipAddress = GameObject.Find("InputField IP Address").GetComponent<InputField>().text;
        networkAddress = ipAddress;
    }
}