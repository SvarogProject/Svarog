using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour {
    public string PlayerInputId;

    private PlayerStateManager _states;
    private Animator _animator;
    private AttacksBase[] _attacks;

    private readonly DoubleClick _rightDoubleClick = new DoubleClick();
    private readonly DoubleClick _leftDoubleClick = new DoubleClick();

    public void Start() {
        _states = GetComponent<PlayerStateManager>();
        _animator = GetComponent<PlayerAnimationHandler>().Animator;
        _attacks = _states.Attacks;
    }

    public void FixedUpdate() {

        Attack();
        Jump();
        Move();
        _states.Crouch = Input.GetButton("Crouch" + PlayerInputId);

        if (Input.GetKey(KeyCode.P)) {
            _animator.SetTrigger("HurtSmall"); // TODO TEST
        }
    }

    private void Move() {
        if (_animator.GetBool(AnimatorBool.MOVEABLE)) {
            _states.Right = Input.GetButton("Right" + PlayerInputId);
            _states.Left = Input.GetButton("Left" + PlayerInputId);

            _leftDoubleClick.HandleDoubleClick("Left" + PlayerInputId, () => { _states.LeftDouble = true; });
            _rightDoubleClick.HandleDoubleClick("Right" + PlayerInputId, () => { _states.RightDouble = true; });

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
            foreach (var attack in _attacks) {
                DoAttack(attack);
            }
        } else {
            foreach (var attack in _attacks) {
                attack.Reset();
            }
        }
    }

    private void Jump() {
        if (_animator.GetBool(AnimatorBool.JUMPABLE)) {
            _states.Jump = Input.GetButtonDown("Jump" + PlayerInputId);

            if (_states.Jump) {
                _states.RightDouble = false;
                _states.LeftDouble = false;
                _leftDoubleClick.Reset();
                _rightDoubleClick.Reset();
            }
        } else {
            _states.Jump = false;
        }
    }

    private void DoAttack(AttacksBase attack) {
        if (Input.GetButtonDown(attack.AttackButtonName + PlayerInputId)) {
            attack.Do();
        }

        if (attack.Attack) {
            attack.AttackTimer += Time.deltaTime;

            if (attack.AttackTimer > attack.AttackRate || attack.TimesPressed >= 3) {
                attack.Reset();
            }
        }
    }
}