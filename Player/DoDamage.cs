using UnityEngine;
using System.Collections;

public class DoDamage : MonoBehaviour {
    public DamageType DamageType;
    
    private PlayerStateManager _states;
    private PlayerAnimationHandler _animation;

    void Start() {
        _states = GetComponentInParent<PlayerStateManager>();
        _animation = GetComponentInParent<PlayerAnimationHandler>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponentInParent<PlayerStateManager>() && other.CompareTag("HurtCollider")) {
            var otherState = other.GetComponentInParent<PlayerStateManager>();

            if (otherState != _states) {
                if (_animation.Animator.GetBool(AnimatorBool.IS_FIRE_PUNCH)) {
                    otherState.TakeDamage(10, DamageType.FirePunch);
                } else {
                    otherState.TakeDamage(5, DamageType);
                }
            }
        }
    }
}