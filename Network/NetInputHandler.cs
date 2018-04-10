using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetInputHandler : NetworkBehaviour {

	public string PlayerInputId;

    private NetPlayerStateManager _states;
    private Animator _animator;
    private AttacksBase[] _attacks;

    private bool _jumpButtonUp;

    private readonly DoubleClick _rightDoubleClick = new DoubleClick();
    private readonly DoubleClick _leftDoubleClick = new DoubleClick();

    private IInputHandler _input;

    public void Start() {
        _states = GetComponent<NetPlayerStateManager>();
        _animator = GetComponent<NetAnimationHandler>().Animator;
        _attacks = _states.Attacks;
    }

    public void FixedUpdate() {
        if (!isLocalPlayer) { // 非本地用户不控制
            return;
        }
 
        Attack();
        Move();
        Jump();
        Crouch();
        Defense();
    }

    private void Defense() {
        _states.CmdDefenseLeft(Input.GetButton("Left" + PlayerInputId));
        _states.CmdDefenseRight(Input.GetButton("Right" + PlayerInputId));
    }

    private void Crouch() {
        _states.CmdCouch(Input.GetButton("Crouch" + PlayerInputId));
    }

    private void Move() {
        if (_animator.GetBool(AnimatorBool.MOVEABLE)) {
            _states.CmdRight(Input.GetButton("Right" + PlayerInputId));
            _states.CmdLeft(Input.GetButton("Left" + PlayerInputId));

            _leftDoubleClick.HandleDoubleClick("Left" + PlayerInputId, () => { _states.CmdLeftDouble(true); });
            _rightDoubleClick.HandleDoubleClick("Right" + PlayerInputId, () => { _states.CmdRightDouble(true); });

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
            if (Input.GetButtonDown("Jump" + PlayerInputId)) {
                _states.CmdJump(true);
            }

            _jumpButtonUp = false; // 初始化跳跃键没松开

            if (_states.Jump) {
                //_animator.SetBool("Jump", true);
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
                _jumpButtonUp = Input.GetButtonUp("Jump" + PlayerInputId); // 检测是否松开
            } else {
                if (Input.GetButtonDown("Jump" + PlayerInputId)) {
                    _states.CmdJumpDouble(true);
                }

                if (_states.JumpDouble) {
                    //_animator.SetTrigger("JumpDouble");
                    _states.CmdRightDouble(false);
                    _states.CmdLeftDouble(false);
                    _leftDoubleClick.Reset();
                    _rightDoubleClick.Reset();

                    _states.CmdJumpLeft(Input.GetButton("Left" + PlayerInputId));
                    _states.CmdJumpRight(Input.GetButton("Right" + PlayerInputId));

                    _jumpButtonUp = false;
                }
            }
        } else {
            _states.CmdJumpDouble(false);
        }

        // 高跳
        if (_animator.GetBool(AnimatorBool.HIGH_JUMPABLE)) {
            if (Input.GetButtonDown("Jump" + PlayerInputId)) {
                _states.CmdJump(true);
            }

            _states.CmdJumpHigh(_states.Jump);
            _jumpButtonUp = false; // 初始化跳跃键没松开

            if (_states.JumpHigh) {
                //_animator.SetBool("Jump", true);
                //_animator.SetBool("JumpHigh", true);
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
