using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;
//using Random = System.Random;
public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public List<string> aiNames = new List<string>() { "Ann", "Ryuji", "Morgana", "Makoto", "Yusuke", "Futaba", "Haru", "Akechi", "Sumire" };

    // Lists for bigger stuff.
    public Dictionary<string, int> cardRank = new Dictionary<string, int>()
    {
        { "3", 3 },
        { "4", 4 },
        { "5", 5 },
        { "6", 6 },
        { "7", 7 },
        { "8", 8 },
        { "9", 9 },
        { "10", 10 },
        { "J", 11 },
        { "Q", 12 },
        { "K", 13 },
         { "A", 14 },
        { "2", 15 },
        { "Joker", 16 },
    };
    public Stack<string> hierarchy = new Stack<string>();



    public struct PlayedCardState : INetworkSerializable, IEquatable<PlayedCardState>
    {
        public ulong CardNetworkId;   
        public ulong PlayerId;
        public int CardRank;
        public int X;
        public int Y;
        public int Rotate;

        public PlayedCardState(ulong id,ulong pid, int x, int y, int rot, int cardRank)
        {
            CardNetworkId = id;
            PlayerId = pid;
            X = x;
            Y = y;
            Rotate = rot;
            CardRank = cardRank;
        }


        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref CardNetworkId);
            serializer.SerializeValue(ref PlayerId);
            serializer.SerializeValue(ref CardRank);
            serializer.SerializeValue(ref X);
            serializer.SerializeValue(ref Y);
            serializer.SerializeValue(ref Rotate);
        }

        public bool Equals(PlayedCardState other)
        {
            return CardNetworkId == other.CardNetworkId && PlayerId == other.PlayerId;
            // && uiX == other.uiX && uiY == other.uiY;
        }

        public override bool Equals(object obj) => obj is PlayedCardState s && Equals(s);
        public override int GetHashCode() => HashCode.Combine(CardNetworkId, PlayerId);
    }

    public bool hasStartedGame = false;
    public bool hasAllPlayers = false;
    public bool swapcards = false;
    public List<Computer> aiPlayers = new List<Computer>();
    public NetworkList<ulong> aiPlayersId = new NetworkList<ulong>();
    public NetworkList<ulong> allPlayerId = new NetworkList<ulong>();
    public string seatOrder = "";
    public NetworkList<ulong> finishedPlayers = new NetworkList<ulong>();

    // Playing bools
    public bool broadcastedCurPlayer = false;
    public bool eightPlayed = false;
    public bool threeSpadeReversal = false;

    public NetworkVariable<bool> revolution = new NetworkVariable<bool>(false);


    //This is for the players
    public NetworkVariable<int> playerTicker = new NetworkVariable<int>(0);
    public NetworkVariable<int> currentPlayer = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<int> currentCardAmount = new NetworkVariable<int>(0);
 

    public NetworkList<PlayedCardState> dropzoneCards = new NetworkList<PlayedCardState>();
    public NetworkVariable<int> playerPass = new NetworkVariable<int>(0);


    // This is for seeing the order of players once
    public bool hasdoneplayerorderbool = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Debug.Log("In Game Manager Awake");
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log($"In Game Manager spawn");

        aiPlayersId.OnListChanged += OnAIPlayersIdChanged;
        dropzoneCards.OnListChanged += OnPlayedCardsChanged;
    }

    public void OnAIPlayersIdChanged(NetworkListEvent<ulong> change)
    {
        Debug.Log($"AI ID changed. Count: {aiPlayersId.Count}. This is change: {change.Value}");
        RebuildAIList();
    }

    private void RebuildAIList()
    {
        aiPlayers.Clear();
        foreach(var netId in aiPlayersId)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(netId, out var obj)) 
            { 
                Computer com = obj.GetComponent<Computer>();
                if(com != null)
                {
                    com.setGameObjectName();
                    aiPlayers.Add(com);
                    Debug.Log($"Client found AI object netId={netId}, index={com.aiId.Value}, name={com.playerName.Value}, basic name: {com.name}");
                }
            }
            else
            {
                Debug.LogWarning($"Client: AI netId {netId} not yet in SpawnedObjects");
            }
        }
    }
    public void RegisterHumanPlayer(Player p)
    {
        Debug.Log("Registering Player: " + p.PlayerId);
        RequestRegisterPlayerServerRpc(p.PlayerId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestRegisterPlayerServerRpc(ulong playerId)
    {
        allPlayerId.Add(playerId);
    }

    public void RegisterAIPlayer(NetworkObject ai)
    {
        Debug.Log("Setting an ai player: " + ai.NetworkObjectId);
        aiPlayers.Add(ai.GetComponent<Computer>());
        aiPlayersId.Add(ai.NetworkObjectId);
        allPlayerId.Add(ai.GetComponent<Computer>().aiId.Value);
        
    }

    public void RebuildAllList()
    {
        foreach(string s in seatOrder.Split(" - "))
        {
            allPlayerId.Add(ulong.Parse(s));
        }
    }

    // Functions for things
    public void Shuffle()
    {
        var count = allPlayerId.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            var tmp = allPlayerId[i];
            allPlayerId[i] = allPlayerId[r];
            allPlayerId[r] = tmp;
        }
    }

    public void resetRank()
    {
        hierarchy.Push("Beggar");
        hierarchy.Push("Poor");
        hierarchy.Push("Rich");
        hierarchy.Push("Tycoon");
    }

    public List<int> getRandomCardData(int i, int count)
    {
        List<int> ran = new List<int>();
        int x = 0;
        int y = Random.Range(-40, 40); ;
        int rotate = 0;
        int offset = Random.Range(-30, 30);
        switch (count)
        {
            case 1:
                x = 0 + offset;
                rotate = Random.Range(-20, 20);
                break;

            case 2:
                switch (i + 1)
                {
                    case 1:
                        x = -20 + offset;
                        rotate = 10;
                        break;

                    case 2:
                        x = 20 + offset;
                        rotate = -10;
                        break;
                }
                break;

            case 3:
                switch (i + 1)
                {
                    case 1:
                        x = 20 + offset;
                        rotate = 10;
                        break;
                    case 2:
                        x = 0 + offset;
                        rotate = 0;
                        break;
                    case 3:
                        x = 20 + offset;
                        rotate = -10;
                        break;
                }
                break;

            case 4:
                switch (i + 1)
                {
                    case 1:
                        x = -40 + offset;
                        rotate = 20;
                        break;
                    case 2:
                        x = 20 + offset;
                        rotate = 10;
                        break;
                    case 3:
                        x = 0 + offset;
                        rotate = 0;
                        break;
                    case 4:
                        x = 20 + offset;
                        rotate = -10;
                        break;
                }
                break;

            case 5:
                switch (i + 1)
                {
                    case 1:
                        x = -40 + offset;
                        rotate = 20;
                        break;
                    case 2:
                        x = -20 + offset;
                        rotate = 10;
                        break;
                    case 3:
                        x = 0 + offset;
                        rotate = 0;
                        break;
                    case 4:
                        x = 20 + offset;
                        rotate = -10;
                        break;
                    case 5:
                        x = 40 + offset;
                        rotate = -20;
                        break;
                }
                break;

            case 6:
                switch (i + 1)
                {
                    case 1:
                        x = -40 + offset;
                        rotate = 20;
                        break;
                    case 2:
                        x = -20 + offset;
                        rotate = 10;
                        break;
                    case 3:
                        x = 0 + offset;
                        rotate = 0;
                        break;
                    case 4:
                        x = 20 + offset;
                        rotate = -10;
                        break;
                    case 5:
                        x = 40 + offset;
                        rotate = -20;
                        break;
                    case 6:
                        x = 60 + offset;
                        rotate = -30;
                        break;
                }
                break;
        }

        ran.Add(x);
        ran.Add(y);
        ran.Add(rotate);

        return ran;
    }


    // Setting up bools
    [ServerRpc(RequireOwnership = false)]
    public void settingEightServerRpc()
    {
        eightPlayed = true;
    }
    // Setting up bools

    [ClientRpc]
    public void setRevolutionClientRpc()
    {
        settingRevolutionServerRpc();
    }
    
    [ServerRpc(RequireOwnership=false)]
    public void settingRevolutionServerRpc()
    {
        revolution.Value = !revolution.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void settingThreeReversalServerRpc()
    {
        threeSpadeReversal = true;
    }


    // This updates the ui's for all players.
    public void UpdateAllEnemiesUI(int prev, int cur, ulong fromId)
    {
        ulong localPlayerId = NetworkManager.Singleton.LocalClientId;
        Player localPlayer = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();

        Debug.Log($"In update all enemy ui. prev: {prev}, cur: {cur}, from:{fromId}");

        if (!DeckManager.Instance.cardBackRef.Value.TryGet(out NetworkObject cardBackObj)) return;
        
        //See the order of the players
        if(!hasdoneplayerorderbool)
        {
            string orderString = "UAEUI allplayersId order: ";
            foreach (var i in allPlayerId)
            {
                orderString+= $"{i} - ";
            }
            orderString = orderString.Remove(orderString.Length-3);
            Debug.Log(orderString);
            seatOrder = orderString;
            hasdoneplayerorderbool = true;
        }

        localPlayer.arrow.GetComponent<ArrowScript>().SetTurn(currentPlayer.Value);

        if (fromId == localPlayerId) return;


        int index = allPlayerId.IndexOf(localPlayerId);
        int ePos = (allPlayerId.IndexOf(fromId) - index + 4) % 4;

        Transform targetArea = null;

        switch (ePos)
        {
            case 1:
                //Right
                targetArea = localPlayer.enemyAreaRight.transform;
                localPlayer.enemyAreaRight.GetComponent<IdChecker>().setId(fromId);
                break;

            case 2:
                //Up
                targetArea = localPlayer.enemyAreaUp.transform;
                localPlayer.enemyAreaUp.GetComponent<IdChecker>().setId(fromId);
                break;

             case 3:
                //Left
                targetArea = localPlayer.enemyAreaLeft.transform;
                localPlayer.enemyAreaLeft.GetComponent<IdChecker>().setId(fromId);
                break;
            
        }



        if (prev < cur && (targetArea.transform.childCount != cur))
        {
            for (int i = prev; i < cur; i++)
            {
                localPlayer.cardQueue.Enqueue(new Player.CardSpawnData(cardBackObj.NetworkObjectId, targetArea, "enemy"));
            }
        }
        else if (prev > cur && (targetArea.transform.childCount != cur))
        {
            int count = prev - cur;
            
            foreach (Transform child in targetArea.transform)
            {

                if (count == 0)
                {
                    break;
                }
                Destroy(child.gameObject);
                count--;
            }
          
        }
    }



    // This is for swapping cards at the start of the next round.
    public void startGameSwap()
    {
        swapcards = false;
    }

    public void setFirstRoundplayer(ulong pid)
    {
        setNextPlayerServerRpc(allPlayerId.IndexOf(pid));
        showCurrentPlayerClientRpc(allPlayerId.IndexOf(pid));
    }
    
    // Playing the game
    public void startGame()
    {
        Debug.Log("Back in startgame");
        hasStartedGame = true;

        setNextPlayerServerRpc(allPlayerId.IndexOf(1));
        showCurrentPlayerClientRpc(allPlayerId.IndexOf(1));

        //setNextPlayerServerRpc(allPlayerId.IndexOf(12));
        //showCurrentPlayerClientRpc(allPlayerId.IndexOf(12));


        resetRank();
    }

   

    [ClientRpc]
    public void showCurrentPlayerClientRpc(int newPlayer)
    {
        updatingNextPlayer(newPlayer);
    }

    public void playCounterAnimation(int val)
    {
        switch (val)
        {
            case 3:
                // 3 of spades
                break;

            case 4:
                //if a revolution was done
                switch (revolution.Value)
                {
                    case true:
                        // This is a regular revolution
                        break;
                    case false:
                        // Counter revolution
                        break;
                }
                break;

            case 8:
                //If an 8 stop was done
                break;

            default:
                Debug.Log("Not a value");
                break;
        }
    }
    
    private IEnumerator pauseBeforeContinue(int handcount, ulong playerId)
    {
        Debug.Log("In pause before continue.");

        if (eightPlayed)
        {
            playCounterAnimation(8);
        }
        if (threeSpadeReversal)
        {
            playCounterAnimation(3);
        }

        bool shouldClear = (eightPlayed || threeSpadeReversal || (playerPass.Value == (allPlayerId.Count - finishedPlayers.Count) - 1));
        if (shouldClear)
        {
            yield return new WaitForSecondsRealtime(1.5f);
            dropzoneCards.Clear();
        }

        if (finishedPlayers.Count+1 == 3 && handcount == 0)
        {
            Debug.Log("In the ai.count == 1");
            int temp = (currentPlayer.Value + 1) % 4;

            while (finishedPlayers.Count != 4 && finishedPlayers.Contains(allPlayerId[temp]))
            {
                temp = (temp + 1) % 4;
            }
            ulong beggarPlayerId = allPlayerId[temp];


            // Change the aiplayer if it is poor

            if (aiPlayersId.Contains(playerId) && NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerId, out var aiPoorPlayer))
            {
                Computer poorCom = aiPoorPlayer.GetComponent<Computer>();

                Debug.Log($"{poorCom.name} has finished their cards granting them the rank: {hierarchy.Peek()}");
                string rank = hierarchy.Pop();
                Debug.Log($"This is the rank {rank}");

                if (IsServer)
                {
                    poorCom.setRank(rank);
                }
                finishedPlayers.Add(playerId);

            }
            if (!aiPlayersId.Contains(playerId) && NetworkManager.Singleton.ConnectedClients.TryGetValue(playerId, out var humPoorPlayer))
            {
                Player poorPlayer = humPoorPlayer.PlayerObject.GetComponent<Player>();

                Debug.Log($"{poorPlayer.name} has finished their cards granting them the rank: {hierarchy.Peek()}");
                string rank = hierarchy.Pop();
                Debug.Log($"This is the rank {rank}");

                if(IsOwner)
                {
                    poorPlayer.setRank(rank);
                }
                finishedPlayers.Add(playerId);


            }

            //Change the next player
            if (aiPlayersId.Contains(playerId) && NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(beggarPlayerId, out var aiBegPlayer))
            {
                Computer begCom = aiBegPlayer.GetComponent<Computer>();

                Debug.Log($"{begCom.name} has finished their cards granting them the rank: {hierarchy.Peek()}");
                string rank = hierarchy.Pop();
                Debug.Log($"This is the rank {rank}");
                if(IsServer)
                {
                    begCom.setRank(rank);

                }
                finishedPlayers.Add(playerId);

            }
            if (!aiPlayersId.Contains(playerId) && NetworkManager.Singleton.ConnectedClients.TryGetValue(beggarPlayerId, out var humBegPlayer))
            {
                Player begPlayer = humBegPlayer.PlayerObject.GetComponent<Player>();

                Debug.Log($"{begPlayer.name} has finished their cards granting them the rank: {hierarchy.Peek()}");
                string rank = hierarchy.Pop();
                Debug.Log($"This is the rank {rank}");

                if(IsOwner)
                {
                    begPlayer.setRank(rank);
                }
                finishedPlayers.Add(playerId);

            }

            Debug.Log("Before next round");
            startNextRound();

        }
        else
        {
            int temp = (currentPlayer.Value + 1) % 4;
            while (finishedPlayers.Count != 4 && finishedPlayers.Contains(allPlayerId[temp]))
            {
                temp = (temp + 1) % 4;
            }

            switch (handcount)
            {
                case 0:
                    Debug.Log($"Finished all cards an in handcount case 0");

                    if (aiPlayersId.Contains(playerId) && NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerId, out var aiPlayer))
                    {
                        Computer com = aiPlayer.GetComponent<Computer>();

                        Debug.Log($"{com.name} has finished their cards granting them the rank: {hierarchy.Peek()}");
                        string rank = hierarchy.Pop();
                        Debug.Log($"This is the rank {rank}");
                        if(IsServer)
                        {
                            com.setRank(rank);
                        }
                        finishedPlayers.Add(playerId);
                    }
                    if (!aiPlayersId.Contains(playerId) && NetworkManager.Singleton.ConnectedClients.TryGetValue(playerId, out var humPlayer))
                    {
                        Player player = humPlayer.PlayerObject.GetComponent<Player>();

                        Debug.Log($"{player.name} has finished their cards granting them the rank: {hierarchy.Peek()}");
                        string rank = hierarchy.Pop();
                        Debug.Log($"This is the rank {rank}");

                        if(IsOwner)
                        {
                            player.setRank(rank);
                        }
                        finishedPlayers.Add(playerId);

                    }
                    //allPlayerId.Remove(playerId);
                    setNextPlayerServerRpc(temp);
                    showCurrentPlayerClientRpc(temp);
                    break;
                default:
                    switch(eightPlayed)
                    {
                        case true:
                            Debug.Log("In pause befor continue just before setting player case eightplayed = true");
                            eightPlayed = false;
                            temp = currentPlayer.Value;
                            setNextPlayerServerRpc(temp);
                            showCurrentPlayerClientRpc(temp);
                            break;
                        default:
                            switch(threeSpadeReversal)
                            { 
                                case true:
                                    threeSpadeReversal = false;
                                    temp = currentPlayer.Value;
                                    setNextPlayerServerRpc(temp);
                                    showCurrentPlayerClientRpc(temp);
                                    break;

                                default:
                                    if (playerPass.Value == allPlayerId.Count - 1)
                                    {
                                        Debug.Log($"broadcast bool passed: {broadcastedCurPlayer}");
                                        //broadcastingPlayerClientRpc(temp);=
                                        setNextPlayerServerRpc(temp);
                                        showCurrentPlayerClientRpc(temp);
                                        playerPassServerRpc(0, handcount, playerId);
                                    }
                                    else
                                    {
                                        Debug.Log("In pause befor continue just before setting player");
                                        //broadcastingPlayerClientRpc(currentPlayer.Value);
                                        //setNextPlayerServerRpc(temp);
                                        //updatingNextPlayer(temp);
                                        setNextPlayerServerRpc(temp);
                                        showCurrentPlayerClientRpc(temp);

                                    }
                                    break;
                            }
                            break;
                    }
                    break;
            }
             
        }
    }



    public void EndTurn(int handcount, ulong playerId)
    {
        broadcastedCurPlayer = false;
        Debug.Log("In end turn");


        Debug.Log($"There are {finishedPlayers.Count} Players that have finished");
        if (dropzoneCards.Count > 0 && (eightPlayed || threeSpadeReversal || handcount == 0 || (playerPass.Value == (allPlayerId.Count - finishedPlayers.Count) - 1 )))
        {
            StartCoroutine(pauseBeforeContinue(handcount, playerId));
        }
        else 
        {
            int temp = (currentPlayer.Value + 1) % 4;
            while (finishedPlayers.Count != 4 && finishedPlayers.Contains(allPlayerId[temp]))
            {
                temp = (temp + 1) % 4;
            }

            Debug.Log($"Before player set");
            setNextPlayerServerRpc(temp);
            showCurrentPlayerClientRpc(temp);

            Debug.Log($"after player set");
        }
    }

    public void startNextRound()
    {
        Debug.Log("Starting next round");
        foreach(ulong id in allPlayerId)
        {

            RebuildAIList();
            if (aiPlayersId.Contains(id) && NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(id, out var ai))
            {
                Computer com = ai.GetComponent<Computer>();
                com.ClearHand();

            }
            if (!aiPlayersId.Contains(id) && NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(id, out var hum))
            {
                Player player = hum.GetComponent<Player>();
                player.clearHand();
            }
            Debug.Log("Starting next round!");
        }

        finishedPlayers.Clear();
        //setTycoonFirstPlayer();


        //DeckManager.Instance.hasDealtCards = false;
        //swapcards = true;

    }

    public void updatingNextPlayer(int temp)
    {
        Player player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
        if (player == null) return;

        Debug.Log("Is changing arrow");
        player.arrow.GetComponent<ArrowScript>().SetTurn(temp);

    }
    public void setTycoonFirstPlayer()
    {
        foreach(ulong id in allPlayerId)
        {
            if (aiPlayersId.Contains(id) && NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(id, out var aiPlayer))
            {
                Computer com = aiPlayer.GetComponent<Computer>();
                //if(com != null && com.playerRank == "Tycoon")
                if(com != null && com.playerRank.Value == "Tycoon")
                {
                    int temp = allPlayerId.IndexOf(id);
                    setNextPlayerServerRpc(temp);
                    showCurrentPlayerClientRpc(temp);
                }
            }
            if (!aiPlayersId.Contains(id) && NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(id, out var humPlayer))
            {
                Player player = humPlayer.GetComponent<Player>();
                if (player != null && player.playerRank.Value == "Tycoon")//player.playerRank == "Tycoon")
                {
                    int temp = allPlayerId.IndexOf(id);
                    setNextPlayerServerRpc(temp);
                    showCurrentPlayerClientRpc(temp);
                }
            }

        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void setNextPlayerServerRpc(int temp)
    {
        if (!IsServer) return;
        Debug.Log("Set next player");
        //while (finishedPlayers.Count != 4 && finishedPlayers.Contains(allPlayerId[temp]))
        //{
        //    temp = (temp + 1) % 4;
        //}

        currentPlayer.Value = temp;
        playerTicker.Value++;
    }


    [ServerRpc(RequireOwnership = false)]
    public void setNextCardAmountServerRpc(int temp)
    {

        if (!IsServer) return;
        Debug.Log("Set cardAmount");
        currentCardAmount.Value = temp;
    }


    //This is where player plays a card.
    [ServerRpc(RequireOwnership = false)]
    public void PlayCardServerRpc(PlayedCardState[] dc, ulong playerId, int count)
    {

        if (!IsServer) return;
        foreach(PlayedCardState card in dc)
        {
            dropzoneCards.Add(new PlayedCardState { CardNetworkId = card.CardNetworkId, PlayerId = playerId, X = card.X, Y = card.Y, Rotate = card.Rotate, CardRank = card.CardRank });
        }

        EndTurn(count,playerId);
    }

   [ServerRpc(RequireOwnership = false)]
    public void playerPassServerRpc(int value, int handcount, ulong playerId)
    {
        
        if (playerPass.Value == 0 && value == 0) return;
        if (!IsServer) return;
        if(value == 0)
        {
            playerPass.Value = 0;
            Debug.Log("Pass reset");
        }
        else
        {
            playerPass.Value += 1;
            Debug.Log($"Player has passed. Passvalue = {playerPass.Value}");

            EndTurn(handcount,playerId);

        }
    }



    // This is handler for adding/removing cards from the dropzone
    private void OnPlayedCardsChanged(NetworkListEvent<PlayedCardState> changeEvent)
    {
        Debug.Log($"onplayerd change event: {changeEvent.Type}");


        if (changeEvent.Type == NetworkListEvent<PlayedCardState>.EventType.Add)
        {
            Debug.Log("In the add event type");
            var state = changeEvent.Value;
            HandlePlayedCardAdded(state);
        }
        if (changeEvent.Type == NetworkListEvent<PlayedCardState>.EventType.Clear)
        {
            var state = changeEvent.Value;
            Debug.Log("In the clear");
            Debug.Log($"is state == null {state.IsUnityNull()}");
            Debug.Log($"is state.playerid {state.PlayerId}");

            ClearDropzone(state.PlayerId);
        }
    }

    private void HandlePlayedCardAdded(PlayedCardState state)
    {
        Debug.Log("In handled playcards");
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(state.CardNetworkId, out var netObj))
        {
            var netCard = netObj.GetComponent<NetworkCard>();
            CreateDropzoneVisualForClient(netCard, state.PlayerId, state);
        }

    }

    private void CreateDropzoneVisualForClient(NetworkCard nc, ulong playerId, PlayedCardState s)
    {
        Debug.Log("Added to dropzone");
        NetworkObject localPlayerObj = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        if (localPlayerObj == null) return;
        Player player = localPlayerObj.GetComponent<Player>();
        Transform dropzone = player.dropzone.transform;


        GameObject dropCard = Instantiate(player.cardPrefab, dropzone, false);
        dropCard.name = $"Played_{nc.cardName.Value.ToString()}";
        dropCard.tag = nc.cardTag.Value.ToString();
        dropCard.transform.GetChild(0).GetComponent<Image>().sprite = NetworkCard.allSprites.ElementAt(nc.spriteIndex.Value);

        dropCard.transform.localPosition = new Vector3(s.X, s.Y, 0);
        dropCard.transform.rotation = Quaternion.Euler(new Vector3(0, 0, s.Rotate));

        Debug.Log(dropCard.name);
    }

    private void ClearDropzone(ulong playerId)
    {
        NetworkObject localPlayerObj = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        if (localPlayerObj == null) return;
        Player player = localPlayerObj.GetComponent<Player>();
        Transform parent = player.dropzone.transform;


        int count = parent.childCount;
        //for(int i = count-1; i >= 0; i--)
        //{
        //    //Debug.Log($"Should be killing the children: {parent.GetChild(i).gameObject.name}");
        //    Destroy(parent.GetChild(i).gameObject);
            
        //}
        foreach(Transform child in parent.transform)
        {
            Debug.Log($"Should be killing the child: {child.name}");
            Destroy(child.gameObject);
        }
        //currentCardAmount.Value = 0;
        setNextCardAmountServerRpc(0);
    }
}
