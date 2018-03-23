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
    private const float MinWaitTime = 0.05f;
    private const string JoystickPre = "Joystick_";

    public delegate void OnAttack();

    public void Reset() {
        Attack = false;
        AttackTimer = 0;
        TimesPressed = 0;
    }

    public void DoJoystick(string playerInputId, bool lookRight, OnAttack onAttack = null) {
        if (IsCombSkill) {
            DoCombJoystick(playerInputId, lookRight, onAttack);

            return;
        }

        if (Input.GetButtonDown(JoystickPre + AttackButtonName + playerInputId)) {
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

    private void DoCombJoystick(string playerInputId, bool lookRight, OnAttack onAttack) {
        if (_currentComb == CombButtonNames.Length) { // 组合技完成，释放技能
            Attack = true;
            AttackTimer = 0;
            _currentComb = 0;
            _waitTimer = 0;

            if (onAttack != null) {
                onAttack();
            }
        }

        if (Attack) {
            AttackTimer += Time.deltaTime;

            if (AttackTimer > AttackRate) {
                Reset();
            }
        }


        // 判定组合技
        if (Input.anyKeyDown && _waitTimer >= MinWaitTime) {
            bool clickTrue;

            if (!lookRight) {
                if (CombButtonNames[_currentComb] == "Left") {
                    clickTrue = Input.GetAxis("JoystickX" + playerInputId) < 0;
                } else if (CombButtonNames[_currentComb] == "Right") {
                    clickTrue = Input.GetAxis("JoystickX" + playerInputId) > 0;
                } else if (CombButtonNames[_currentComb] == "Crouch") {
                    clickTrue = Input.GetAxis("JoystickY" + playerInputId) < 0;
                } else {
                    clickTrue = Input.GetButtonDown(JoystickPre + CombButtonNames[_currentComb] + playerInputId);
                }
            } else {
                if (CombButtonNames[_currentComb] == "Left") {
                    clickTrue = Input.GetAxis("JoystickX" + playerInputId) > 0;
                } else if (CombButtonNames[_currentComb] == "Right") {
                    clickTrue = Input.GetAxis("JoystickX" + playerInputId) < 0;
                } else if (CombButtonNames[_currentComb] == "Crouch") {
                    clickTrue = Input.GetAxis("JoystickY" + playerInputId) < 0;
                } else {
                    clickTrue = Input.GetButtonDown(JoystickPre + CombButtonNames[_currentComb] + playerInputId);
                }
            }

            if (clickTrue) {
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

    public void Do(string playerInputId, bool lookRight, OnAttack onAttack = null) {
        if (IsCombSkill) {
            DoComb(playerInputId, lookRight, onAttack);

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

    private void DoComb(string playerInputId, bool lookRight, OnAttack onAttack) {
        if (_currentComb == CombButtonNames.Length) { // 组合技完成，释放技能
            Attack = true;
            AttackTimer = 0;
            _currentComb = 0;
            _waitTimer = 0;

            if (onAttack != null) {
                onAttack();
            }
        }

        if (Attack) {
            AttackTimer += Time.deltaTime;

            if (AttackTimer > AttackRate) {
                Reset();
            }
        }


        // 判定组合技
        if (Input.anyKeyDown && _waitTimer >= MinWaitTime) {
            bool clickTrue;

            if (!lookRight) {
                if (CombButtonNames[_currentComb] == "Left") {
                    clickTrue = Input.GetButtonDown("Right" + playerInputId);
                } else if (CombButtonNames[_currentComb] == "Right") {
                    clickTrue = Input.GetButtonDown("Left" + playerInputId);
                } else {
                    clickTrue = Input.GetButtonDown(CombButtonNames[_currentComb] + playerInputId);
                }
            } else {
                clickTrue = Input.GetButtonDown(CombButtonNames[_currentComb] + playerInputId);
            }

            if (clickTrue) {
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