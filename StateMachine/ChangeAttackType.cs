
using UnityEngine;

public class ChangeAttackType : StateMachineBehaviour {
    public DamageHandler DamageHandler;
    public DamageType DamageType;
    public bool ResetOnExit;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        DamageHandler.DamageType = DamageType;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (ResetOnExit)
            DamageHandler.DamageType = DamageType.Light;
    }
}
