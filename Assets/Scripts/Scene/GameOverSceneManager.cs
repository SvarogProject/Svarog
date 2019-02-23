using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverSceneManager : MonoBehaviour {

	public GameObject StartText;
	private float _timer;
	
	private void Update () {
		_timer += Time.deltaTime;

		if (_timer > 0.6f) {
			_timer = 0;
			StartText.SetActive(!StartText.activeInHierarchy);
		}

		if (Input.anyKeyDown) {
			GameSceneManager.GetInstance().RequestLevelLoad(SceneType.Main, SceneName.INTRO);
		}
	}
}
