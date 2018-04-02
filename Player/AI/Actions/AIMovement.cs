using System;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using Action = BehaviorDesigner.Runtime.Tasks.Action;

public class AIMovement : Action {

    public AIMovementType AiMovementType;
    private PlayerStateManager _enemyStates;
    private PlayerStateManager _states;
    
    public override void OnStart() {
        _enemyStates = GetComponent<AIParams>().EnemyStates;
        _states = GetComponent<PlayerStateManager>();
    }

    public override TaskStatus OnUpdate() {

        if (!_enemyStates || !_states) {
            return TaskStatus.Failure;
        }

        switch (AiMovementType) {

            case AIMovementType.Run:

                if (!_states.AnimationHandler.Animator.GetBool(AnimatorBool.MOVEABLE)) {
                    return TaskStatus.Failure;
                }

                if (_states.LookRight) {
                    _states.RightDouble = true;
                    _states.LeftDouble = false;
                    _states.Right = true;
                    _states.Left = false;
                    _states.DefenseLeft = false;
                    _states.DefenseRight = false;
                } else {
                    _states.LeftDouble = true;
                    _states.RightDouble = false;
                    _states.Right = false;
                    _states.Left = true;
                    _states.DefenseLeft = false;
                    _states.DefenseRight = false;
                }

                break;
            case AIMovementType.Walk:
                
                
                if (!_states.AnimationHandler.Animator.GetBool(AnimatorBool.MOVEABLE)) {
                    return TaskStatus.Failure;
                }
                
                if (_states.LookRight) {
                    _states.RightDouble = false;
                    _states.LeftDouble = false;
                    _states.Right = true;
                    _states.Left = false;
                    _states.DefenseLeft = false;
                    _states.DefenseRight = false;
                } else {
                    _states.LeftDouble = false;
                    _states.RightDouble = false;
                    _states.Right = false;
                    _states.Left = true;
                    _states.DefenseLeft = false;
                    _states.DefenseRight = false;
                }

                break;
            case AIMovementType.Idle:
                
                if (_states.LookRight) {
                    _states.RightDouble = false;
                    _states.LeftDouble = false;
                    _states.Right = false;
                    _states.Left = false;
                    _states.DefenseLeft = false;
                    _states.DefenseRight = false;
                } else {
                    _states.LeftDouble = false;
                    _states.RightDouble = false;
                    _states.Right = false;
                    _states.Left = false;
                    _states.DefenseLeft = false;
                    _states.DefenseRight = false;
                }

                break;
            case AIMovementType.Away:
                
                
                if (!_states.AnimationHandler.Animator.GetBool(AnimatorBool.MOVEABLE)) {
                    return TaskStatus.Failure;
                }
                
                if (_states.LookRight) {
                    _states.RightDouble = false;
                    _states.LeftDouble = false;
                    _states.Right = false;
                    _states.Left = true;
                    _states.DefenseLeft = true;
                    _states.DefenseRight = false;
                } else {
                    _states.LeftDouble = false;
                    _states.RightDouble = false;
                    _states.Right = true;
                    _states.Left = false;
                    _states.DefenseLeft = false;
                    _states.DefenseRight = true;
                }
                break;
                
            case AIMovementType.Jump:

                if (!_states.AnimationHandler.Animator.GetBool(AnimatorBool.JUMPABLE)) {
                    return TaskStatus.Failure;
                }

                _states.Jump = true;
                StartCoroutine(CloseJump(0.2f));
                _states.LeftDouble = false;
                _states.RightDouble = false;
                _states.DefenseLeft = false;
                _states.DefenseRight = false;
                break;
                
            case AIMovementType.Crouch:

                if (_states.OnGround) {
                    _states.Crouch = true;
                    StartCoroutine(CloseCrouch(1));
                    _states.LeftDouble = false;
                    _states.RightDouble = false;
                    _states.DefenseLeft = false;
                    _states.DefenseRight = false;
                } else {
                    _states.Crouch = false;
                }
                
                break;
            default:

                throw new ArgumentOutOfRangeException();
        }
        return TaskStatus.Success;


    }

    public override void OnBehaviorComplete() {
        
        if (!_enemyStates || !_states) {
            return;
        }
        
        _states.LeftDouble = false;
        _states.RightDouble = false;
        _states.Right = false;
        _states.Left = false;
        _states.DefenseLeft = false;
        _states.DefenseRight = false;
    }

    private IEnumerator CloseJump(float time) {
        yield return new WaitForSeconds(time);

        _states.Jump = false;
    }
    
    private IEnumerator CloseCrouch(float time) {
        yield return new WaitForSeconds(time);

        _states.Crouch = false;
    }
}

public enum AIMovementType {
    Run,
    Walk,
    Idle,
    Away,
    Jump,
    Crouch
}