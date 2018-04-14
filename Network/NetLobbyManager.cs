using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetLobbyManager : MonoBehaviour {

	public void StartupHost() {
		((RandomCharacterNetworkManager)NetworkManager.singleton).StartupHost();
	}

	public void JoinGame() {
		((RandomCharacterNetworkManager)NetworkManager.singleton).JoinGame();
	}
}
