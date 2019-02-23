using System;
using UnityEngine;

public class InputHandler : MonoBehaviour {
    public string PlayerInputId;

    private PlayerStateManager _states;
    private Animator _animator;
    private AttacksBase[] _attacks;

    private bool _jumpButtonUp;

    private readonly DoubleClick _rightDoubleClick = new DoubleClick();
    private readonly DoubleClick _leftDoubleClick = new DoubleClick();

    private readonly DoubleClick _rightDoubleClickJoystick = new DoubleClick();
    private readonly DoubleClick _leftDoubleClickJoystick = new DoubleClick();

    public void Start() {
        _states = GetComponent<PlayerStateManager>();
        _animator = GetComponent<PlayerAnimationHandler>().Animator;
        _attacks = _states.Attacks;
    }

    public void FixedUpdate() {
        Attack();
        Move();
        Jump();
        Crouch();
        Defense();
    }

    private void Defense() {
        _states.DefenseLeft = Input.GetButton("Left" + PlayerInputId) || Input.GetAxis("JoystickX" + PlayerInputId) > 0;

        _states.DefenseRight =
            Input.GetButton("Right" + PlayerInputId) || Input.GetAxis("JoystickX" + PlayerInputId) < 0;
    }

    private void Crouch() {
        _states.Crouch = Input.GetButton("Crouch" + PlayerInputId) ||
                         Math.Abs(Input.GetAxis("JoystickY" + PlayerInputId) - -1) < 0.01;
    }

    private void Move() {
        if (_animator.GetBool(AnimatorBool.MOVEABLE)) {
            _states.Right = Input.GetButton("Right" + PlayerInputId) || Input.GetAxis("JoystickX" + PlayerInputId) < 0;
            _states.Left = Input.GetButton("Left" + PlayerInputId) || Input.GetAxis("JoystickX" + PlayerInputId) > 0;

            _leftDoubleClick.HandleDoubleClick("Left" + PlayerInputId, () => { _states.LeftDouble = true; });
            _rightDoubleClick.HandleDoubleClick("Right" + PlayerInputId, () => { _states.RightDouble = true; });

            // Joystick
            _leftDoubleClickJoystick.HandleDoubleBool(Math.Abs(Input.GetAxis("JoystickX" + PlayerInputId) - 1) < 0.01f,
                () => { _states.LeftDouble = true; });

            _rightDoubleClickJoystick.HandleDoubleBool(
                Math.Abs(Input.GetAxis("JoystickX" + PlayerInputId) - -1) < 0.01f,
                () => { _states.RightDouble = true; });

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
            // TODO 这里应该做一个树的优先级
            foreach (var attack in _attacks) {
                attack.Do(PlayerInputId, _states.LookRight);
            }
        } else {
            foreach (var attack in _attacks) {
                attack.Reset();
            }
        }
    }

    private void Jump() {
        // 普通跳
        if (_animator.GetBool(AnimatorBool.JUMPABLE)) {
            if (Input.GetButtonDown("Jump" + PlayerInputId) ||
                // Joystick
                Math.Abs(Input.GetAxis("JoystickY" + PlayerInputId) - 1) < 0.01) {
                _states.Jump = true;
            }

            _jumpButtonUp = false; // 初始化跳跃键没松开

            if (_states.Jump) {
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
                _jumpButtonUp = Input.GetButtonUp("Jump" + PlayerInputId) ||
                                // Joystick
                                Math.Abs(Input.GetAxis("JoystickY" + PlayerInputId) - -1) < 0.01; // 检测是否松开
            } else {
                if (Input.GetButtonDown("Jump" + PlayerInputId)) {
                    _states.JumpDouble = true;
                }

                if (_states.JumpDouble) {
                    _states.RightDouble = false;
                    _states.LeftDouble = false;
                    _leftDoubleClick.Reset();
                    _rightDoubleClick.Reset();
                    _rightDoubleClick.Reset();

                    _states.JumpLeft = Input.GetButton("Left" + PlayerInputId) ||
                                       Input.GetAxis("JoystickX" + PlayerInputId) > 0;

                    _states.JumpRight = Input.GetButton("Right" + PlayerInputId) ||
                                        Input.GetAxis("JoystickX" + PlayerInputId) < 0;

                    _jumpButtonUp = false;
                }
            }
        } else {
            _states.JumpDouble = false;
        }

        // 高跳
        if (_animator.GetBool(AnimatorBool.HIGH_JUMPABLE)) {
            if (Input.GetButtonDown("Jump" + PlayerInputId) ||
                // Joystick
                Math.Abs(Input.GetAxis("JoystickY" + PlayerInputId) - 1) < 0.01) {
                _states.Jump = true;
            }

            _states.JumpHigh = _states.Jump;
            _jumpButtonUp = false; // 初始化跳跃键没松开

            if (_states.JumpHigh) {
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