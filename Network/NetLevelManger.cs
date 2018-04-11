using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Vuforia;

public class NetLevelManger : NetworkBehaviour {
    public Transform[] SpawnPositions; // 角色出生点（在游戏布局中设定好）
    public int MaxRounds = 2;          // 回合数

    private readonly WaitForSeconds _oneSec = new WaitForSeconds(1); // 重复利用等一秒

    private NetCameraMoveManager _cameraManager;
    //private CharacterManager _characterManager;

    public NetPlayerStateManager[] Players = new NetPlayerStateManager[2];
    
    private LevelUI _levelUi;       // 保存UI元素，方便调用
    private int _currentRounds = 1; // 当前回合

    // 倒计时参数
    public bool EnableCountdown;
    public int MaxRoundsTimer = 60;
    private int _currentTimer;
    private float _internalTimer;

    private int _createdPlayerNum;

    private bool _isStarted = false;

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
        //_characterManager = CharacterManager.GetInstance();
        _levelUi = LevelUI.GetInstance();
        _cameraManager = NetCameraMoveManager.GetInstance();

        _levelUi.AnnouncerTextLine1.gameObject.SetActive(false);
        _levelUi.AnnouncerTextLine2.gameObject.SetActive(false);

    }

    public void FixedUpdate() {

        Debug.Log(NetworkManager.singleton.numPlayers);
        
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
        _levelUi.LevelTimer.text = _currentTimer.ToString();

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

        yield return CreatePlayers();
        yield return InitRound();
    }

    private IEnumerator InitRound() {
        Debug.Log("InitRound");
        _levelUi.AnnouncerTextLine1.gameObject.SetActive(false);
        _levelUi.AnnouncerTextLine2.gameObject.SetActive(false);
        _levelUi.LevelTimer.text = MaxRoundsTimer.ToString();

        _currentTimer = MaxRoundsTimer;
        EnableCountdown = false;

        yield return InitPlayers();
        yield return EnableControl();
    }

    
    private IEnumerator CreatePlayers() {
        Debug.Log("CreatePlayers");

        for (var i = 0; i < 2; i++) {

            Players[i].HealthSlider = _levelUi.HealthSliders[i]; // 绑定血条


            if (Players[i] != null) {
                Players[i].gameObject.layer = LayerMask.NameToLayer("Player") + i; // 角色分层
                foreach (var c in Players[i].GetComponentsInChildren<BoxCollider2D>()) { 
                    if (c.CompareTag("MovementCollider")) {
                        c.gameObject.layer = LayerMask.NameToLayer("MovementCollider") + i; // 碰撞体设置不同的层级
                    }                
                }

                _cameraManager.Players.Add(Players[i].gameObject); // 给摄像机控制添加角色
                _cameraManager.Initial();
            }
        }

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
        // Round x FIGHT!
        _levelUi.AnnouncerTextLine1.gameObject.SetActive(true);
        _levelUi.AnnouncerTextLine1.text = "Round " + _currentRounds;
        _levelUi.AnnouncerTextLine1.color = Color.white;

        yield return _oneSec;
        yield return _oneSec;

        _levelUi.AnnouncerTextLine1.color = Color.white;
        _levelUi.AnnouncerTextLine1.text = "FIGHT!";

        // 开启角色控制
        foreach (var player in Players) {
            player.gameObject.GetComponent<NetInputHandler>().enabled = true;
        }

        // 过一秒让提示消失
        yield return _oneSec;

        _levelUi.AnnouncerTextLine1.gameObject.SetActive(false);
        EnableCountdown = true;
    }

    private void DisableControl() {
        Debug.Log("DisableControl");

        foreach (var player in Players) {
            player.ResetPlayer(); // 先重置角色状态

            player.gameObject.GetComponent<NetInputHandler>().enabled = false;
        }
    }

    public void EndRoundFunction(bool timeOut = false) {
        Debug.Log("EndTurnFunction(" + timeOut + ")");

        EnableCountdown = false;
        //_levelUi.LevelTimer.text = "0";

        if (timeOut) {
            _levelUi.AnnouncerTextLine1.gameObject.SetActive(true);
            _levelUi.AnnouncerTextLine1.text = "Time Out!";
            _levelUi.AnnouncerTextLine1.color = Color.white;
        } else {
            _levelUi.AnnouncerTextLine1.gameObject.SetActive(true);
            _levelUi.AnnouncerTextLine1.text = "K.O.";
            _levelUi.AnnouncerTextLine1.color = Color.white;
        }

        DisableControl();
        StartCoroutine(EndTurn());
    }

    private IEnumerator EndTurn() {
        Debug.Log("EndTurn");

        yield return _oneSec;
        yield return _oneSec;
        yield return _oneSec;

        /*
        var vPlayer = FindWinningPlayer();

        if (vPlayer == null) {
            _levelUi.AnnouncerTextLine1.text = "Draw"; // 平局
            _levelUi.AnnouncerTextLine1.color = Color.white;
        } else {
            _levelUi.AnnouncerTextLine1.text = vPlayer.PlayerId + " Wins!";
            _levelUi.AnnouncerTextLine1.color = Color.white;
        }

        yield return _oneSec;
        yield return _oneSec;
        yield return _oneSec;

        // 完美胜利
        if (vPlayer != null) {
            if (Math.Abs(vPlayer.PlayerStates.Health - 100) < 0.01f) {
                _levelUi.AnnouncerTextLine2.gameObject.SetActive(true);
                _levelUi.AnnouncerTextLine2.text = "Flawless Victory!";
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
            GameSceneManager.GetInstance().RequestLevelLoad(SceneType.Main, "GameOver");
        }
        */
    }

    private bool IsMatchOver() {
        Debug.Log("IsMatchOver");
        var retVal = false;

        /*
        foreach (var player in _characterManager.Players) {
            if (player.Score < MaxRounds) continue;

            retVal = true;

            break;
        }*/

        return retVal;
    }

    /*
    private PlayerBase FindWinningPlayer() {
        // 血量相等则平手，返回null
        if (Math.Abs(_characterManager.Players[0].PlayerStates.Health - _characterManager.Players[1].PlayerStates.Health) < 0.01f)
            return null;

        PlayerStateManager targetPlayerState;

        if (_characterManager.Players[0].PlayerStates.Health < _characterManager.Players[1].PlayerStates.Health) {
            _characterManager.Players[1].Score++;
            targetPlayerState = _characterManager.Players[1].PlayerStates;
            _levelUi.AddWinIndicator(1);
        } else {
            _characterManager.Players[0].Score++;
            targetPlayerState = _characterManager.Players[0].PlayerStates;
            _levelUi.AddWinIndicator(0);
        }

        var retVal = _characterManager.GetPlayerByStates(targetPlayerState);

        return retVal;
    }*/
}