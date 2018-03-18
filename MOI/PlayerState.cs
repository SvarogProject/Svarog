
public class PlayerState
{
    public bool IsMoveable = true;
    public bool IsJumpping;
    public bool IsJumpForwarding;
    public bool IsJumpBacking;
    public bool IsForwarding;
    public bool IsBacking;
    public bool usedDoubleJump;
    public bool canDoubleJump;
    public float SpurtOnAirTime = -1;
    public float RetreatOnAirTime = -1;
    public float RetreatTime = -1;

    public bool IsSpurtingOnAir()
    {
        return SpurtOnAirTime >= 0 && SpurtOnAirTime <= 0.3f;
    }

    public bool IsSpurtOnAirEnding()
    {
        return SpurtOnAirTime > 0.3f;
    }

    public bool IsRetreatingOnAir()
    {
        return RetreatOnAirTime >= 0 && RetreatOnAirTime <= 0.3f;
    }

    public bool IsRetreatOnAirEnding()
    {
        return RetreatOnAirTime > 0.3f;
    }

    public bool IsRetreating()
    {
        return RetreatTime >= 0 && RetreatTime <= 0.3f;
    }

    public void CleanStates()
    {
        RetreatTime = -1;
        RetreatOnAirTime = -1;
        SpurtOnAirTime = -1;
        IsJumpping = false;
        IsJumpForwarding = false;
        IsJumpBacking = false;
        usedDoubleJump = false;
        canDoubleJump = false;
    }
}
