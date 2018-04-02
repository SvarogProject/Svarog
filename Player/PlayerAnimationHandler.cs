using UnityEngine;
using System.Collections;

public class PlayerAnimationHandler : MonoBehaviour {
    public Animator Animator;

    private PlayerStateManager _states;

    private float _defenseTimer;

    public void Start() {
        _states = GetComponent<PlayerStateManager>();
    }

    public void FixedUpdate() {
        Animator.SetBool("Dead", _states.Dead);
        HandleHurt();     
        Animator.SetBool("Crouch", _states.Crouch);
        HandleJump();
        HandleMove();     
        HandleAttacks();
    }

    private void HandleMove() {
        if (_states.LookRight) {
            DoMoveForward(_states.RightDouble, _states.Right);
            DoMoveBack(_states.LeftDouble, _states.Left);       
        } else {
            DoMoveForward(_states.LeftDouble, _states.Left);
            DoMoveBack(_states.RightDouble, _states.Right);
        }
    }

    private void HandleJump() {
        if (_states.Jump) {
            DoJumpAnim();
        }

        if (_states.JumpHigh) {
            DoJumpHighAnim();
        }

        if (_states.JumpDouble) {
            DoJumpDoubleAnim();
        }
    }

    private void HandleHurt() {
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

        if (_states.IsGettingHurtDefense) {
            Animator.SetBool("Defense", true);
            _defenseTimer = 0.2f;
        } else {
            if (_defenseTimer < 0) {
                Animator.SetBool("Defense", false);
            } else {
                _defenseTimer -= Time.deltaTime;
            }
            
        }
    }

    private void HandleAttacks() {
        foreach (var attack in _states.Attacks) {
            Animator.SetBool(attack.AttackAnimName, attack.Attack);
        }
    }

    private void DoMoveForward(bool doubleClick, bool forward) {
        if (doubleClick) {
            if (_states.OnGround) {
                Animator.SetBool("Run", true);
            } else {
                if (!Animator.GetBool(AnimatorBool.IS_SPURTING) && _states.CanSpurtOrRetreatOnAir) {
                    Animator.SetTrigger("SpurtOnAir");
                }
            }
        } else {
            Animator.SetBool("Run", false);
            Animator.SetBool("WalkForward", forward);
        }      
    }

    private void DoMoveBack(bool doubleClick, bool back) {
        if (doubleClick) {
            if (!Animator.GetBool(AnimatorBool.IS_RETREATING) &&
                (_states.OnGround || _states.CanSpurtOrRetreatOnAir)) {
                Animator.SetTrigger("Retreat");
            }
        } else {
            Animator.SetBool("WalkBack", back);
        }
    }

    private void DoJumpAnim() {
        Animator.ResetTrigger("SpurtOnAir");
        Animator.ResetTrigger("Retreat");
        Animator.SetBool("Defense", false);
        Animator.SetBool("Jump", true);
        HandleLookBack();
    }

    private void DoJumpDoubleAnim() {
        Animator.ResetTrigger("SpurtOnAir");
        Animator.ResetTrigger("Retreat");
        Animator.SetBool("Defense", false);
        Animator.SetTrigger("JumpDouble");
        HandleLookBack();
    }

    private void DoJumpHighAnim() {
        Animator.ResetTrigger("SpurtOnAir");
        Animator.ResetTrigger("Retreat");
        Animator.SetBool("Defense", false);
        Animator.SetBool("Jump", true);
        Animator.SetBool("JumpHigh", true);
        HandleLookBack();
    }

    public void CloseJumpAnim() {
        Animator.SetBool("Jump", false);
        Animator.SetBool("JumpHigh", false);
        Animator.SetBool("HurtOnAir", false);
        Animator.SetBool("Defense", false);
        Animator.ResetTrigger("JumpDouble");
        Animator.ResetTrigger("SpurtOnAir");
        Animator.ResetTrigger("Retreat");
        HandleLookBack();
    }


    private IEnumerator CloseBoolInAnim(string animName, float time) {
        yield return new WaitForSeconds(time);

        Animator.SetBool(animName, false);
    }
    
    private void HandleLookBack() {
        if (_states.ShouldLookBack) {
            _states.LookRight = !_states.LookRight;
            _states.ShouldLookBack = false;
        }
    }

    public void Stop(float time) {
        Animator.speed = 0f;
        //Invoke("AnimPlay", time);
        StartCoroutine(AnimPlay(time));
    }
    
    private IEnumerator AnimPlay(float time) {
        yield return new WaitForSeconds(time);

        Animator.speed = 1;
    }
    
    private void AnimPlay() {
        Animator.speed = 1;
        
    }
}