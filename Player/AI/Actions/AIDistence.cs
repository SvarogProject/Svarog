using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class AIDistence : Action {

    public float min;
    public float max;
    
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


        if (Mathf.Abs(_enemyStates.transform.position.x - _states.transform.position.x) > min
            && Mathf.Abs(_enemyStates.transform.position.x - _states.transform.position.x) <= max) {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;


    }
}