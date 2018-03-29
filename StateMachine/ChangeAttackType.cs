
using UnityEngine;

public class ChangeAttackType : StateMachineBehaviour {
    public DamageType DamageType;
    public bool ResetOnExit;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.gameObject.GetComponentInChildren<DamageHandler>().DamageType = DamageType;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (ResetOnExit)
            animator.GetComponentInChildren<DamageHandler>().DamageType = DamageType.Light;
    }
}
