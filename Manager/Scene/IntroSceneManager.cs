using UnityEngine;
using System.Collections;


public class IntroSceneManager : MonoBehaviour {
    public GameObject StartText;
    public int ActiveElement;
    public GameObject MenuObj;
    public ButtonRef[] MenuOptions;

    private float _timer;
    private bool _loadingLevel;
    private bool _init;

    public void Start() {
        // 开始时显示点击进入的画面，隐藏菜单
        MenuObj.SetActive(false);
    }

    public void Update() {

        if (!_init) {
            // 只是让点击进入的字样闪烁而已
            _timer += Time.deltaTime;

            if (_timer > 0.6f) {
                _timer = 0;
                StartText.SetActive(!StartText.activeInHierarchy);
            }

            // 点击进入 (这里用Down的话会快速触发下一次直接进入游戏，为什么Up就不会)
            if (Input.GetKeyUp(KeyCode.Space)) {
                _init = true;
                StartText.SetActive(false);
                MenuObj.SetActive(true);
            }
        } else {
            if (!_loadingLevel) {
                // 选中
                MenuOptions[ActiveElement].Selected = true;

                // 选择菜单
                if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetButtonUp("Jump")) {
                    MenuOptions[ActiveElement].Selected = false;

                    ActiveElement = (ActiveElement + MenuOptions.Length - 1) % MenuOptions.Length;
                }

                if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetButtonUp("Crouch")) {
                    MenuOptions[ActiveElement].Selected = false;

                    ActiveElement = (ActiveElement + 1) % MenuOptions.Length;
                }

                // 再次空格进入游戏
                if (Input.GetKeyUp(KeyCode.Space)) {
                    _loadingLevel = true;
                    StartCoroutine("LoadLevel");
                    // 让选中的菜单放大一下…… TODO 之后用Animator做
                    MenuOptions[ActiveElement].transform.localScale *= 1.2f;
                }
            }
        }

    }

    private void HandleSelectedOption() {
        
        switch (ActiveElement) {
            case 0: // 人机
                CharacterManager.GetInstance().NumberOfUsers = 1;
                CharacterManager.GetInstance().Players[1].Type = PlayerBase.PlayerType.Ai;

                break;
            case 1: // 双人
                CharacterManager.GetInstance().NumberOfUsers = 2;
                CharacterManager.GetInstance().Players[1].Type = PlayerBase.PlayerType.User;

                break;
        }
    }

    private IEnumerator LoadLevel() {
        HandleSelectedOption();

        yield return new WaitForSeconds(0.6f);

        GameSceneManager.GetInstance().RequestLevelLoad(SceneType.Main, SceneName.SELECT);

    }
}