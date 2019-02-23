using UnityEngine;
using System.Collections;

public class SmokeEndJumpControl : MonoBehaviour {
    void Update() {
        Destroy(gameObject, 0.4f);
    }
}