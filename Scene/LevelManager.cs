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
    public ETCButton ButtonCrouch;


    public Transform[] SpawnPositions; // 角色出生点（在游戏布局中设定好）
    public int MaxRounds = 2;          // 回合数

    private readonly WaitForSeconds _oneSec = new WaitForSeconds(1); // 重复利用等一秒

    private CameraMoveManager _cameraManager;
    private GameManager _gameManager;
    private LevelUI _levelUi;       // 保存UI元素，方便调用
    private int _currentRounds = 1; // 当前回合

    // 倒计时参数
    public bool EnableCountdown;
    public int MaxRoundsTimer = 60;
    private int _currentTimer;
    private float _internalTimer;

    private bool _paused;

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
        _gameManager = GameManager.GetInstance();
        _levelUi = LevelUI.GetInstance();
        _cameraManager = CameraMoveManager.GetInstance();

        _levelUi.AnnouncerTextLine1.gameObject.SetActive(false);
        _levelUi.AnnouncerTextLine2.gameObject.SetActive(false);

        StartCoroutine(StartGame());
    }

    public void FixedUpdate() {
        // 控制角色朝向
        var player1IsLeft = _gameManager.Players[0].PlayerStates.transform.position.x <
                            _gameManager.Players[1].PlayerStates.transform.position.x;

        for (var i = 0; i < 2; i++) {
            if (_gameManager.Players[i].PlayerStates.GetComponentInChildren<Animator>()
                .GetBool(AnimatorBool.CAN_LOOK_BACK)) {
                _gameManager.Players[i].PlayerStates.LookRight =
                    i == 0 && player1IsLeft || i == 1 && !player1IsLeft;

                _gameManager.Players[i].PlayerStates.ShouldLookBack = false; // 主动转了就清除通知
            } else {
                _gameManager.Players[i].PlayerStates.ShouldLookBack = _gameManager.Players[i].PlayerStates.LookRight !=
                                                                      (i == 0 && player1IsLeft ||
                                                                       i == 1 && !player1IsLeft);
            }
        }

    }

    public void Update() {
        if (EnableCountdown) {
            HandleRoundTimer();
        }

        if (Input.GetKeyUp(KeyCode.Space))
            _paused = !_paused;

        Time.timeScale = _paused ? 0 : 1;
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
        yield return _levelUi.ShowPlayersPoster(_gameManager.GetCharacterByPrefab(_gameManager.Players[0].PlayerPrefab),
            _gameManager.GetCharacterByPrefab(_gameManager.Players[1].PlayerPrefab));
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

        for (var i = 0; i < _gameManager.Players.Count; i++) {
            var player = Instantiate(
                _gameManager.Players[i].PlayerPrefab,
                SpawnPositions[i].position,
                Quaternion.identity);

            if (player != null)
                _gameManager.Players[i].PlayerStates =
                    player.GetComponent<PlayerStateManager>();

            _gameManager.Players[i].PlayerStates.HealthSlider = _levelUi.HealthSliders[i]; // 绑定血条
            _gameManager.Players[i].PlayerStates.HitText = _levelUi.HitTexts[i]; // 绑定攻击提示文字


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

        for (var i = 0; i < _gameManager.Players.Count; i++) {
            _gameManager.Players[i].PlayerStates.Health = 100;
            _gameManager.Players[i].PlayerStates.ResetPlayer();
            _gameManager.Players[i].PlayerStates.AnimationHandler.Animator.Play("Idle");
            _gameManager.Players[i].PlayerStates.transform.position = SpawnPositions[i].position;
        }

        yield return null;
    }

    private IEnumerator EnableControl() {
        Debug.Log("EnableControl");

        // Round x FIGHT!
        yield return _levelUi.RoundXFight(_currentRounds);

        // 开启角色控制
        foreach (var player in _gameManager.Players) {
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
                    ai.EnemyStates = _gameManager.GetOppositePlayer(player).PlayerStates;

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

        EnableCountdown = true; // TODO 这个倒计时应该是可以设置的，无限时间模式则为false
    }

    private void DisableControl() {
        Debug.Log("DisableControl");

        foreach (var player in _gameManager.Players) {
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
            foreach (var player in _gameManager.Players) {
                player.Score = 0;
                player.HasCharacter = false;
            }

            if (_gameManager.IsSolo) {
                if (vPlayer == _gameManager.Players[0])
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

        foreach (var player in _gameManager.Players) {
            if (player.Score < MaxRounds) continue;

            retVal = true;

            break;
        }

        return retVal;
    }

    private PlayerBase FindWinningPlayer() {
        // 血量相等则平手，返回null
        if (Math.Abs(_gameManager.Players[0].PlayerStates.Health - _gameManager.Players[1].PlayerStates.Health) < 0.01f)
            return null;

        PlayerStateManager targetPlayerState;

        if (_gameManager.Players[0].PlayerStates.Health < _gameManager.Players[1].PlayerStates.Health) {
            _gameManager.Players[1].Score++;
            targetPlayerState = _gameManager.Players[1].PlayerStates;
            _levelUi.AddWinIndicator(1);
        } else {
            _gameManager.Players[0].Score++;
            targetPlayerState = _gameManager.Players[0].PlayerStates;
            _levelUi.AddWinIndicator(0);
        }

        var retVal = _gameManager.GetPlayerByStates(targetPlayerState);

        return retVal;
    }
}