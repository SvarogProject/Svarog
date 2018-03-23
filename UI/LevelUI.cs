using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelUI : MonoBehaviour {
    public Text AnnouncerTextLine1;
    public Text AnnouncerTextLine2;
    public Text LevelTimer;

    public Slider[] HealthSliders;

    public GameObject[] WinIndicatorGrids;
    public GameObject WinIndicator;

    #region Singleton
    
    private static LevelUI _instance;

    public static LevelUI GetInstance() {
        return _instance;
    }

    public void Awake() {
        _instance = this;
    }
    #endregion

    public void AddWinIndicator(int player) {
        var go = Instantiate(WinIndicator, transform.position, Quaternion.identity) as GameObject;
        if (go != null) 
            go.transform.SetParent(WinIndicatorGrids[player].transform);
    }
}