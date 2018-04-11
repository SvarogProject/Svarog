using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePostion : MonoBehaviour {

	[SerializeField] public Vector3 Position;
	
	// 强行给乱跑的network对象重设位置
	private void Update () {
		transform.localPosition = Position;
	}
}
