using UnityEngine;
using System.Collections;

public class AnimationHandler : MonoBehaviour {
    public Animator Animator;

    public float AttackRate = .3f;
    private AttacksBase[] _attacks = new AttacksBase[4];
    private PlayerStateManager _states;

    public void Start() {
        _states = GetComponent<PlayerStateManager>();
        Animator = GetComponentInChildren<Animator>();
    }

    public void FixedUpdate() {

        //Animator.SetBool("TakesHit", _states.IsGettingHit);
        Animator.SetBool("OnAir", !_states.OnGround);
        Animator.SetBool("Crouch", _states.Crouch);

        float movement = 0;
        if (_states.LookRight) {
            if (_states.Right)
                movement = 1;
            else if (_states.Left)
                movement = -1;
        }
        else {
            if (_states.Right)
                movement = -1;
            else if (_states.Left)
                movement = 1;
        }

        Animator.SetFloat("Movement", movement);

        /* if(states.vertical < 0)
        {
            states.crouch = true;
        }
        else
        {
            states.crouch = false;
        } */

        HandleAttacks();
    }

    private void HandleAttacks() {
        if (_states.Attackable) {            

        }
    }

    private void Attacks(string attackName, bool attack, int index) {
        if (attack) {
            _attacks[index].Attack = true;
            _attacks[index].AttackTimer = 0;
            _attacks[index].TimesPressed++;
        }

        if (_attacks[index].Attack) {
            _attacks[index].AttackTimer += Time.deltaTime;

            if (_attacks[index].AttackTimer > AttackRate || _attacks[index].TimesPressed >= 3) {
                _attacks[index].AttackTimer = 0;
                _attacks[index].Attack = false;
                _attacks[index].TimesPressed = 0;
            }
        }

        Animator.SetBool(attackName, _attacks[index].Attack);
    }

    public void JumpAnim() {
        Animator.SetBool("Attack1", false);
        Animator.SetBool("Attack2", false);
        Animator.SetBool("Jump", true);
        StartCoroutine(CloseBoolInAnim("Jump"));
    }

    private IEnumerator CloseBoolInAnim(string animName) {
        yield return new WaitForSeconds(0.5f);
        Animator.SetBool(animName, false);
    }
}