using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static DeckManager;
using static GameManager;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;

public class Player : NetworkBehaviour
{
    public struct CardSpawnData
    {
        public ulong netId;
        public Transform targetArea;
        public string spawnType;

        public CardSpawnData(ulong id, Transform trans, string type)
        {
            netId = id;
            targetArea = trans;
            spawnType = type;
        }

    }
   
    public GameObject playerAreaPrefab;
    public GameObject enemyAreaLeftPrefab;
    public GameObject enemyAreaRightPrefab;
    public GameObject enemyAreaUpPrefab;
    public GameObject cardPrefab;
    public GameObject arrowPrefab;
    public GameObject ReadyButtonPrefab;

    public GameObject playerArea;
    public GameObject enemyAreaLeft;
    public GameObject enemyAreaRight;
    public GameObject enemyAreaUp;
    public GameObject dropzone;
    public GameObject arrow;


    public bool hasSetAreas = false;

    public NetworkVariable<int> PlayerHandCount = new NetworkVariable<int>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<FixedString64Bytes> playerRank = new NetworkVariable<FixedString64Bytes>("Commoner", readPerm: NetworkVariableReadPermission.Everyone);

    public Queue<CardSpawnData> cardQueue = new Queue<CardSpawnData>(256);
    //private float spawnDelay = 0.01f;
    private float spawnDelay = 0.03f;
    private float spawnTimer = 0f;

    ////This are for the player
    private List<string> selectedTags = new List<string>();
    private List<GameObject> selectedCards = new List<GameObject>();
    public Dictionary<string, ulong> cardNametoId = new Dictionary<string, ulong>();
    private bool has3ofSpades = false;


    //public string playerRank = "Commoner";
    public ulong PlayerId => OwnerClientId;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log($"[OnNetworkSpawn] Player spawned. OwnerClientId={OwnerClientId}, IsOwner={IsOwner}, LocalClientId={NetworkManager.Singleton.LocalClientId}");


        if (IsOwner)
        {
            PlayerHandCount.Value = 0;
        }

        PlayerHandCount.OnValueChanged += (prev, newVal) =>
        {
            GameManager.Instance.UpdateAllEnemiesUI(prev, newVal, PlayerId);
        };


        if (!IsOwner) return;

        ulong playerNumber = OwnerClientId + 1;
        gameObject.name = "Player " + playerNumber;

        SceneManager.sceneLoaded += OnSceneLoaded;


    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        PlayerHandCount.OnValueChanged -= (prev, newVal) =>
        {
            GameManager.Instance.UpdateAllEnemiesUI(prev, newVal, PlayerId);
        };
    }

    // When we get to the Game Scene, set the player area up
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!IsOwner) return;

        if (SceneManager.GetActiveScene().name == "Game Scene")
        {
            GameManager.Instance.RegisterHumanPlayer(this);
            SetupPlayer();
        }

    }
    
    // This is to set up the player's areas and dropzone
    public void SetupPlayer()
    {
        if (hasSetAreas) return;
        Debug.Log("Setting the playerobject");

        playerArea = Instantiate(playerAreaPrefab, GameObject.Find("Main Canvas").transform, false);
        enemyAreaLeft = Instantiate(enemyAreaLeftPrefab, GameObject.Find("Main Canvas").transform, false);
        enemyAreaUp = Instantiate(enemyAreaUpPrefab, GameObject.Find("Main Canvas").transform, false);
        enemyAreaRight = Instantiate(enemyAreaRightPrefab, GameObject.Find("Main Canvas").transform, false);
        dropzone = GameObject.Find("Dropzone");

        arrow = Instantiate(arrowPrefab, GameObject.Find("Main Canvas").transform, false);
        arrow.GetComponent<ArrowScript>().setInital();
        //arrow.GetComponent<ArrowScript>().SetTurn(OwnerClientId);


        if (OwnerClientId == 0)
        {
            playerArea.GetComponent<Image>().color = Color.red;
            enemyAreaLeft.GetComponent<Image>().color = Color.blue;
            enemyAreaUp.GetComponent<Image>().color = Color.blue;
            enemyAreaRight.GetComponent<Image>().color = Color.blue;
        }
        else if (OwnerClientId == 1)
        {
            playerArea.GetComponent<Image>().color = Color.aliceBlue;
            enemyAreaLeft.GetComponent<Image>().color = Color.mediumVioletRed;
            enemyAreaUp.GetComponent<Image>().color = Color.mediumVioletRed;
            enemyAreaRight.GetComponent<Image>().color = Color.mediumVioletRed;
        }
        else if (OwnerClientId == 2)
        {
            playerArea.GetComponent<Image>().color = Color.lightGoldenRodYellow;
            enemyAreaLeft.GetComponent<Image>().color = Color.forestGreen;
            enemyAreaUp.GetComponent<Image>().color = Color.forestGreen;
            enemyAreaRight.GetComponent<Image>().color = Color.forestGreen;
        }
        else
        {
            playerArea.GetComponent<Image>().color = Color.darkGoldenRod;
            enemyAreaLeft.GetComponent<Image>().color = Color.gray;
            enemyAreaUp.GetComponent<Image>().color = Color.gray;
            enemyAreaRight.GetComponent<Image>().color = Color.gray;
        }

            hasSetAreas = true;
    }


    // These are for the Select cards and tags
    public void setSelected(GameObject card)
    {
        selectedCards.Add(card);
        selectedTags.Add(card.tag);
    }
    public void removeSelected(GameObject card)
    {
        selectedCards.Remove(card);
        selectedTags.Remove(card.tag);
    }
    public int getSelectedCount()
    {
        return selectedTags.Count;
    }
    public bool selectContains(string tag)
    {

        return selectedTags.Contains(tag);
    }
    public bool OnlyJokers()
    {
        if (selectedTags.Contains("Joker"))
        {
            int jokerCount = 0;
            foreach (string joker in selectedTags)
            {
                if (joker == "Joker")
                {
                    jokerCount++;
                }
            }
            if (selectedTags.Count == jokerCount)
            {
                return true;
            }
        }
        return false;
    }
    public void setHas3OfSpades(bool b)
    {
        has3ofSpades = b;
    }
    public bool getHas3OfSpades()
    {
        return has3ofSpades;
    }

    // Set for rank
    public void setRank(string rank)
    {
        playerRank.Value = rank;
        //playerRank = rank;
    }

    // These spawns the player's and enemies hand
    public void SpawnHandCard(CardSpawnData data, NetworkCard nc)
    {
        GameObject myCard = Instantiate(cardPrefab, data.targetArea, false);
        myCard.name = nc.cardName.Value.ToString();
        myCard.tag = nc.cardTag.Value.ToString();
        myCard.transform.GetChild(0).GetComponent<Image>().sprite = NetworkCard.allSprites.ElementAt(nc.spriteIndex.Value);
        cardNametoId.Add(nc.cardName.Value.ToString(), data.netId);
    }
    public void SpawnEnemyCard(CardSpawnData data, NetworkCard ncBack)
    {
        GameObject cardBack = Instantiate(cardPrefab.gameObject, data.targetArea, false);
        int rotate = data.targetArea.name.Contains("Left") ? 270 : data.targetArea.name.Contains("Up") ? 180 : data.targetArea.name.Contains("Right") ? 90 : 0;

        cardBack.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        cardBack.GetComponent<Outline>().effectColor = Color.whiteSmoke;
        cardBack.name = ncBack.cardName.Value.ToString();
        cardBack.tag = ncBack.cardTag.Value.ToString();

        cardBack.transform.GetChild(0).GetComponent<Image>().sprite = NetworkCard.allSprites.ElementAt(ncBack.spriteIndex.Value);
        cardBack.transform.GetChild(0).GetComponent<Image>().transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotate));
    }


    public void addCardsToDropzone()
    {
      
        List<int> temp = new List<int>();
        List<GameManager.PlayedCardState> dc =  new List<GameManager.PlayedCardState>();
        for (int i = 0; i < selectedTags.Count; i++)
        {
            GameObject c = selectedCards[i];
            Debug.Log($"This is the id for {c.name}: {cardNametoId[c.name]}");
            temp = GameManager.Instance.getRandomCardData(i, selectedTags.Count);
            if(GameManager.Instance.cardRank[c.tag] == 8)
            {
                GameManager.Instance.settingEightServerRpc();
            }
            if(c.name == "3Spades")
            {
                GameManager.Instance.settingThreeReversalServerRpc();
            }

            GameManager.PlayedCardState cardInfo =  new GameManager.PlayedCardState(cardNametoId[c.name], PlayerId, temp[0], temp[1], temp[2], GameManager.Instance.cardRank[c.tag]);
            dc.Add(cardInfo);
            Destroy(c);
        }
        GameManager.Instance.PlayCardServerRpc(dc.ToArray(), PlayerId, PlayerHandCount.Value - selectedTags.Count);
        PlayerHandCount.Value -= selectedTags.Count;

        //callingPlayerPassClientRpc(0, PlayerHandCount.Value, PlayerId);
        callingPlayerPass(0, PlayerHandCount.Value, PlayerId);
        selectedCards.Clear();
        selectedTags.Clear();


    }
    public void SetHandCards(int newVal)
    {
        if (!IsOwner) return;
        PlayerHandCount.Value = newVal;

    }
    //[ClientRpc]
    //public void callingPlayerPassClientRpc(int val, int handcount, ulong id)
    public void callingPlayerPass(int val, int handcount, ulong id)
    {
        GameManager.Instance.playerPassServerRpc(val, handcount, id);
    }
    

    public void clearHand()
    {
        PlayerHandCount.Value = 0;   
    }

    public void clearTheBoard()
    {
        clearHand();
        foreach(GameObject child in playerArea.gameObject.transform)
        {
            Destroy(child);
        }
        foreach(GameObject child in enemyAreaLeft.gameObject.transform)
        {
            Destroy(child);
        }
        foreach (GameObject child in enemyAreaUp.gameObject.transform)
        {
            Destroy(child);
        }
        foreach (GameObject child in enemyAreaRight.gameObject.transform)
        {
            Destroy(child);
        }
    }

    public void Update()
    {

        //Put queue in here 
        if (cardQueue.Count > 0)
        {
            spawnTimer -= Time.deltaTime;
            if(spawnTimer <= 0f)
            {
                spawnTimer = spawnDelay;
                var data = cardQueue.Dequeue();
                if(data.spawnType == "enemy")
                {
                    if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(data.netId, out var cardObj))
                    {
                        SpawnEnemyCard(data, cardObj.GetComponent<NetworkCard>());
                    }
                }
            }
        }
    }


}

