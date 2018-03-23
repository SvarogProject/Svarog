using UnityEngine;
using System.Collections;

[System.Serializable]
public class ButtonRef : MonoBehaviour {
    public GameObject SelectIndicator;

    public bool Selected;

    public void Start() {
        SelectIndicator.SetActive(false);
    }

    public void Update() {
        SelectIndicator.SetActive(Selected);
    }
}