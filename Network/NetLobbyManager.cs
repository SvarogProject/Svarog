using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetLobbyManager : MonoBehaviour {

	public ButtonRef[] MenuOptions;
	public int ActiveElement;
	private bool _loadingLevel;

	private void Update() {
		if (!_loadingLevel) {
			// 选中
			MenuOptions[ActiveElement].Selected = true;

			// 选择菜单
			if (Input.GetKeyDown(KeyCode.UpArrow)) {
				MenuOptions[ActiveElement].Selected = false;

				ActiveElement = (ActiveElement + MenuOptions.Length - 1) % MenuOptions.Length;
			}

			if (Input.GetKeyDown(KeyCode.DownArrow)) {
				MenuOptions[ActiveElement].Selected = false;

				ActiveElement = (ActiveElement + 1) % MenuOptions.Length;
			}

			if (ActiveElement == 2) { // 输入框焦点
				MenuOptions[2].gameObject.GetComponent<InputField>().ActivateInputField();
			} else {
				MenuOptions[2].gameObject.GetComponent<InputField>().DeactivateInputField();
			}

			// P进入游戏
			if (Input.GetButtonDown("P")) {
				switch (ActiveElement) {
					case 0:
						((RandomCharacterNetworkManager)NetworkManager.singleton).StartupHost();
						MenuOptions[ActiveElement].transform.localScale *= 1.2f;
						_loadingLevel = true;
						break;
					case 1:
						((RandomCharacterNetworkManager)NetworkManager.singleton).JoinGame();
						MenuOptions[ActiveElement].transform.localScale *= 1.2f;
						_loadingLevel = true;
						break;
				}
			}
		}
	}

	public void StartupHost() {
		((RandomCharacterNetworkManager)NetworkManager.singleton).StartupHost();
	}

	public void JoinGame() {
		((RandomCharacterNetworkManager)NetworkManager.singleton).JoinGame();
	}
}
