using UnityEngine;
using UnityEngine.Networking;

public class NetMobilePlayerInputManager : NetworkBehaviour {
    public ETCJoystick Joystick;
    public ETCButton ButtonAttackP;
    public ETCButton ButtonAttackK;
    public ETCButton ButtonAttackS;
    public ETCButton ButtonAttackHS;
    public ETCButton ButtonJump;
    public ETCButton ButtonCrouch;

    private NetPlayerStateManager _states;
    private Animator _animator;
    private AttacksBase[] _attacks;

    private bool _jumpButtonUp;

    private readonly DoubleClick _rightDoubleClick = new DoubleClick();
    private readonly DoubleClick _leftDoubleClick = new DoubleClick();

    private bool _isAttack;

    public void Start() {
        if (!MobileManager.IsMobile) {
            enabled = false;
        }
        _states = GetComponent<NetPlayerStateManager>();
        _animator = GetComponent<NetAnimationHandler>().Animator;
        _attacks = _states.Attacks;
        
        var ui = GameObject.Find("UI").GetComponent<NetLevelUI>();
        Joystick = ui.Joystick;
        ButtonAttackP = ui.ButtonAttackP;
        ButtonAttackK = ui.ButtonAttackK;
        ButtonAttackS = ui.ButtonAttackS;
        ButtonAttackHS = ui.ButtonAttackHS;
        ButtonJump = ui.ButtonJump;
        ButtonCrouch = ui.ButtonCrouch;
    }

    public void FixedUpdate() {
        if (isLocalPlayer) {
            Attack();
            Move();
            Jump();
            Crouch();
            Defense();
        }     
    }

    private void Defense() {
        _states.CmdDefenseLeft(Joystick.axisX.axisState == ETCAxis.AxisState.PressLeft);
        _states.CmdDefenseRight(Joystick.axisX.axisState == ETCAxis.AxisState.PressRight);
    }

    private void Crouch() {
        _states.Crouch = Joystick.axisY.axisState == ETCAxis.AxisState.PressDown ||
                         ButtonCrouch.axis.axisState == ETCAxis.AxisState.Press;
    }

    private void Move() {
        if (_animator.GetBool(AnimatorBool.MOVEABLE)) {
            _states.CmdRight(Joystick.axisX.axisState == ETCAxis.AxisState.PressRight);
            _states.CmdLeft(Joystick.axisX.axisState == ETCAxis.AxisState.PressLeft);

            _leftDoubleClick.HandleDoubleBool(Joystick.axisX.axisState == ETCAxis.AxisState.PressLeft,
                () => { _states.CmdLeftDouble(true); });

            _rightDoubleClick.HandleDoubleBool(Joystick.axisX.axisState == ETCAxis.AxisState.PressRight,
                () => { _states.CmdRightDouble(true); });

            if (!_states.Right) {
                _states.CmdRightDouble(false);
            }

            if (!_states.Left) {
                _states.CmdLeftDouble(false);
            }
        } else {
            _states.CmdRight(false);
            _states.CmdLeft(false);
            _states.CmdRightDouble(false);
            _states.CmdLeftDouble(false);
            _leftDoubleClick.Reset();
            _rightDoubleClick.Reset();
        }
    }

    private void Attack() {
        if (_states.Attackable) {
            if (ButtonAttackP.axis.axisState == ETCAxis.AxisState.Press) {
                _attacks[0].Attack = !_isAttack;
            } else {
                _isAttack = false;
                _attacks[0].Attack = false;
            }

            if (ButtonAttackK.axis.axisState == ETCAxis.AxisState.Press) {
                _attacks[1].Attack = !_isAttack;
            } else {
                _isAttack = false;
                _attacks[1].Attack = false;
            }

            if (ButtonAttackS.axis.axisState == ETCAxis.AxisState.Press) {
                _attacks[2].Attack = !_isAttack;
            } else {
                _isAttack = false;
                _attacks[2].Attack = false;
            }

            if (ButtonAttackHS.axis.axisState == ETCAxis.AxisState.Press) {
                _attacks[3].Attack = !_isAttack;
            } else {
                _isAttack = false;
                _attacks[3].Attack = false;
            }
        } else {
            _isAttack = false;

            foreach (var attack in _attacks) {
                attack.Reset();
            }
        }
    }

    private void Jump() {
        // 普通跳
        if (_animator.GetBool(AnimatorBool.JUMPABLE)) {
            if (Joystick.axisY.axisState == ETCAxis.AxisState.DownUp ||
                ButtonJump.axis.axisState == ETCAxis.AxisState.Press) {
                _states.CmdJump(true);
            }

            _jumpButtonUp = false; // 初始化跳跃键没松开

            if (_states.Jump) {
                _states.CmdRightDouble(false);
                _states.CmdLeftDouble(false);
                _leftDoubleClick.Reset();
                _rightDoubleClick.Reset();
            }
        } else {
            _states.CmdJump(false);
        }

        // 二段跳
        if (_animator.GetBool(AnimatorBool.JUMP) && !_animator.GetBool(AnimatorBool.USED_JUMP_DOUBLE)) {
            if (!_jumpButtonUp) {
                _jumpButtonUp = Joystick.axisY.axisState == ETCAxis.AxisState.DownDown; // 检测是否松开
            } else {
                if (Joystick.axisY.axisState == ETCAxis.AxisState.DownUp) {
                    _states.CmdJumpDouble(true);
                }

                if (_states.JumpDouble) {
                    _states.CmdRightDouble(false);
                    _states.CmdLeftDouble(false);
                    _leftDoubleClick.Reset();
                    _rightDoubleClick.Reset();
                    _rightDoubleClick.Reset();

                    _states.CmdJumpLeft(Joystick.axisX.axisState == ETCAxis.AxisState.PressLeft);
                    _states.CmdJumpRight(Joystick.axisX.axisState == ETCAxis.AxisState.PressRight);

                    _jumpButtonUp = false;
                }
            }
        } else {
            _states.CmdJumpDouble(false);
        }

        // 高跳
        if (_animator.GetBool(AnimatorBool.HIGH_JUMPABLE)) {
            if (Joystick.axisY.axisState == ETCAxis.AxisState.DownUp) {
                _states.CmdJump(true);
            }

            _states.CmdJumpHigh(_states.Jump);
            _jumpButtonUp = false; // 初始化跳跃键没松开

            if (_states.JumpHigh) {
                _states.CmdRightDouble(false);
                _states.CmdLeftDouble(false);
                _leftDoubleClick.Reset();
                _rightDoubleClick.Reset();
            }
        } else {
            _states.CmdJumpHigh(false);
        }
    }
}