
using UnityEngine;

public class KeepBool : StateMachineBehaviour {
    public string BoolName;
    public bool Status;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool(BoolName, Status);   
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool(BoolName, Status);
    }
}
