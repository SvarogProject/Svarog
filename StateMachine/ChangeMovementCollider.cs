using UnityEngine;
using System.Collections;

public class ChangeMovementCollider : StateMachineBehaviour {

    PlayerStateManager states;

    public int index;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (states == null)
            states = animator.transform.GetComponentInParent<PlayerStateManager>();

        states.CloseMovementCollider(index);

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (states == null)
            states = animator.transform.GetComponentInParent<PlayerStateManager>();

        states.OpenMovementCollider(index);
    }

}
