using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetLevelUI : NetworkBehaviour {


    [SyncVar(hook = "OnAnnouncerText1TextChange")] public string AnnouncerTextLine1Text;
    [SyncVar(hook = "OnAnnouncerText2TextChange")] public string AnnouncerTextLine2Text;
    [SyncVar(hook = "OnLevelTimerTextChange")] public string LevelTimerText;

    [SyncVar(hook = "OnAnnouncerText1ActiveChange")] public bool AnnouncerTextLine1Active;
    [SyncVar(hook = "OnAnnouncerText2ActiveChange")] public bool AnnouncerTextLine2Active;
    [SyncVar] public int NowWinPlayer;
    [SyncVar(hook = "OnSpawnWinIndicator")] public GameObject NowSpawnWinIndicator;

    [SyncVar(hook = "OnWaitingCanvasActiveChange")] public bool WaitingCanvasActive;
    [SyncVar(hook = "OnWaitingTextChange")] public string WaitingTextContent;
     
    public Text AnnouncerTextLine1;
    public Text AnnouncerTextLine2;
    public Text LevelTimer;

    public Slider[] HealthSliders;

    public GameObject[] WinIndicatorGrids;
    public GameObject WinIndicator;

    public GameObject WaitingCanvas;
    public Text WaitingText;
    
    // Mobile
    public ETCJoystick Joystick;
    public ETCButton ButtonAttackP;
    public ETCButton ButtonAttackK;
    public ETCButton ButtonAttackS;
    public ETCButton ButtonAttackHS;
    public ETCButton ButtonJump;
    public ETCButton ButtonCrouch;
    public GameObject EasyTouchControlsCanvas;
    
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
        if (MobileManager.IsMobile) {
            EasyTouchControlsCanvas.SetActive(true);    
        } else {
            EasyTouchControlsCanvas.SetActive(false);
        }
    }
    
    private void OnWaitingTextChange(string text) {
        WaitingTextContent = text;
        WaitingText.text = text;
    }

    private void OnWaitingCanvasActiveChange(bool active) {
        WaitingCanvasActive = active;
        WaitingCanvas.SetActive(active);
    }

    private void OnSpawnWinIndicator(GameObject indicator) {
        NowSpawnWinIndicator = indicator;
        if (indicator != null) {
            indicator.transform.SetParent(WinIndicatorGrids[NowWinPlayer].transform);
            indicator.transform.localScale = Vector3.one;
            indicator.transform.localPosition = new Vector3(0, 0, -10); // 不然会给个随机的z轴
        }
    }

    private void OnLevelTimerTextChange(string text) {
        LevelTimerText = text;
        LevelTimer.text = text;
    }

    private void OnAnnouncerText2TextChange(string text) {
        AnnouncerTextLine2Text = text;
        AnnouncerTextLine2.text = text;
    }
    
    private void OnAnnouncerText1TextChange(string text) {
        AnnouncerTextLine1Text = text;
        AnnouncerTextLine1.text = text;
    }

    private void OnAnnouncerText1ActiveChange(bool active) {
        AnnouncerTextLine1Active = active;
        AnnouncerTextLine1.gameObject.SetActive(active);
    }
    
    private void OnAnnouncerText2ActiveChange(bool active) {
        AnnouncerTextLine2Active = active;
        AnnouncerTextLine2.gameObject.SetActive(active);
    }

    [Command]
    public void CmdAddWinIndicator(int player) {
        NowWinPlayer = player;
        var go = Instantiate(WinIndicator, Vector3.zero, Quaternion.identity);

        if (go != null) {
            go.transform.SetParent(WinIndicatorGrids[player].transform);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = new Vector3(0, 0, -10); // 不然会给个随机的z轴
        }
        
        NetworkServer.Spawn(go);
        
        NowSpawnWinIndicator = go;
    }
}