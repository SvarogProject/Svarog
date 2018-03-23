using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour {
    public int ProgressionStages = 5; // 游戏局数

    public List<string>
        Levels = new List<string>(); // 对局得种类 TODO 这个设置多个其实就是场景不一样，设置场景应该放在LevelManager中设置

    public List<MainScene> MainScenes = new List<MainScene>();               // 其他的主页面（菜单、选人、游戏结束）
    public int NextProgressionIndex;                                         // 下一次对局的下标
    public List<SoloProgression> Progressions = new List<SoloProgression>(); // 对局

    private bool _waitToLoad;

    private CharacterManager _characterManager;

    #region Singleton

    private static GameSceneManager _instance;

    public static GameSceneManager GetInstance() {
        return _instance;
    }

    public void Awake() {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    public void Start() {
        _characterManager = CharacterManager.GetInstance();
    }

    public void CreateProgression() {
        Progressions.Clear();
        NextProgressionIndex = 0;

        var usedCharacters = new List<int>();

        var playerChoosedCharacter = _characterManager.GetCharacterIndex(_characterManager.Players[0].PlayerPrefab);
        usedCharacters.Add(playerChoosedCharacter); // 玩家已经选择了的角色先排除掉

        if (ProgressionStages > _characterManager.CharacterList.Count - 1) { // 游戏局数应该小于和所有人对局完的总数
            ProgressionStages = _characterManager.CharacterList.Count - 2;
        }

        for (var i = 0; i < ProgressionStages; i++) {
            var progression = new SoloProgression();

            // TODO 这里换场景应该给LevelManager设置场景，或者在progression中保存一个
            var levelInt = Random.Range(0, Levels.Count);
            progression.LevelId = Levels[levelInt];

            // 随机一个角色，排除已用过的角色
            var charInt = Tools.UniqueRandomInt(usedCharacters, 0, _characterManager.CharacterList.Count);
            progression.CharId = _characterManager.CharacterList[charInt].CharacterId;
            usedCharacters.Add(charInt);
            Progressions.Add(progression);
        }
    }

    public void LoadNextOnProgression() {
        string targetId;
        var sceneType = SceneType.Proggression;

        if (NextProgressionIndex > Progressions.Count - 1) { // 全打完了，回到主页面 TODO 这里应该加一个通关页面
            targetId = SceneName.INTRO;
            sceneType = SceneType.Main;
        } else {
            targetId = Progressions[NextProgressionIndex].LevelId; // TODO 这个也不会变了，都是一个场景了

            // 得到敌人
            _characterManager.Players[1].PlayerPrefab =
                _characterManager.GetCharacterById(Progressions[NextProgressionIndex].CharId).Prefab;

            NextProgressionIndex++;
        }

        RequestLevelLoad(sceneType, targetId);
    }

    public void RequestLevelLoad(SceneType type, string levelId) {
        if (_waitToLoad) return; // 如果已经准备读取了，就别读了

        /*  这一段得到id又何意义？！
        string targetId = "";
        
        
        switch (type) {
           case SceneType.Main:
               targetId = ReturnMainScene(level).LevelId;
        
               break;
           case SceneType.Proggression:
               targetId = level;
        
               break;
        }
        */

        StartCoroutine(LoadScene(levelId));
        _waitToLoad = true;

    }

    private IEnumerator LoadScene(string levelId) {
        yield return SceneManager.LoadSceneAsync(levelId, LoadSceneMode.Single);

        _waitToLoad = false;
    }

    private MainScene ReturnMainScene(string level) {
        MainScene returnValue = null;

        foreach (var mainScene in MainScenes) {
            if (mainScene.LevelId == level) {
                returnValue = mainScene;
                break;
            }
        }

        return returnValue;
    }
}