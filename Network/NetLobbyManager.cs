using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetLobbyManager : MonoBehaviour {

	public ButtonRef[] MenuOptions;
	public GameObject Wait;
	public float TimeOutTime = 5;
	public int ActiveElement;
	private bool _loadingLevel;
	private string _waitText;
	private float _timeOutTimer;

	private void Update() {
		if (!_loadingLevel) {
			Wait.SetActive(false);
			// 选中
			MenuOptions[ActiveElement].Selected = true;
			_timeOutTimer = TimeOutTime;
			
			// 选择菜单
			if (Input.GetKeyDown(KeyCode.UpArrow)) {
				MenuOptions[ActiveElement].Selected = false;

				ActiveElement = (ActiveElement + MenuOptions.Length - 1) % MenuOptions.Length;
			}

			if (Input.GetKeyDown(KeyCode.DownArrow)) {
				MenuOptions[ActiveElement].Selected = false;

				ActiveElement = (ActiveElement + 1) % MenuOptions.Length;
			}

			if (Input.GetKeyDown(KeyCode.RightArrow)) {
				if (ActiveElement == 1) {
					MenuOptions[1].gameObject.GetComponentInChildren<InputField>().ActivateInputField();
				}
			}

			// P进入游戏
			if (Input.GetButtonDown("P")) {
				switch (ActiveElement) {
					case 0:
						((RandomCharacterNetworkManager)NetworkManager.singleton).StartupHost();
						MenuOptions[ActiveElement].transform.localScale *= 1.2f;
						_loadingLevel = true;
						_waitText = "Creating...";
						break;
					case 1:
						((RandomCharacterNetworkManager)NetworkManager.singleton).JoinGame();
						MenuOptions[ActiveElement].transform.localScale *= 1.2f;
						_loadingLevel = true;
						_waitText = "Linking...";
						break;
					case 2:
						MenuOptions[ActiveElement].transform.localScale *= 1.2f;
						Back();
						break;
				}
			}
		}

		if (_loadingLevel) {
			Wait.SetActive(true);
			Wait.GetComponentInChildren<Text>().text = _waitText;
			_timeOutTimer -= Time.deltaTime;

			if (_timeOutTimer < 0) { // 超时了
				_waitText = "Time out.";
				_timeOutTimer = TimeOutTime;
			}
		}
	}

	public void Back() {
		if (_loadingLevel) {
			_loadingLevel = false;
		} else {
			SceneManager.LoadSceneAsync(SceneName.INTRO, LoadSceneMode.Single);
		}
	}

	public void StartupHost() {
		((RandomCharacterNetworkManager)NetworkManager.singleton).StartupHost();
	}

	public void JoinGame() {
		((RandomCharacterNetworkManager)NetworkManager.singleton).JoinGame();
	}
}
