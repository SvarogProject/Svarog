using UnityEngine;
using System.Collections;

public class ChangeBool : StateMachineBehaviour {
    public string BoolName;
    public bool Status;
    public bool ResetOnExit;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool(BoolName, Status);   
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (ResetOnExit)
            animator.SetBool(BoolName, !Status);
    }

}