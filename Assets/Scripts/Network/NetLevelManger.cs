using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class NetLevelManger : NetworkBehaviour {
    public int MaxRounds = 2;          // 回合数

    private readonly WaitForSeconds _oneSec = new WaitForSeconds(1); // 重复利用等一秒

    private NetCameraMoveManager _cameraManager;

    public NetPlayerStateManager[] Players = new NetPlayerStateManager[2];
    
    public NetLevelUI LevelUi;    // 保存UI元素，方便调用
    private int _currentRounds = 1; // 当前回合

    // 倒计时参数
    public bool EnableCountdown;
    public int MaxRoundsTimer = 60;
    private int _currentTimer;
    private float _internalTimer;

    private int _createdPlayerNum;

    private bool _isStarted;

    #region Singleton

    private static NetLevelManger _instance;

    public static NetLevelManger GetInstance() {
        return _instance;
    }

    public void Awake() {
        _instance = this;
    }

    #endregion

    public void Start() {
        LevelUi = NetLevelUI.GetInstance();
        _cameraManager = NetCameraMoveManager.GetInstance();
        LevelUi.AnnouncerTextLine1Active = false;
        LevelUi.AnnouncerTextLine2Active = false;

    }

    public void FixedUpdate() {

        Debug.Log(NetworkManager.singleton.numPlayers);

        if (NetworkManager.singleton.numPlayers == 1) {
            LevelUi.WaitingCanvasActive = true;
            LevelUi.WaitingTextContent = "Waiting for other players to join...\n\nyour ip is " + Network.player.ipAddress;
        }
        
        if (NetworkManager.singleton.numPlayers < 2 && _createdPlayerNum == 2) {
            // 有人掉线了
            NetworkManager.singleton.StopHost();
        }
        
        if (NetworkManager.singleton.numPlayers > _createdPlayerNum && _createdPlayerNum < 2) {
            if (_createdPlayerNum == 0) {
                Players[0] = GameObject.FindGameObjectWithTag("Player").GetComponent<NetPlayerStateManager>();
            } else {
                Players[1] = GameObject.FindGameObjectsWithTag("Player").Last().GetComponent<NetPlayerStateManager>();
            }
            _createdPlayerNum++;
        }

        if (NetworkManager.singleton.numPlayers == 2) { // 人来齐了
            if (!_isStarted) {
                StartCoroutine(StartGame());
                _isStarted = true;
            }
            // 控制角色朝向
            var player1IsLeft = Players[0].transform.position.x < Players[1].transform.position.x;

            for (var i = 0; i < 2; i++) {
                
                if (Players[i].AnimationHandler.Animator.GetBool(AnimatorBool.CAN_LOOK_BACK)) {
                    Players[i].CmdLookRight(i == 0 && player1IsLeft || i == 1 && !player1IsLeft);
                    Players[i].CmdShouldLookBack(false); // 主动转了就清除通知
                } else {
                    if (Players[i].LookRight != (i == 0 && player1IsLeft || i == 1 && !player1IsLeft)) {
                        Players[i].CmdShouldLookBack(true); // 通知需要回头
                    } else {
                        Players[i].CmdShouldLookBack(false);
                    }
                }
            }
        }
    }

    public void Update() {
        if (EnableCountdown) {
            HandleRoundTimer();
        }
    }

    private void HandleRoundTimer() {
        LevelUi.LevelTimerText = _currentTimer.ToString();

        _internalTimer += Time.deltaTime;

        if (_internalTimer > 1) {
            _currentTimer--;
            _internalTimer = 0;
        }

        if (_currentTimer <= 0) {
            EndRoundFunction(true); // 超时结束回合
            EnableCountdown = false;
        }
    }

    private IEnumerator StartGame() {
        Debug.Log("StartGame");

        LevelUi.WaitingTextContent = "Ready to fight.\n\nLoading...";
        yield return _oneSec;
        yield return _oneSec;
        yield return _oneSec;
        
        yield return CreatePlayers();

        LevelUi.WaitingCanvasActive = false;
        yield return InitRound();
    }

    private IEnumerator InitRound() {
        Debug.Log("InitRound");

        LevelUi.AnnouncerTextLine1Active = false;
        LevelUi.AnnouncerTextLine2Active = false;
        LevelUi.LevelTimerText = MaxRoundsTimer.ToString();

        _currentTimer = MaxRoundsTimer;
        EnableCountdown = false;

        yield return InitPlayers();
        yield return EnableControl();
    }

    
    private IEnumerator CreatePlayers() {
        Debug.Log("CreatePlayers");
    
        for (var i = 0; i < 2; i++) {

            Players[i].CmdHealthSlider(LevelUi.HealthSliders[i].gameObject); // 绑定血条


            if (Players[i] != null) {
                Players[i].gameObject.layer = LayerMask.NameToLayer("Player") + i; // 角色分层
                foreach (var c in Players[i].GetComponentsInChildren<BoxCollider2D>()) { 
                    if (c.CompareTag("MovementCollider")) {
                        c.gameObject.layer = LayerMask.NameToLayer("MovementCollider") + i; // 碰撞体设置不同的层级
                    }                
                }
            }
        }

        _cameraManager.Initial(); // 这里只有服务端执行，客户端没有初始化camera

        yield return null;
    }

    private IEnumerator InitPlayers() {
        Debug.Log("InitPlayers");

        for (var i = 0; i < 2; i++) {
            Players[i].Health = 100;
            Players[i].ResetPlayer();
            Players[i].AnimationHandler.Animator.Play("Idle");
        }

        yield return null;
    }

    private IEnumerator EnableControl() {
        Debug.Log("EnableControl");

        LevelUi.AnnouncerTextLine1.color = Color.white;

        LevelUi.AnnouncerTextLine1Active = true;
        LevelUi.AnnouncerTextLine1Text = "Round " + _currentRounds;

        yield return _oneSec;
        yield return _oneSec;

        LevelUi.AnnouncerTextLine1.color = Color.white;
        LevelUi.AnnouncerTextLine1Text = "FIGHT!";

        // 开启角色控制
        foreach (var player in Players) {
            if (MobileManager.IsMobile) {
                player.gameObject.GetComponent<NetMobilePlayerInputManager>().enabled = true;
            } else {
                player.gameObject.GetComponent<NetInputHandler>().enabled = true;
            }
        }

        // 过一秒让提示消失
        yield return _oneSec;

        LevelUi.AnnouncerTextLine1Active = false;
        EnableCountdown = true;
    }

    private void DisableControl() {
        Debug.Log("DisableControl");

        foreach (var player in Players) {
            player.ResetPlayer(); // 先重置角色状态

            if (MobileManager.IsMobile) {
                player.gameObject.GetComponent<NetMobilePlayerInputManager>().enabled = false;
            } else {
                player.gameObject.GetComponent<NetInputHandler>().enabled = false;
            }
        }
    }

    public void EndRoundFunction(bool timeOut = false) {
        Debug.Log("EndTurnFunction(" + timeOut + ")");

        EnableCountdown = false;

        if (timeOut) {
            LevelUi.AnnouncerTextLine1.color = Color.white;
            
            LevelUi.AnnouncerTextLine1Active = true;
            LevelUi.AnnouncerTextLine1Text = "Time Out!";
        } else {
            LevelUi.AnnouncerTextLine1.color = Color.white;
            
            LevelUi.AnnouncerTextLine1Active = true;
            LevelUi.AnnouncerTextLine1Text = "K.O.";
        }

        DisableControl();
        StartCoroutine(EndTurn());
    }

    private IEnumerator EndTurn() {
        Debug.Log("EndTurn");

        yield return _oneSec;
        yield return _oneSec;
        yield return _oneSec;

        var vPlayer = FindWinningPlayer();

        if (vPlayer == null) {
            LevelUi.AnnouncerTextLine1.color = Color.white;
            
            LevelUi.AnnouncerTextLine1Text = "Draw"; // 平局
        } else {
            LevelUi.AnnouncerTextLine1.color = Color.white;
            
            LevelUi.AnnouncerTextLine1Text = vPlayer.gameObject.name + " Wins!";
        }

        yield return _oneSec;
        yield return _oneSec;
        yield return _oneSec;

        // 完美胜利
        if (vPlayer != null) {
            if (Math.Abs(vPlayer.Health - 100) < 0.01f) {

                LevelUi.AnnouncerTextLine2Active = true;
                LevelUi.AnnouncerTextLine2Text = "Flawless Victory!";
            }
        }

        yield return _oneSec;
        yield return _oneSec;
        yield return _oneSec;

        _currentRounds++;

        var matchOver = IsMatchOver();

        if (!matchOver) {
            StartCoroutine(InitRound());
        } else {
            //GameSceneManager.GetInstance().RequestLevelLoad(SceneType.Main, "GameOver");
            // 退出用户
            NetworkManager.singleton.StopHost();
        }
        
    }

    private bool IsMatchOver() {
        Debug.Log("IsMatchOver");
        var retVal = false;
        
        foreach (var player in Players) {
            if (player.Score < MaxRounds) continue;

            retVal = true;

            break;
        }

        return retVal;
    }

    
    private NetPlayerStateManager FindWinningPlayer() {
        // 血量相等则平手，返回null
        if (Math.Abs(Players[0].Health - Players[1].Health) < 0.01f)
            return null;

        NetPlayerStateManager targetPlayerState;

        if (Players[0].Health < Players[1].Health) {
            Players[1].Score++;
            targetPlayerState = Players[1];
            LevelUi.CmdAddWinIndicator(1);
        } else {
            Players[0].Score++;
            targetPlayerState = Players[0];
            LevelUi.CmdAddWinIndicator(0);
        }

        var retVal = targetPlayerState;

        return retVal;
    }
}