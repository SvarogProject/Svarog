using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IndicatorShower : NetworkBehaviour {

	public GameObject Indicator;

	private void Update() {
		if (Indicator == null) {
			enabled = false;
			return;
		}
		
		Indicator.transform.localRotation = transform.localRotation;

		if (!isServer || NetworkManager.singleton.numPlayers == 2) { // 来齐了开始倒计时
			if (!isLocalPlayer) {
				Destroy(Indicator.gameObject, 0);
			} else {
				Destroy(Indicator.gameObject, 5);
			}
		}

	}
}
