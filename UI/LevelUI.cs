using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelUI : MonoBehaviour {
    public Text AnnouncerTextLine1;
    public Text AnnouncerTextLine2;
    public Text LevelTimer;

    public Slider[] HealthSliders;
    public HitText[] HitTexts;

    public GameObject[] WinIndicatorGrids;
    public GameObject WinIndicator;

    public Image RoundXFightImage;
    public Sprite[] RoundSprites;
    public Sprite FightSprite;

    public GameObject ShowPlayerPosterCanvas;
    public Image Player1Poster;
    public Image Player2Poster;
    public Text Player1Name;
    public Text Player2Name;
    public Text VS;

    public GameObject EasyTouchControlsCanvas;

    #region Singleton
    
    private static LevelUI _instance;

    public static LevelUI GetInstance() {
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

    public void AddWinIndicator(int player) {
        var go = Instantiate(WinIndicator, Vector3.zero, Quaternion.identity);
        if (go != null) {
            go.transform.SetParent(WinIndicatorGrids[player].transform);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = new Vector3(0, 0, -10); // 不然会给个随机的z轴
        }
    }

    public IEnumerator ShowPlayersPoster(CharacterBase character1, CharacterBase character2) {
        ShowPlayerPosterCanvas.SetActive(true);
        Player1Poster.sprite = character1.Icon;
        Player1Name.text = character1.Prefab.name;
        Player1Poster.gameObject.SetActive(true);
        Player1Name.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        Player2Poster.sprite = character2.Icon;
        Player2Name.text = character2.Prefab.name;
        Player2Poster.gameObject.SetActive(true);
        Player2Name.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        VS.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        VS.gameObject.SetActive(false);
        Player2Poster.gameObject.SetActive(false);
        Player2Name.gameObject.SetActive(false);
        Player1Poster.gameObject.SetActive(false);
        Player1Name.gameObject.SetActive(false);
        ShowPlayerPosterCanvas.SetActive(false);
    }

    public IEnumerator RoundXFight(int currentRounds) {
        RoundXFightImage.sprite = RoundSprites[currentRounds - 1];
        RoundXFightImage.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(2);

        RoundXFightImage.sprite = FightSprite;

        // 过一秒让提示消失
        yield return new WaitForSeconds(1);

        RoundXFightImage.gameObject.SetActive(false);
    }
}