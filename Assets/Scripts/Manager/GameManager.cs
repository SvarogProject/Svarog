using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {
    
    public bool IsSolo;
    public int NumberOfUsers;
    public List<PlayerBase> Players = new List<PlayerBase>();
    public List<CharacterBase> CharacterList = new List<CharacterBase>();

    #region Singleton
    private static GameManager _instance;

    public static GameManager GetInstance() {
        return _instance;
    }

    public void Awake() {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    public CharacterBase GetCharacterByPrefab(GameObject prefab) {
        CharacterBase retVal = null;

        foreach (var character in CharacterList) {
            if (character.Prefab != prefab) continue;
            retVal = character;
            break;
        }

        return retVal;
    }

    public CharacterBase GetCharacterById(string id) {
        CharacterBase retVal = null;

        foreach (var character in CharacterList) {
            if (!string.Equals(character.CharacterId, id)) continue;
            retVal = character;
            break;
        }

        return retVal;
    }

    public PlayerBase GetPlayerByStates(PlayerStateManager states) {
        PlayerBase retVal = null;

        foreach (var player in Players) {
            if (player.PlayerStates != states) continue;
            retVal = player;
            break;
        }

        return retVal;
    }

    public PlayerBase GetOppositePlayer(PlayerBase pl) {
        return Players.FirstOrDefault(t => t != pl);
    }

    public int GetCharacterIndex(GameObject prefab) {
        var retVal = 0;

        for (var i = 0; i < CharacterList.Count; i++) {
            if (CharacterList[i].Prefab != prefab) continue;
            retVal = i;
            break;
        }

        return retVal;
    }
}