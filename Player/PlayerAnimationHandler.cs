using UnityEngine;
using System.Collections;

public class PlayerAnimationHandler : MonoBehaviour {
    public Animator Animator;

    private PlayerStateManager _states;

    public void Start() {
        _states = GetComponent<PlayerStateManager>();
    }

    public void FixedUpdate() {

        //_states.Moveable = !Animator.GetBool("DontMove");
        Animator.SetBool("Dead", _states.Dead);

        if (_states.IsGettingHurtSmall) {
            Animator.SetTrigger("HurtSmall");
        }

        if (_states.IsGettinghurtLarge) {
            Animator.SetTrigger("HurtLarge");
        }

        if (_states.IsGettingFriePunch) {
            Animator.SetBool("HurtOnAir", true);
            Animator.SetTrigger("HurtLarge");
            Animator.SetBool("Jump", true);
        }

        Animator.SetBool("Crouch", _states.Crouch);

        if (_states.Jump) {
            JumpAnim();
        }

        if (_states.JumpHigh) {
            JumpHighAnim();
        }

        if (_states.JumpDouble) {
            JumpDoubleAnim();
        }
        
        if (_states.LookRight) {
            if (_states.RightDouble) {
                if (_states.OnGround) {
                    Animator.SetBool("Run", true);
                } else {
                    if (!Animator.GetBool(AnimatorBool.IS_SPURTING) && _states.CanSpurtOrRetreatOnAir) {
                        Animator.SetTrigger("SpurtOnAir");
                    }
                }
            } else {
                Animator.SetBool("Run", false);
                Animator.SetBool("WalkForward", _states.Right);
            }

            if (_states.LeftDouble) {
                if (!Animator.GetBool(AnimatorBool.IS_RETREATING) &&
                    (_states.OnGround || _states.CanSpurtOrRetreatOnAir)) {
                    Animator.SetTrigger("Retreat");
                }
            } else {
                Animator.SetBool("WalkBack", _states.Left);
            }
        } else {
            if (_states.LeftDouble) {
                if (_states.OnGround) {
                    Animator.SetBool("Run", true);
                } else {
                    if (!Animator.GetBool(AnimatorBool.IS_SPURTING) && _states.CanSpurtOrRetreatOnAir) {
                        Animator.SetTrigger("SpurtOnAir");
                    }
                }
            } else {
                Animator.SetBool("Run", false);
                Animator.SetBool("WalkForward", _states.Left);
            }

            if (_states.RightDouble) {
                if (!Animator.GetBool(AnimatorBool.IS_RETREATING) &&
                    (_states.OnGround || _states.CanSpurtOrRetreatOnAir)) {
                    Animator.SetTrigger("Retreat");
                }
            } else {
                Animator.SetBool("WalkBack", _states.Right);
            }
        }

        HandleAttacks();
    }

    private void HandleAttacks() {
        foreach (var attack in _states.Attacks) {
            Animator.SetBool(attack.AttackAnimName, attack.Attack);
        }
    }

    private void JumpAnim() {
        Animator.ResetTrigger("SpurtOnAir");
        Animator.ResetTrigger("Retreat");
        Animator.SetBool("Jump", true);
    }

    private void JumpDoubleAnim() {
        Animator.ResetTrigger("SpurtOnAir");
        Animator.ResetTrigger("Retreat");
        Animator.SetTrigger("JumpDouble");
    }

    private void JumpHighAnim() {
        Animator.ResetTrigger("SpurtOnAir");
        Animator.ResetTrigger("Retreat");
        Animator.SetBool("Jump", true);
        Animator.SetBool("JumpHigh", true);
    }

    public void CloseJumpAnim() {
        Animator.SetBool("Jump", false);
        Animator.SetBool("JumpHigh", false);
        Animator.SetBool("HurtOnAir", false);
        Animator.ResetTrigger("JumpDouble");
        Animator.ResetTrigger("SpurtOnAir");
        Animator.ResetTrigger("Retreat");
    }

    private IEnumerator CloseBoolInAnim(string animName, float time) {
        yield return new WaitForSeconds(time);

        Animator.SetBool(animName, false);
    }
}