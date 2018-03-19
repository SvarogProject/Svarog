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

        //Animator.SetBool("TakesHit", _states.IsGettingHit);
        //Animator.SetBool("OnAir", !_states.OnGround);
        Animator.SetBool("Crouch", _states.Crouch);

        if (_states.Jump) {
            Animator.SetBool("Jump", true);
        }

        if (_states.JumpHigh) {
            Animator.SetBool("Jump", true);
            Animator.SetBool("JumpHigh", true);
        }

        if (_states.JumpDouble) {
            Animator.SetTrigger("JumpDouble");
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

    public void JumpAnim() {
        //Animator.SetBool("Attack1", false);
        //Animator.SetBool("Attack2", false);
        Animator.SetBool("Jump", true);
        //StartCoroutine(CloseBoolInAnim("Jump"));
    }

    public void JumpDoubleAnim() {
        Animator.SetTrigger("JumpDouble");
    }

    public void JumpHighAnim() {
        Animator.SetBool("Jump", true);
        Animator.SetBool("JumpHigh", true);
    }

    public void CloseJumpAnim() {
        Animator.SetBool("Jump", false);
        Animator.SetBool("JumpHigh", false);
        Animator.ResetTrigger("JumpDouble");
    }

    private IEnumerator CloseBoolInAnim(string animName, float time) {
        yield return new WaitForSeconds(time);

        Animator.SetBool(animName, false);
    }
}