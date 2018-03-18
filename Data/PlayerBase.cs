using UnityEngine;

[System.Serializable]
public class PlayerBase {
    public string PlayerId;
    public string InputId;
    public PlayerType Type;
    public bool HasCharacter;
    public GameObject PlayerPrefab;
    public PlayerStateManager PlayerStates;
    public int Score;

    public enum PlayerType {
        User,       //it's a real human
        Ai,         //skynet basically
        Simulation, //for multiplayer over network, no, that's not a promise..
        None
    }
}