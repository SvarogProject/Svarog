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

        if (_states.LookRight) {
            if (_states.RightDouble) {
                Animator.SetBool("Run", true);
            } else {
                Animator.SetBool("Run", false);
                Animator.SetBool("WalkForward", _states.Right);
            }

            if (_states.LeftDouble) {
                if (!Animator.GetBool(AnimatorBool.IS_RETREATING)) {
                    if (_states.OnGround) {
                        Animator.SetTrigger("Retreat");
                    } else {
                        // TODO 空中后跳
                        
                    }                   
                }    
            } else {
                Animator.SetBool("WalkBack", _states.Left);
            }
        } else {
            if (_states.LeftDouble) {
                Animator.SetBool("Run", true);
            } else {
                Animator.SetBool("Run", false);
                Animator.SetBool("WalkForward", _states.Left);
            }

            if (_states.RightDouble) {
                if (!Animator.GetBool(AnimatorBool.IS_RETREATING)) {
                    if (_states.OnGround) {
                        Animator.SetTrigger("Retreat");
                    } else {
                        // TODO 空中后跳
                    }                   
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

    public void CloseJumpAnim() {
        Animator.SetBool("Jump", false);
    }

    private IEnumerator CloseBoolInAnim(string animName, float time) {
        yield return new WaitForSeconds(time);

        Animator.SetBool(animName, false);
    }
}