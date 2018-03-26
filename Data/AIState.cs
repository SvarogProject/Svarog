[System.Serializable]
public class AIState {
    public float ChangeStateToleranceMax = 3;
    public float ChangeStateToleranceMin = 2;
    public float NormalRateMax = 1;
    public float NormalRateMin = 0.8f;
    public float CloseRateMax = 0.5f;
    public float CloseRateMin = 0.4f;
    public float BlockingRateMax = 1.5f;
    public float BlockingRateMin = 1.5f;
    public float AiStateLifeMax = 1;
    public float AiStateLifeMin = 1;
    public float JumpRateMax = 1;
    public float JumpRateMin = 1;
}