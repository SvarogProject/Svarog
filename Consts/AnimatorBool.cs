public class AnimatorBool {
    public const string JUMPABLE = "Jumpable";
    public const string MOVEABLE = "Moveable";
    public const string IS_RUN_ENDING = "IsRunEnding";
    public const string IS_RUNNING = "IsRunning";
    public const string IS_RETREATING = "IsRetreating";
    public const string IS_SPURTING = "IsSpurting";
    public const string USED_JUMP_DOUBLE = "UsedJumpDouble"; // 当前状态已经使用了二段跳
    public const string JUMP = "Jump";
    public const string CROUCH = "Crouch";
    public const string HIGH_JUMPABLE = "HighJumpable";
    public const string IS_FIRE_PUNCH = "IsFirePunch";
    public const string CAN_LOOK_BACK = "CanLookBack"; // 当前状态可以立即回头
    public const string ATTACK_HS = "AttackHS";
    public const string IS_HEAVY_ATTACK = "IsHeavyAttack";
    public const string ATTACKED = "Attacked"; // 当前攻击状态中角色攻击到了敌人，攻击结束后会重置，为了取消后摇
    
    // Effects
    public const string ATTACK_SMALL_EFFECT = "AttackSmallEffect";
    public const string ATTACK_KNIFE_EFFECT = "AttackKnifeEffect";
    public const string ATTACK_CENTER_DOWM_EFFECT = "AttackCenterDownEffect";
}