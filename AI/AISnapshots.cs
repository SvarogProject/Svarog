using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AISnapshots : MonoBehaviour {
    public List<AIState> AiStates = new List<AIState>();

    #region Singleton
    private static AISnapshots _instance;

    public static AISnapshots GetInstance() {
        return _instance;
    }

    public void Awake() {
        _instance = this;
    }
    #endregion
    
    public void Start() {
        if (AiStates.Count < 1) {
            AiStates.Add(new AIState());
        }
    }

    public void RequestAISnapshot(AICharacter character) {
        var index = GameSceneManager.GetInstance().NextProgressionIndex;

        if (index > AiStates.Count - 1) {
            index = AiStates.Count - 1;
        }

        SetAIStates(index, character);
    }

    private void SetAIStates(int index, AICharacter character) {
        var a = AiStates[index];
        character.changeStateTolerance = Random.Range(a.ChangeStateToleranceMin, a.ChangeStateToleranceMax);
        character.normalRate = Random.Range(a.NormalRateMin, a.NormalRateMax);
        character.closeRate = Random.Range(a.CloseRateMin, a.CloseRateMax);
        character.blockingRate = Random.Range(a.BlockingRateMin, a.BlockingRateMax);
        character.aiStateLife = Random.Range(a.AiStateLifeMin, a.AiStateLifeMax);
        character.JumpRate = Random.Range(a.JumpRateMin, a.JumpRateMax);
    }
}