using UnityEngine;

[System.Serializable]
public class PlayerChoose {
    public PotraitInfo ActivePotrait;                // 选中的头像
    public PotraitInfo PreviewPotrait;               // 展示的头像
    public GameObject Selector;                      // 选择框
    public Transform CharacterVisualizationPosition; // 显示角色的位置
    public GameObject CreatedCharacter;              // 创建的角色

    public int ActiveX; // 当前选择的x y坐标
    public int ActiveY;

    // variables for smoothing out input
    public bool HitInputOnce;
    public float TimerToReset;

    public PlayerBase PlayerBase;
}