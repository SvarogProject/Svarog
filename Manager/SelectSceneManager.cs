using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SelectSceneManager : MonoBehaviour
{
    public int numberOfPlayers = 1;
    public List<PlayerInterfaces> plInterfaces = new List<PlayerInterfaces>();
    int maxRow;
    int maxCollumn;
    List<PotraitInfo> potraitList = new List<PotraitInfo>();

    public GameObject potraitCanvas; // the canvas that holds all the potraits
   
    bool loadLevel; //if we are loading the level  
    public bool bothPlayersSelected;

    CharacterManager charManager;

    GameObject potraitPrefab;

    #region Singleton
    public static SelectSceneManager instance;
    public static SelectSceneManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }
    #endregion

    void Start()
    {
        //we start by getting the reference to the character manager
        charManager = CharacterManager.GetInstance();
        numberOfPlayers = charManager.NumberOfUsers;

        potraitPrefab = Resources.Load("potraitPrefab") as GameObject;
        CreatePotraits();

        charManager.IsSolo = (numberOfPlayers == 1);

    }

    void CreatePotraits()
    {
        GridLayoutGroup group = potraitCanvas.GetComponent<GridLayoutGroup>();

        maxRow = group.constraintCount;
        int x = 0;
        int y = 0;

        for (int i = 0; i < charManager.CharacterList.Count; i++)
        {
            CharacterBase c = charManager.CharacterList[i];

            GameObject go = Instantiate(potraitPrefab) as GameObject;
            go.transform.SetParent(potraitCanvas.transform);

            PotraitInfo p = go.GetComponent<PotraitInfo>();
            p.Img.sprite = c.Icon;
            p.CharacterId = c.CharacterId;
            p.PositionX = x;
            p.PositionY = y;
            potraitList.Add(p);

            if(x < maxRow-1)
            {
                x++;
            }
            else
            {
                x=0;
                y++;
            }

            maxCollumn = y;
        }
    }

    void Update()
    {
        if (!loadLevel)
        {
            for (int i = 0; i < plInterfaces.Count; i++)
            {
                if (i < numberOfPlayers)
                {
                    if (Input.GetButtonUp("Fire2" + charManager.Players[i].InputId))
                    {
                        plInterfaces[i].playerBase.HasCharacter = false;
                    }

                    if (!charManager.Players[i].HasCharacter)
                    {
                        plInterfaces[i].playerBase = charManager.Players[i];

                        HandleSelectorPosition(plInterfaces[i]);
                        HandleSelectScreenInput(plInterfaces[i], charManager.Players[i].InputId);
                        HandleCharacterPreview(plInterfaces[i]);
                    }
                }
                else
                {
                    charManager.Players[i].HasCharacter = true;
                }
            }
           
        }

        if(bothPlayersSelected)
        {
            Debug.Log("loading");
            StartCoroutine("LoadLevel"); //and start the coroutine to load the level
            loadLevel = true;
            bothPlayersSelected = false;
        }
        else
        {
            if(charManager.Players[0].HasCharacter 
                && charManager.Players[1].HasCharacter)
            {
                bothPlayersSelected = true;
            }
           
        }
    }
  
    void HandleSelectScreenInput(PlayerInterfaces pl, string playerId)
    {
        #region Grid Navigation

        /*To navigate in the grid
         * we simply change the active x and y to select what entry is active
         * we also smooth out the input so if the user keeps pressing the button
         * it won't switch more than once over half a second
         */

        float vertical = Input.GetAxis("Vertical" + playerId);

        if (vertical != 0)
        {
            if (!pl.hitInputOnce)
            {
                if (vertical > 0)
                {
                    pl.activeY = (pl.activeY > 0) ? pl.activeY - 1 : maxCollumn;
                }
                else
                {
                    pl.activeY = (pl.activeY < maxCollumn) ? pl.activeY + 1 : 0;
                }

                pl.hitInputOnce = true;
            }
        }

        float horizontal = Input.GetAxis("Horizontal" + playerId);

        if (horizontal != 0)
        {
            if (!pl.hitInputOnce)
            {
                if (horizontal > 0)
                {
                    pl.activeX = (pl.activeX > 0) ? pl.activeX - 1 : maxRow-1;
                }
                else
                {
                    pl.activeX = (pl.activeX < maxRow-1) ? pl.activeX + 1 : 0;
                }

                pl.timerToReset = 0;
                pl.hitInputOnce = true;
            }
        }
       
        if(vertical == 0 && horizontal == 0)
        {
            pl.hitInputOnce = false;
        }

        if (pl.hitInputOnce)
        {
            pl.timerToReset += Time.deltaTime;

            if (pl.timerToReset > 0.8f)
            {
                pl.hitInputOnce = false;
                pl.timerToReset = 0;
            }
        }

        #endregion

        //if the user presses space, he has selected a character
        if (Input.GetButtonUp("Fire1" + playerId))
        {
            //make a reaction on the character, because why not
            pl.createdCharacter.GetComponentInChildren<Animator>().Play("Kick");

            //pass the character to the character manager so that we know what prefab to create in the level
            pl.playerBase.PlayerPrefab =
               charManager.GetCharacterById(pl.activePotrait.CharacterId).Prefab;

            pl.playerBase.HasCharacter = true;
        }
    }

    IEnumerator LoadLevel()
    {
        //if any of the players is an AI, then assign a random character to the prefab
        for (int i = 0; i < charManager.Players.Count; i++)
        {
            if(charManager.Players[i].Type == PlayerBase.PlayerType.Ai)
            {
                if(charManager.Players[i].PlayerPrefab == null)
                {
                    int ranValue = Random.Range(0, potraitList.Count);

                    charManager.Players[i].PlayerPrefab = 
                        charManager.GetCharacterById(potraitList[ranValue].CharacterId).Prefab;

                    Debug.Log(potraitList[ranValue].CharacterId);
                }
            }
        }

        yield return new WaitForSeconds(2);//after 2 seconds load the level

        if (charManager.IsSolo)
        {
            GameSceneManager.GetInstance().CreateProgression();
            GameSceneManager.GetInstance().LoadNextOnProgression();
        }
        else
        {
            GameSceneManager.GetInstance().RequestLevelLoad(SceneType.prog, "Level1");
        }

    }

    void HandleSelectorPosition(PlayerInterfaces pl)
    {
        pl.selector.SetActive(true); //enable the selector

        PotraitInfo pi = ReturnPotrait(pl.activeX, pl.activeY);//

        if (pi != null)
        {
            pl.activePotrait = pi; //find the active potrait

            //and place the selector over it's position
            Vector2 selectorPosition = pl.activePotrait.transform.localPosition;

            selectorPosition = selectorPosition + new Vector2(potraitCanvas.transform.localPosition.x
                , potraitCanvas.transform.localPosition.y);

            pl.selector.transform.localPosition = selectorPosition;
        }
    }

    void HandleCharacterPreview(PlayerInterfaces pl)
    {
        //if the previews potrait we had, is not the same as the active one we have
        //that means we changed characters
        if (pl.previewPotrait != pl.activePotrait)
        {
            if (pl.createdCharacter != null)//delete the one we have now if we do have one
            {
                Destroy(pl.createdCharacter);
            }

            //and create another one
            GameObject go = Instantiate(
                CharacterManager.GetInstance().GetCharacterById(pl.activePotrait.CharacterId).Prefab,
                pl.charVisPos.position,
                Quaternion.identity) as GameObject;

            pl.createdCharacter = go;

            pl.previewPotrait = pl.activePotrait;

            if(!string.Equals(pl.playerBase.PlayerId, charManager.Players[0].PlayerId))
            {
                pl.createdCharacter.GetComponent<PlayerStateManager>().LookRight = false;
            }
        }
    }


    PotraitInfo ReturnPotrait(int x, int y)
    {
        PotraitInfo r = null;
        for (int i = 0; i < potraitList.Count; i++)
        {
            if(potraitList[i].PositionX == x && potraitList[i].PositionY == y)
            {
                r = potraitList[i];
            }
        }

        return r;
    }

    [System.Serializable]
    public class PlayerInterfaces
    {
        public PotraitInfo activePotrait; //the current active potrait for player 1
        public PotraitInfo previewPotrait; 
        public GameObject selector; //the select indicator for player1
        public Transform charVisPos; //the visualization position for player 1
        public GameObject createdCharacter; // the created character for player 1

        public int activeX;//the active X and Y entries for player 1
        public int activeY;

        //variables for smoothing out input
        public bool hitInputOnce;
        public float timerToReset;

        public PlayerBase playerBase;

    }

}
