using UnityEngine;
using System.Collections;

[System.Serializable]
public class ButtonRef : MonoBehaviour {

    public GameObject SelectIndicator;

    public bool Selected;

    void Start()
    {
        SelectIndicator.SetActive(false);
    }

    void Update()
    {
        SelectIndicator.SetActive(Selected);
    }
}