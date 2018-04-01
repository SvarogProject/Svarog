
using UnityEngine;

public class MobilePlayerInputManager : MonoBehaviour {
    public ETCJoystick Joystick;
    public ETCButton ButtonAttackP;
    
        
    private PlayerStateManager _states;
    private Animator _animator;
    private AttacksBase[] _attacks;

    private bool _jumpButtonUp;

    private readonly DoubleClick _rightDoubleClick = new DoubleClick();
    private readonly DoubleClick _leftDoubleClick = new DoubleClick();

    private bool _isAttack = false;
    
    public void Start() {
        _states = GetComponent<PlayerStateManager>();
        _animator = GetComponent<PlayerAnimationHandler>().Animator;
        _attacks = _states.Attacks;
        Joystick = LevelManager.GetInstance().Joystick;
        ButtonAttackP = LevelManager.GetInstance().ButtonAttackP;   
    }

    public void FixedUpdate() {
       
        Attack();
        Move();
        Jump();
        Crouch();
        Defense();
    }

    private void Defense() {
        _states.DefenseLeft = Joystick.axisX.axisState == ETCAxis.AxisState.PressLeft;
        _states.DefenseRight = Joystick.axisX.axisState == ETCAxis.AxisState.PressRight;
    }

    private void Crouch() {
        _states.Crouch = Joystick.axisY.axisState == ETCAxis.AxisState.PressDown;
    }

    private void Move() {
        if (_animator.GetBool(AnimatorBool.MOVEABLE)) {
            _states.Right = Joystick.axisX.axisState == ETCAxis.AxisState.PressRight;
            _states.Left = Joystick.axisX.axisState == ETCAxis.AxisState.PressLeft;

            //_leftDoubleClick.HandleDoubleClick("Left" + PlayerInputId, () => { _states.LeftDouble = true; });
            //_rightDoubleClick.HandleDoubleClick("Right" + PlayerInputId, () => { _states.RightDouble = true; });

            if (!_states.Right) {
                _states.RightDouble = false;
            }

            if (!_states.Left) {
                _states.LeftDouble = false;
            }
        } else {
            _states.Right = false;
            _states.Left = false;
            _states.RightDouble = false;
            _states.LeftDouble = false;
            _leftDoubleClick.Reset();
            _rightDoubleClick.Reset();
        }
    }

    private void Attack() {
        if (_states.Attackable) {
            Debug.Log(ButtonAttackP.axis.axisState);
            if (ButtonAttackP.axis.axisState == ETCAxis.AxisState.Press) {
                _attacks[0].Attack = !_isAttack;
            } else {
                 _isAttack = false;                
                _attacks[0].Attack = false;
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
            if (Joystick.axisY.axisState == ETCAxis.AxisState.DownUp) {
                _states.Jump = true;
            }

            _jumpButtonUp = false; // 初始化跳跃键没松开

            if (_states.Jump) {
                //_animator.SetBool("Jump", true);
                _states.RightDouble = false;
                _states.LeftDouble = false;
                _leftDoubleClick.Reset();
                _rightDoubleClick.Reset();
            }
        } else {
            _states.Jump = false;
        }

        // 二段跳
        if (_animator.GetBool(AnimatorBool.JUMP) && !_animator.GetBool(AnimatorBool.USED_JUMP_DOUBLE)) {
            if (!_jumpButtonUp) {
                _jumpButtonUp = Joystick.axisY.axisState == ETCAxis.AxisState.DownDown; // 检测是否松开
            } else {
                if (Joystick.axisY.axisState == ETCAxis.AxisState.DownUp) {
                    _states.JumpDouble = true;
                }

                if (_states.JumpDouble) {
                    //_animator.SetTrigger("JumpDouble");
                    _states.RightDouble = false;
                    _states.LeftDouble = false;
                    _leftDoubleClick.Reset();
                    _rightDoubleClick.Reset();
                    _rightDoubleClick.Reset();

                    _states.JumpLeft = Joystick.axisX.axisState == ETCAxis.AxisState.PressLeft;
                    _states.JumpRight = Joystick.axisX.axisState == ETCAxis.AxisState.PressRight;

                    _jumpButtonUp = false;
                }
            }
        } else {
            _states.JumpDouble = false;
        }

        // 高跳
        if (_animator.GetBool(AnimatorBool.HIGH_JUMPABLE)) {
            if (Joystick.axisY.axisState == ETCAxis.AxisState.DownUp) {
                _states.Jump = true;
            }

            _states.JumpHigh = _states.Jump;
            _jumpButtonUp = false; // 初始化跳跃键没松开

            if (_states.JumpHigh) {
                //_animator.SetBool("Jump", true);
                //_animator.SetBool("JumpHigh", true);
                _states.RightDouble = false;
                _states.LeftDouble = false;
                _leftDoubleClick.Reset();
                _rightDoubleClick.Reset();
            }
        } else {
            _states.JumpHigh = false;
        }
    }
}