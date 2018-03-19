using UnityEngine;

[System.Serializable]
public class AttacksBase {
    
    public string AttackAnimName;
    public string AttackButtonName;
    public bool Attack;
    public float AttackTimer;
    public int TimesPressed;
    public float AttackRate;
    public bool IsCombSkill;
    public string[] CombButtonNames;

    private int _currentComb;
    private float _waitTimer;
    private float _minWaitTime = 0.05f;

    public void Reset() {
        Attack = false;
        AttackTimer = 0;
        TimesPressed = 0;
    }

    public void Do(string playerInputId) {
        if (IsCombSkill) {
            DoComb(playerInputId);
            return;
        }
        
        if (Input.GetButtonDown(AttackButtonName + playerInputId)) {
            Attack = true;
            AttackTimer = 0;
            TimesPressed++;
        }
        if (Attack) {
            AttackTimer += Time.deltaTime;

            if (AttackTimer > AttackRate || TimesPressed >= 3) {
                Reset();
            }
        }
    }

    private void DoComb(string playerInputId) {
        if (_currentComb == CombButtonNames.Length) { // 组合技完成，释放技能
            Attack = true;
            AttackTimer = 0;
            _currentComb = 0;
            _waitTimer = 0;
        }

        if (Attack) {
            AttackTimer += Time.deltaTime;
            if (AttackTimer > AttackRate) {
                Reset();
            }
        }
        
        
        // 判定组合技
        if (Input.anyKeyDown && _waitTimer >= _minWaitTime) {
            if (Input.GetButtonDown(CombButtonNames[_currentComb] + playerInputId)) {
                _currentComb++;
                _waitTimer = 0;
            } else {
                _currentComb = 0; // 按下的不是我们想要的键，重新开始
                _waitTimer = 0;
            }
        } else {
            _waitTimer += Time.deltaTime;

            if (_waitTimer > 0.5f) { // 超时没按任何键，重新开始
                _currentComb = 0;
                _waitTimer = 0;
            }
        }
    }
}