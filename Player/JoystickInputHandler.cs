
using System;
using JetBrains.Annotations;
using UnityEngine;

public class JoystickInputHandler : IInputHandler {

    private PlayerStateManager _states;
    private PlayerAnimationHandler _animation;
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private MovementHandler _movement;
    private InputHandler _input;
    private bool _jumpButtonUp;

    private readonly DoubleClick _rightDoubleClick = new DoubleClick();
    private readonly DoubleClick _leftDoubleClick = new DoubleClick();
    
    public JoystickInputHandler(InputHandler input) {
        _input = input;
        _states = input.GetComponent<PlayerStateManager>();
        _animation = _states.AnimationHandler;
        _animator = _animation.Animator;
        _rigidbody = _states.GetComponent<Rigidbody2D>();
        _movement = _states.GetComponent<MovementHandler>();      
    }

    public void HandleAttack() {

        if (_states.Attackable) {
            foreach (var attack in _states.Attacks) {
                if (attack.AttackAnimName == "FirePunch") {
                    attack.DoJoystick(_input.PlayerInputId, _states.LookRight, () => {
                        _animator.SetBool("Jump", true);

                        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x,
                            _movement.JumpSpeed);
                    });
                } else {
                    attack.DoJoystick(_input.PlayerInputId, _states.LookRight);
                }

            }
        } else {
            foreach (var attack in _states.Attacks) {
                attack.Reset();
            }
        }
    }

    public void HandleMove() {
        if (_animator.GetBool(AnimatorBool.MOVEABLE)) {
            _states.Right = Input.GetAxis("JoystickX" + _input.PlayerInputId) < 0;
            _states.Left = Input.GetAxis("JoystickX" + _input.PlayerInputId) > 0;

            _leftDoubleClick.HandleDoubleClickWithAxis("JoystickX" + _input.PlayerInputId, true, () => { _states.LeftDouble = true; });
            _rightDoubleClick.HandleDoubleClickWithAxis("JoystickX" + _input.PlayerInputId, false, () => { _states.RightDouble = true; });

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

    public void HandleJump() {
        if (_animator.GetBool(AnimatorBool.JUMPABLE)) {
            if (Input.GetAxis("JoystickY" + _input.PlayerInputId) > 0) {
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
                _jumpButtonUp = Input.GetAxis("JoystickY" + _input.PlayerInputId) <= 0; // 检测是否松开
            } else {
                if (Input.GetAxis("JoystickY" + _input.PlayerInputId) > 0) {
                    _states.JumpDouble = true;
                }
                
                if (_states.JumpDouble) {
                    //_animator.SetTrigger("JumpDouble");
                    _states.RightDouble = false;
                    _states.LeftDouble = false;
                    _leftDoubleClick.Reset();
                    _rightDoubleClick.Reset();
                    _rightDoubleClick.Reset();

                    _states.JumpLeft = Input.GetAxis("JoystickX" + _input.PlayerInputId) > 0;
                    _states.JumpRight = Input.GetAxis("JoystickX" + _input.PlayerInputId) < 0;
                    
                    _jumpButtonUp = false;
                }              
            }
        } else {
            _states.JumpDouble = false;
        }
        
        // 高跳
        if (_animator.GetBool(AnimatorBool.HIGH_JUMPABLE)) {
            if (Input.GetAxis("JoystickY" + _input.PlayerInputId) > 0) {
                _states.JumpDouble = true;
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

    public void HandleCrouch() {
        _states.Crouch = Input.GetAxis("JoystickY" + _input.PlayerInputId) < 0;
    }
}
