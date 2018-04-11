using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetLevelUI : NetworkBehaviour {
    public Text AnnouncerTextLine1;
    public Text AnnouncerTextLine2;
    public Text LevelTimer;

    public Slider[] HealthSliders;

    public GameObject[] WinIndicatorGrids;
    public GameObject WinIndicator;

    #region Singleton

    private static NetLevelUI _instance;

    public static NetLevelUI GetInstance() {
        return _instance;
    }

    public void Awake() {
        _instance = this;
    }

    #endregion

    private void Start() {

    }

    public void AddWinIndicator(int player) {
        var go = Instantiate(WinIndicator, Vector3.zero, Quaternion.identity);

        if (go != null) {
            go.transform.SetParent(WinIndicatorGrids[player].transform);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = new Vector3(0, 0, -10); // 不然会给个随机的z轴
        }
    }
}