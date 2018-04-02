using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    // Mobile
    public ETCJoystick Joystick;
    public ETCButton ButtonAttackP;
    public ETCButton ButtonAttackK;
    public ETCButton ButtonAttackS;
    public ETCButton ButtonAttackHS;
    public ETCButton ButtonJump;
    
    
    public Transform[] SpawnPositions; // 角色出生点（在游戏布局中设定好）
    public int MaxRounds = 2;          // 回合数

    private readonly WaitForSeconds _oneSec = new WaitForSeconds(1); // 重复利用等一秒

    private CameraMoveManager _cameraManager;
    private CharacterManager _characterManager;
    private LevelUI _levelUi;       // 保存UI元素，方便调用
    private int _currentRounds = 1; // 当前回合

    // 倒计时参数
    public bool EnableCountdown;
    public int MaxRoundsTimer = 60;
    private int _currentTimer;
    private float _internalTimer;

    #region Singleton

    private static LevelManager _instance;

    public static LevelManager GetInstance() {
        return _instance;
    }

    public void Awake() {
        _instance = this;
    }

    #endregion

    public void Start() {
        _characterManager = CharacterManager.GetInstance();
        _levelUi = LevelUI.GetInstance();
        _cameraManager = CameraMoveManager.GetInstance();

        _levelUi.AnnouncerTextLine1.gameObject.SetActive(false);
        _levelUi.AnnouncerTextLine2.gameObject.SetActive(false);

        StartCoroutine(StartGame());
    }

    public void FixedUpdate() {
        // 控制角色朝向
        var player1IsLeft = _characterManager.Players[0].PlayerStates.transform.position.x <
                            _characterManager.Players[1].PlayerStates.transform.position.x;

        for (var i = 0; i < 2; i++) {
            if (_characterManager.Players[i].PlayerStates.GetComponentInChildren<Animator>()
                .GetBool(AnimatorBool.CAN_LOOK_BACK)) {
                _characterManager.Players[i].PlayerStates.LookRight =
                    i == 0 && player1IsLeft || i == 1 && !player1IsLeft;
                _characterManager.Players[i].PlayerStates.ShouldLookBack = false; // 主动转了就清除通知
            } else {
                if (_characterManager.Players[i].PlayerStates.LookRight !=
                    (i == 0 && player1IsLeft || i == 1 && !player1IsLeft)) {
                    _characterManager.Players[i].PlayerStates.ShouldLookBack = true; // 通知需要回头
                } else {
                    _characterManager.Players[i].PlayerStates.ShouldLookBack = false;
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
        yield return ShowPlayersPoster();
        yield return InitRound();
    }

    private IEnumerator ShowPlayersPoster() {
        yield return _levelUi.ShowPlayersPoster(_characterManager.GetCharacterByPrefab(_characterManager.Players[0].PlayerPrefab), 
            _characterManager.GetCharacterByPrefab(_characterManager.Players[1].PlayerPrefab));    
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

        for (var i = 0; i < _characterManager.Players.Count; i++) {
            var player = Instantiate(
                    _characterManager.Players[i].PlayerPrefab,
                    SpawnPositions[i].position,
                    Quaternion.identity);

            if (player != null)
                _characterManager.Players[i].PlayerStates =
                    player.GetComponent<PlayerStateManager>(); // TODO 这个为什么不在选人的时候就绑定好？

            _characterManager.Players[i].PlayerStates.HealthSlider = _levelUi.HealthSliders[i]; // 绑定血条


            if (player != null) {
                player.layer = LayerMask.NameToLayer("Player") + i; // 角色分层
                foreach (var c in player.GetComponentsInChildren<BoxCollider2D>()) { 
                    if (c.CompareTag("MovementCollider")) {
                        c.gameObject.layer = LayerMask.NameToLayer("MovementCollider") + i; // 碰撞体设置不同的层级
                    }                
                }

                _cameraManager.Players.Add(player.gameObject); // 给摄像机控制添加角色
                _cameraManager.Initial();
            }

        }

        yield return null;
    }

    private IEnumerator InitPlayers() {
        Debug.Log("InitPlayers");

        for (var i = 0; i < _characterManager.Players.Count; i++) {
            _characterManager.Players[i].PlayerStates.Health = 100;
            _characterManager.Players[i].PlayerStates.ResetPlayer();
            _characterManager.Players[i].PlayerStates.AnimationHandler.Animator.Play("Idle");
            _characterManager.Players[i].PlayerStates.transform.position = SpawnPositions[i].position;
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
        foreach (var player in _characterManager.Players) {
            switch (player.Type) {
                // 玩家控制
                case PlayerBase.PlayerType.User:

                    if (MobileManager.IsMobile) {
                        var inputHandler = player.PlayerStates.gameObject.GetComponent<MobilePlayerInputManager>();
                        inputHandler.enabled = true;
                    } else {
                        var inputHandler = player.PlayerStates.gameObject.GetComponent<InputHandler>();
                        inputHandler.PlayerInputId = player.InputId;
                        inputHandler.enabled = true;
                    }

                    break;
                // AI控制
                case PlayerBase.PlayerType.Ai:
                    var ai = player.PlayerStates.gameObject.GetComponent<AIParams>();
                    //ai.enabled = true;

                    // 给AI设置敌人
                    ai.EnemyStates = _characterManager.GetOppositePlayer(player).PlayerStates;

                    var behaviour = player.PlayerStates.gameObject.GetComponent<BehaviorTree>();
                    behaviour.enabled = true;

                    break;
                // 网络控制
                case PlayerBase.PlayerType.Simulation:

                    break;
                case PlayerBase.PlayerType.None:

                    break;
                default:

                    throw new ArgumentOutOfRangeException();
            }
        }

        // 过一秒让提示消失
        yield return _oneSec;

        _levelUi.AnnouncerTextLine1.gameObject.SetActive(false);
        EnableCountdown = true; // TODO 这个倒计时应该是可以设置的，无限时间模式则为false
    }

    private void DisableControl() {
        Debug.Log("DisableControl");

        foreach (var player in _characterManager.Players) {
            player.PlayerStates.ResetPlayer(); // 先重置角色状态

            switch (player.Type) {
                case PlayerBase.PlayerType.User:
                    player.PlayerStates.GetComponent<InputHandler>().enabled = false;

                    if (MobileManager.IsMobile) {
                        player.PlayerStates.GetComponent<MobilePlayerInputManager>().enabled = false;
                    }

                    break;
                case PlayerBase.PlayerType.Ai:
                    player.PlayerStates.GetComponent<BehaviorTree>().enabled = false;

                    break;
                case PlayerBase.PlayerType.Simulation:

                    break;
                case PlayerBase.PlayerType.None:

                    break;
                default:

                    throw new ArgumentOutOfRangeException();
            }
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
            foreach (var player in _characterManager.Players) {
                player.Score = 0;
                player.HasCharacter = false;
            }

            if (_characterManager.IsSolo) {
                if (vPlayer == _characterManager.Players[0])
                    GameSceneManager.GetInstance().LoadNextOnProgression();
                else
                    GameSceneManager.GetInstance().RequestLevelLoad(SceneType.Main, "GameOver");
            } else {
                GameSceneManager.GetInstance().RequestLevelLoad(SceneType.Main, "Select");
            }
        }
    }

    private bool IsMatchOver() {
        Debug.Log("IsMatchOver");
        var retVal = false;

        foreach (var player in _characterManager.Players) {
            if (player.Score < MaxRounds) continue;

            retVal = true;

            break;
        }

        return retVal;
    }

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
    }
}