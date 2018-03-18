using UnityEngine;
using System.Collections;

public class DoDamage : MonoBehaviour {
    public DamageColliderHandler.DamageType DamageType;
    
    private PlayerStateManager _states;

    void Start() {
        _states = GetComponentInParent<PlayerStateManager>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponentInParent<PlayerStateManager>()) {
            var otherState = other.GetComponentInParent<PlayerStateManager>();

            if (otherState != _states) {
                // if (!oState.CurrentlyAttacking) {
                otherState.TakeDamage(5, DamageType);
                // }
            }
        }
    }
}