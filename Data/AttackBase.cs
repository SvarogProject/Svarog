[System.Serializable]
public class AttacksBase {
    
    public string AttackAnimName;
    public string AttackButtonName;
    public bool Attack;
    public float AttackTimer;
    public int TimesPressed;
    public float AttackRate;

    public void Reset() {
        Attack = false;
        AttackTimer = 0;
        TimesPressed = 0;
    }

    public void Do() {
        Attack = true;
        AttackTimer = 0;
        TimesPressed++;
    }
}