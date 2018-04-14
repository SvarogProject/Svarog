
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class NetAnimationHandler : NetworkBehaviour {
    public Animator Animator;

    private NetPlayerStateManager _states;

    private float _defenseTimer;

    public void Start() {
        _states = GetComponent<NetPlayerStateManager>();
    }

    public void FixedUpdate() {       
        CmdHandleDead();
        CmdHandleHurt();
        CmdHandleCrouch();
        CmdHandleJump();
        CmdHandleMove();
        CmdHandleAttacks();

    }

    //[Command]
    private void CmdHandleCrouch() {
        Animator.SetBool("Crouch", _states.Crouch);
    }

    //[Command]
    private void CmdHandleDead() {
        Animator.SetBool("Dead", _states.Dead);
    }

    //[Command]
    private void CmdHandleMove() {
        if (_states.LookRight) {
            CmdDoMoveForward(_states.RightDouble, _states.Right);
            CmdDoMoveBack(_states.LeftDouble, _states.Left);       
        } else {
            CmdDoMoveForward(_states.LeftDouble, _states.Left);
            CmdDoMoveBack(_states.RightDouble, _states.Right);
        }
    }

    //[Command]
    private void CmdHandleJump() {
        if (_states.Jump) {
            CmdDoJumpAnim();
        }

        if (_states.JumpHigh) {
            CmdDoJumpHighAnim();
        }

        if (_states.JumpDouble) {
            CmdDoJumpDoubleAnim();
        }
    }

    //[Command]
    private void CmdHandleHurt() {
        if (_states.IsGettingHurtSmall) {
            Animator.SetTrigger("HurtSmall");
        }

        if (_states.IsGettingHurtLarge) {
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

    //[Command]
    private void CmdHandleAttacks() {
        foreach (var attack in _states.Attacks) {
            Animator.SetBool(attack.AttackAnimName, attack.Attack);
        }
    }

    //[Command]
    private void CmdDoMoveForward(bool doubleClick, bool forward) {
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

    //[Command]
    private void CmdDoMoveBack(bool doubleClick, bool back) {
        if (doubleClick) {
            if (!Animator.GetBool(AnimatorBool.IS_RETREATING) &&
                (_states.OnGround || _states.CanSpurtOrRetreatOnAir)) {
                Animator.SetTrigger("Retreat");
            }
        } else {
            Animator.SetBool("WalkBack", back);
        }
    }

    //[Command]
    private void CmdDoJumpAnim() {
        Animator.ResetTrigger("SpurtOnAir");
        Animator.ResetTrigger("Retreat");
        Animator.SetBool("Defense", false);
        Animator.SetBool("Jump", true);
        HandleLookBack();
    }

    //[Command]
    private void CmdDoJumpDoubleAnim() {
        Animator.ResetTrigger("SpurtOnAir");
        Animator.ResetTrigger("Retreat");
        Animator.SetBool("Defense", false);
        Animator.SetTrigger("JumpDouble");
        HandleLookBack();
    }

    //[Command]
    private void CmdDoJumpHighAnim() {
        Animator.ResetTrigger("SpurtOnAir");
        Animator.ResetTrigger("Retreat");
        Animator.SetBool("Defense", false);
        Animator.SetBool("Jump", true);
        Animator.SetBool("JumpHigh", true);
        HandleLookBack();
    }

    //[Command]
    public void CmdCloseJumpAnim() {
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
            _states.CmdLookRight(!_states.LookRight);
            _states.CmdShouldLookBack(false);
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
