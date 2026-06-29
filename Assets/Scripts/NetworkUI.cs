using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


using Image = UnityEngine.UI.Image;
using Random = System.Random;
public class NetworkUI : NetworkBehaviour
{
    public static NetworkUI Instance;

    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private TMP_Dropdown playerMaxSetter;

    //public struct PlayerPoints : INetworkSerializable
    //{
    //    ulong playerId;
    //    int points;
    //    FixedString64Bytes currentRank;

    //    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    //    {
    //        throw new NotImplementedException();
    //    }
    //    public void setID(ulong pId)
    //    {
    //        playerId = pId;
    //    }
    //    public void UpdatePoints(int p)
    //    {
    //        points = p;
    //    }
    //    public void UpdateRank(FixedString64Bytes rank)
    //    {
    //        currentRank = rank;
    //    }
    //    public ulong GetPlayerId()
    //    {
    //        return playerId;
    //    }
    //    public int GetPoints()
    //    {
    //        return points;
    //    }

    //    public FixedString64Bytes GetRank()
    //    {
    //        return currentRank;
    //    }
    //}


    private NetworkVariable<int> playersNum = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    
    public NetworkVariable<int> playerClick = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    //public NetworkVariable<List<PlayerPoints>> pointContainer = new NetworkVariable<List<PlayerPoints>>();


    private int playerMax = 2; //Change to 4 when ready to deploy ig
   

    public GameObject aiPrefab;



    public string spritesheetPath = "Persona/PersonaResource";


    private void Awake()
    {
        hostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();

        });
        clientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
        playerMaxSetter.onValueChanged.AddListener(delegate
        {
            ChangePlayerMax(playerMaxSetter.value);
        });
        Instance = this;
    }


    [ServerRpc(RequireOwnership = false)]
    public void setClickserverRpc()
    {
        playerClick.Value += 1;
    }

    [ServerRpc(RequireOwnership =false)]
    public void resetClickServerRpc()
    {
        playerClick.Value = 0;
    }


    public void ChangePlayerMax(int amount)
    {
        if (amount == 0)
        {
            playerMax = 1;
        }
        else if (amount == 1)
        {
            playerMax = 2;
        }
        else if (amount == 2)
        {
            playerMax = 3;
        }
        else if (amount == 3)
        {
            playerMax = 4;
        }
    }

    public void DealCards()
    {
        Debug.Log("In DealCards");
        Random rando = new Random();
        List<int> cardDealt = new List<int> { 13, 13, 14, 14 };
        List<int> cardNums = Enumerable.Range(0, DeckManager.Instance.deck.Count).ToList();

        Debug.Log($"Deckmanager deck count: {DeckManager.Instance.deck.Count}");
        Debug.Log($"ClientList count: {NetworkManager.Singleton.ConnectedClientsList.Count}");

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            List<ulong> dealtCardsIDs = new List<ulong>();
            int cardIndex = rando.Next(0, cardDealt.Count);

            List<int> playerRand = new List<int>();

            int maxHandSize = cardDealt[cardIndex];


            for (int i = 0; i < maxHandSize; i++)
            {
                int ranCard = rando.Next(0, cardNums.Count);
                playerRand.Add(cardNums[ranCard]);
                playerRand.Sort();
                cardNums.RemoveAt(ranCard);
            }

            SortCards(playerRand);


           
            cardDealt.RemoveAt(cardIndex);

            foreach (int i in playerRand)
            {
                dealtCardsIDs.Add(DeckManager.Instance.deck[i].NetworkObjectId);
            }

            //If the player contians the 3 of diamonds they go first.
            if (playerRand.Contains(7))
            {
                Debug.Log($"Player {client.ClientId} is going first.");

                GameManager.Instance.setFirstRoundplayer(client.ClientId);
            }
           

            DealCardsClientRpc(dealtCardsIDs.ToArray(), new ClientRpcParams {
                 Send = new ClientRpcSendParams
                 {
                     TargetClientIds = new ulong[] { client.ClientId }  // only send to this client
                 }
             });
        }


        foreach (ulong aiId in GameManager.Instance.aiPlayersId)
        {
            Debug.Log($"In foreach aiId. AiId {aiId}");
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(aiId, out var com))
            {
                Computer c = com.GetComponent<Computer>();
                List<string> aiplayerRand = new List<string>();

                int cardIndex = rando.Next(0, cardDealt.Count);
                int maxHandSize = cardDealt[cardIndex];


                for (int i = 0; i < maxHandSize; i++)
                {
                    int ranCard = rando.Next(0, cardNums.Count);
                    int place = cardNums[ranCard];
                    if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(DeckManager.Instance.deck[place].NetworkObjectId, out var cardObj))
                    {
                        NetworkCard nc = cardObj.GetComponent<NetworkCard>();
                        c.cardNametoId.Add(nc.cardName.Value.ToString(), cardObj.NetworkObjectId);
                        com.GetComponent<Computer>().cards.Add(nc);
                        aiplayerRand.Add(nc.cardName.Value.ToString());
                    }

                    cardNums.RemoveAt(ranCard);
                }

                if (aiplayerRand.Contains("3Diamonds"))
                {
                    Debug.Log($"{c.playerName.Value} is going first, they contain 3Diamonds");
                    GameManager.Instance.setFirstRoundplayer(aiId);

                }
                if (aiplayerRand.Contains("3Spades"))
                {
                    Debug.Log($"{c.playerName.Value} has 3Spades");

                    c.setHas3OfSpades(true);
                }
                aiplayerRand.Clear();

                cardDealt.RemoveAt(cardIndex);
                com.GetComponent<Computer>().SetAiHandCount(maxHandSize);
            }
        }





        DeckManager.Instance.hasDealtCards = true;
    }

    public void SortCards(List<int> hand)
    {

        // 50 - 53 Queens
        // 46 - 49 Kings
        // 38 - 41 A
        // 1 - 4: 2
        // 0 & 5 Jokers
        List<int> checking = new List<int>() { 50, 51, 52, 53, 46, 47, 48, 49, 38, 39, 40, 41, 1, 2, 3, 4, 0, 5 };
        foreach(int num in checking)
        {
            if(hand.Contains(num))
            {
                hand.Remove(num);
                hand.Add(num);
            }
        }
    }

    [ClientRpc]
    private void DealCardsClientRpc(ulong[] cards, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("In dealcardsclient rpc");
        
        NetworkObject localPlayer = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        ulong clientId = localPlayer.OwnerClientId;
        Player player = localPlayer.GetComponent<Player>();


        player.SetHandCards(cards.Length);

        foreach (ulong c in cards)
        {
            //player.cardQueue.Enqueue(new Player.CardSpawnData(c, player.playerArea.transform, "self"));
            if(c == 9)
            {
                player.setHas3OfSpades(true);
            }

            Player.CardSpawnData data = new Player.CardSpawnData(c, player.playerArea.transform, "self");
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(c, out var cardObj))
            {
                player.SpawnHandCard(data, cardObj.GetComponent<NetworkCard>());
            }
        }
    }

    private void Update()
    {
        
        if(SceneManager.GetActiveScene().name == "NetworkUI")
        {
           

            playerCountText.text = "Players Connected: " + playersNum.Value.ToString();
            if (playersNum.Value == playerMax)
            {
                if (NetworkCard.allSprites == null)
                {
                    NetworkCard.SetGlobalSpriteList(Resources.LoadAll<Sprite>(spritesheetPath).ToList());
                }
                if(playersNum.Value == 4)
                {
                    GameManager.Instance.hasAllPlayers = true;
                }

                if (!IsServer) return;
                NetworkManager.SceneManager.LoadScene("Game Scene", LoadSceneMode.Single);
            }


            if (!IsServer) return;
            playersNum.Value = NetworkManager.Singleton.ConnectedClients.Count;
        }

        else if((SceneManager.GetActiveScene().name == "Game Scene") &&  (playerClick.Value == playerMax))
        {
           
            if (!IsServer) return;


            //if (GameManager.Instance.allPlayerId.Count >= playerMax && GameManager.Instance.allPlayerId.Count < 4)
            if (!GameManager.Instance.hasAllPlayers)
            {
                for (int i = playerMax; i < 4; i++)
                {
                    if (!IsServer) break;

                    GameObject ai = Instantiate(aiPrefab);
                    NetworkObject aino = ai.GetComponent<NetworkObject>();
                    aino.Spawn();

                    Computer AiPlayer = ai.GetComponent<Computer>();
                    AiPlayer.SetAiServerRpc(AiPlayer.NetworkObjectId);
                    //AiPlayer.SetAiServerRpc(i);


                    Debug.Log(AiPlayer.playerName + " has the id " + AiPlayer.aiId);

                    if(GameManager.Instance.allPlayerId.Count == 4)
                    {
                        GameManager.Instance.hasAllPlayers = true;
                    }

                }
            }

            if (DeckManager.Instance.deck.Count < 50 && GameManager.Instance.allPlayerId.Count == 4)
            {
                Debug.Log("All players count: "+ GameManager.Instance.allPlayerId.Count);
                GameManager.Instance.Shuffle();
                DeckManager.Instance.setDeckServerRpc();
            }

            if (DeckManager.Instance.deck.Count > 50 && !DeckManager.Instance.hasDealtCards)
            {
                Debug.Log("Before Dealt cards");
                DealCards();
            }

            if (DeckManager.Instance.hasDealtCards && !GameManager.Instance.hasStartedGame)
            {
                GameManager.Instance.startGame();
            }

            if(GameManager.Instance.swapcards)
            {
                GameManager.Instance.startGameSwap();
            }

        }

    }

 
}
