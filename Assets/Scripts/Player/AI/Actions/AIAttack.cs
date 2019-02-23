using BehaviorDesigner.Runtime.Tasks;
using Action = BehaviorDesigner.Runtime.Tasks.Action;

public class AIAttack : Action {

    public AIAttackType AttackType;
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

        if (!_states.Attackable) {
            return TaskStatus.Failure;
        }

        _states.Attacks[0].Attack = AttackType == AIAttackType.P;
        _states.Attacks[1].Attack = AttackType == AIAttackType.K;
        _states.Attacks[2].Attack = AttackType == AIAttackType.S;
        _states.Attacks[3].Attack = AttackType == AIAttackType.HS;

        if (AttackType == AIAttackType.Clear) {
            foreach (var a in _states.Attacks) {
                a.Attack = false;
            }
        }
        return TaskStatus.Success;
    }
   
}

public enum AIAttackType {
    P,
    K,
    S,
    HS,
    Clear
}