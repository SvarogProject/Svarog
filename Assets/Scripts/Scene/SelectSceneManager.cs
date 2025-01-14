﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class SelectSceneManager : MonoBehaviour {
    public int NumberOfPlayers = 1;
    public List<PlayerChoose> PlayerChooseList = new List<PlayerChoose>();
    public GameObject PotraitCanvas; // 放头像的画布
    public bool BothPlayersSelected;
    public GameObject PotraitPrefab;
    public GameObject LoadingCanvas;

    private int _maxRow;
    private int _maxCollumn;
    private readonly List<PotraitInfo> _potraitList = new List<PotraitInfo>();
    private bool _isLoadLevel;
    private GameManager _charManager;

    #region Singleton

    private static SelectSceneManager _instance;

    public static SelectSceneManager GetInstance() {
        return _instance;
    }

    public void Awake() {
        _instance = this;
    }

    #endregion

    public void Start() {
        _charManager = GameManager.GetInstance();
        NumberOfPlayers = _charManager.NumberOfUsers;
        CreatePotraits();
        _charManager.IsSolo = NumberOfPlayers == 1;
    }

    private void Update() {
        // Mobile
        if (MobileManager.IsMobile) {
            // 如果是移动端就先不选择
            foreach (var playerChoose in PlayerChooseList) {
                playerChoose.ActiveX = -1;
                playerChoose.ActiveY = -1;
            }
        }

        if (!_isLoadLevel) {
            for (var i = 0; i < PlayerChooseList.Count; i++) {
                if (i < NumberOfPlayers) {
                    // 取消选择
                    if (Input.GetButtonUp("K" + _charManager.Players[i].InputId)) {
                        PlayerChooseList[i].PlayerBase.HasCharacter = false;
                    }

                    if (!_charManager.Players[i].HasCharacter) {
                        PlayerChooseList[i].PlayerBase = _charManager.Players[i];

                        HandleSelectScreenInput(PlayerChooseList[i], _charManager.Players[i].InputId);
                        HandleSelectorPosition(PlayerChooseList[i]);
                        HandleCharacterPreview(PlayerChooseList[i]);
                    }
                } else {
                    // 电脑默认已经选择了角色
                    _charManager.Players[i].HasCharacter = true;
                }
            }

        }

        // 双方都选择了之后进入游戏
        if (BothPlayersSelected) {
            StartCoroutine(LoadLevel());
            _isLoadLevel = true;
            BothPlayersSelected = false;
        } else {
            if (_charManager.Players[0].HasCharacter && _charManager.Players[1].HasCharacter) {
                BothPlayersSelected = true;
            }

        }
    }

    private void CreatePotraits() {
        var gridLayoutGroup = PotraitCanvas.GetComponent<GridLayoutGroup>();

        _maxRow = gridLayoutGroup.constraintCount;
        var x = 0;
        var y = 0;

        foreach (var character in _charManager.CharacterList) {

            var potraitPrefab = Instantiate(PotraitPrefab);
            potraitPrefab.transform.SetParent(PotraitCanvas.transform);
            potraitPrefab.transform.localScale = Vector3.one;

            var potraitInfo = potraitPrefab.GetComponent<PotraitInfo>();

            potraitInfo.Img.sprite = character.Icon;
            potraitInfo.CharacterId = character.CharacterId;
            potraitInfo.PositionX = x;
            potraitInfo.PositionY = y;
            _potraitList.Add(potraitInfo);

            // Mobile
            if (MobileManager.IsMobile) {
                potraitInfo.gameObject.GetComponent<Button>().onClick.AddListener(delegate { OnClick(potraitInfo); });
            }

            if (x < _maxRow - 1) {
                x++;
            } else {
                x = 0;
                y++;
            }

            _maxCollumn = y;
        }
    }

    private void HandleSelectScreenInput(PlayerChoose pl, string playerId) {

        #region Grid Navigation    

        if (Input.GetButtonDown("Right" + playerId)) {
            pl.ActiveX = (pl.ActiveX + 1) % _maxRow;
        }

        if (Input.GetButtonDown("Left" + playerId)) {
            pl.ActiveX = (pl.ActiveX + _maxRow - 1) % _maxRow;
        }

        if (Input.GetButtonDown("Jump" + playerId)) {
            pl.ActiveY = (pl.ActiveY + _maxCollumn - 1) % _maxCollumn;
        }

        if (Input.GetButtonDown("Crouch" + playerId)) {
            pl.ActiveY = (pl.ActiveY + 1) % _maxCollumn;
        }

        #endregion

        // 按P攻击选择角色
        if (Input.GetButtonUp("P" + playerId)) {
            // TODO 给角色一个选中的动作
            pl.CreatedCharacter.GetComponentInChildren<Animator>().Play("AttackP");

            // 拿到选中角色的prefab
            pl.PlayerBase.PlayerPrefab =
                _charManager.GetCharacterById(pl.ActivePotrait.CharacterId).Prefab;

            pl.PlayerBase.HasCharacter = true;
        }
    }

    private IEnumerator LoadLevel() {
        // 给AI设置随机角色，但是为啥这里要设置一遍？有何意义！
        /*
        foreach (var player in _charManager.Players) {
            if (player.Type == PlayerBase.PlayerType.Ai) {
                if (player.PlayerPrefab == null) {
                    int ranValue = Random.Range(0, _potraitList.Count);

                    player.PlayerPrefab =
                        _charManager.GetCharacterById(_potraitList[ranValue].CharacterId).Prefab;
                }
            }
        }*/

        yield return new WaitForSeconds(0.1f);
        // TODO 都准备好了的动作
        /*
        for (var i = 0; i < PlayerChooseList.Count; i++) {
            if (i < NumberOfPlayers) {
                PlayerChooseList[i].CreatedCharacter.GetComponentInChildren<Animator>().Play("AttackHS");
            }
        }*/
        LoadingCanvas.SetActive(true);
        yield return new WaitForSeconds(0.3f);

        if (_charManager.IsSolo) {
            GameSceneManager.GetInstance().CreateProgression();
            GameSceneManager.GetInstance().LoadNextOnProgression();
        } else {
            GameSceneManager.GetInstance().RequestLevelLoad(SceneType.Proggression, SceneName.BATTLE);
        }

    }

    private void HandleSelectorPosition(PlayerChoose playerChoose) {
        playerChoose.Selector.SetActive(true);

        var potraitInfo = ReturnPotrait(playerChoose.ActiveX, playerChoose.ActiveY);

        if (potraitInfo == null) return;

        playerChoose.ActivePotrait = potraitInfo;
        // 把selector移动到框框上
        var selectorPosition = playerChoose.ActivePotrait.transform.localPosition;
        selectorPosition += PotraitCanvas.transform.localPosition;
        playerChoose.Selector.transform.localPosition = selectorPosition;
    }

    void HandleCharacterPreview(PlayerChoose playerChoose) {
        // 如果选择的头像和显示的不同表示换了一个角色
        if (playerChoose.PreviewPotrait != playerChoose.ActivePotrait) {
            if (playerChoose.CreatedCharacter != null) {
                Destroy(playerChoose.CreatedCharacter);
            }

            var playerObject = Instantiate(
                    GameManager.GetInstance().GetCharacterById(playerChoose.ActivePotrait.CharacterId).Prefab,
                    playerChoose.CharacterVisualizationPosition.position,
                    Quaternion.identity)
                as GameObject;

            if (playerObject != null) {
                // 播放一个跳下来的动作
                playerObject.GetComponentInChildren<Animator>().SetBool("Jump", true);
                playerObject.GetComponentInChildren<Animator>().Play("JumpNormal", 0, 0.6f);

                playerChoose.CreatedCharacter = playerObject;
                playerChoose.PreviewPotrait = playerChoose.ActivePotrait;

                // 让玩家2转向
                if (!string.Equals(playerChoose.PlayerBase.PlayerId, _charManager.Players[0].PlayerId)) {
                    if (playerChoose.CreatedCharacter != null) {
                        playerChoose.CreatedCharacter.GetComponentInChildren<SpriteRenderer>().transform.rotation =
                            Quaternion.Euler(0, 180, 0);
                    }
                }
            }
        }
    }


    private PotraitInfo ReturnPotrait(int x, int y) {
        PotraitInfo returnValue = null;

        foreach (var potraitInfo in _potraitList) {
            if (potraitInfo.PositionX == x && potraitInfo.PositionY == y) {
                returnValue = potraitInfo;
            }
        }

        return returnValue;
    }

    // Mobile
    private void OnClick(PotraitInfo potraitInfo) {

        if (!_charManager.Players[0].HasCharacter) { // 玩家1没选择,给玩家一选角色
            PlayerChooseList[0].ActiveX = potraitInfo.PositionX;
            PlayerChooseList[0].ActiveY = potraitInfo.PositionY;
            PlayerChooseList[0].ActivePotrait = ReturnPotrait(potraitInfo.PositionX, potraitInfo.PositionY);
            PlayerChooseList[0].Selector.SetActive(true);
            PlayerChooseList[0].CreatedCharacter.GetComponentInChildren<Animator>().Play("AttackP");

            PlayerChooseList[0].PlayerBase.PlayerPrefab =
                _charManager.GetCharacterById(PlayerChooseList[0].ActivePotrait.CharacterId).Prefab;

            PlayerChooseList[0].PlayerBase.HasCharacter = true;
        } else if (!_charManager.Players[1].HasCharacter) {
            PlayerChooseList[1].ActiveX = potraitInfo.PositionX;
            PlayerChooseList[1].ActiveY = potraitInfo.PositionY;
            PlayerChooseList[1].ActivePotrait = ReturnPotrait(potraitInfo.PositionX, potraitInfo.PositionY);
            PlayerChooseList[1].Selector.SetActive(true);
            PlayerChooseList[1].CreatedCharacter.GetComponentInChildren<Animator>().Play("AttackP");

            PlayerChooseList[1].PlayerBase.PlayerPrefab =
                _charManager.GetCharacterById(PlayerChooseList[0].ActivePotrait.CharacterId).Prefab;

            PlayerChooseList[1].PlayerBase.HasCharacter = true;
        }

    }
}