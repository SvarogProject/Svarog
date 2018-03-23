
using UnityEngine;

public class ChangeAttackType : StateMachineBehaviour {
    public DoDamage DoDamage;
    public DamageType DamageType;
    public bool ResetOnExit;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        DoDamage.DamageType = DamageType;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (ResetOnExit)
            DoDamage.DamageType = DamageType.Light;
    }
}
