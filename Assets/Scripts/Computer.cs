using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Unity.Collections;
using Unity.Multiplayer.Playmode;
using Unity.Netcode;
using UnityEngine;
using static GameManager;

//using Random = System.Random;

public class Computer : NetworkBehaviour
{
    public NetworkVariable<ulong> aiId = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<FixedString64Bytes> playerName = new NetworkVariable<FixedString64Bytes>(new FixedString64Bytes(""), NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    
   
    public NetworkVariable<int> comHandCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public Dictionary<string, ulong> cardNametoId = new Dictionary<string, ulong>();

    public List<NetworkCard> cards = new List<NetworkCard>();
    private bool has3ofSpades = false;

    public NetworkVariable<FixedString64Bytes> playerRank = new NetworkVariable<FixedString64Bytes>(new FixedString64Bytes("Commoner"), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    //public string playerRank = "Commoner";


    /* Rules
     * Playing an 8 card ends the chain so the person who played it goes first.
     * 3 of spades is larger than an unpaired joker.
     * Joker's can be used to make a copy of a card.
     * Revolution playing 4 of the same card reverses the strength of the cards.
     */

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

       

        comHandCount.OnValueChanged += (prev, newVal) =>
        {
            GameManager.Instance.UpdateAllEnemiesUI(prev, newVal, aiId.Value);
        };
        if(IsServer)
        {
            int val = Random.Range(0, GameManager.Instance.aiNames.Count - 1);

            string name = GameManager.Instance.aiNames[val];
            playerName.Value = new FixedString64Bytes(name);
            gameObject.name = playerName.Value.ToString();

            GameManager.Instance.aiNames.RemoveAt(val);

            //GameManager.Instance.currentPlayer.OnValueChanged += OnTurnChange;
            GameManager.Instance.playerTicker.OnValueChanged += OnTurnChange;
        }

    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (playerName.Value.ToString() != "")
        {
            GameManager.Instance.aiNames.Add(playerName.Value.ToString());
        }

        comHandCount.OnValueChanged -= (prev, newVal) =>
        {
            GameManager.Instance.UpdateAllEnemiesUI(prev, newVal, aiId.Value);
        };
    }



    [ServerRpc(RequireOwnership = false)]
    public void SetAiServerRpc(ulong index)
    {
        aiId.Value = index;
        

        //playerName.Value = //new FixedString64Bytes($"AI {index}");

        GameManager.Instance.RegisterAIPlayer(GetComponent<NetworkObject>());
    }

    public void SetAiHandCount(int count)
    {
        if(!IsServer) return;
        comHandCount.Value = count;
    }

    public void setGameObjectName()
    {   if(playerName.Value != "")
        gameObject.name = playerName.Value.ToString();
    }

    public void setRank(string rank)
    {

        playerRank.Value = rank;
        //playerRank = rank;
    }

    public void setHas3OfSpades(bool b)
    {
        has3ofSpades = b;
    }
    public bool getHas3OfSpades()
    {
        return has3ofSpades;
    }

    public void OnTurnChange(int oldVal, int newVal)
    {
        if(GameManager.Instance.currentPlayer.Value == GameManager.Instance.allPlayerId.IndexOf(aiId.Value))
        {

            if (GameManager.Instance.dropzoneCards.Count == 0)
            {
                Debug.Log("First player to drop");
                StartCoroutine(FirstToDrop());
            }
            else
            {
                Debug.Log("Play after");
                StartCoroutine(PlayingAfterPlayer());
            }
            
        }
    }

    //This is a basic bot
    public List<NetworkCard> basicOpeningPlay()
    {
        List<NetworkCard> playinghand = new List<NetworkCard>();
        playinghand.Add(cards[Random.Range(0, cards.Count - 1)]);
        return playinghand;
    }


    //This is for better bots
    public List<NetworkCard> aggressiveOpeningPlay()
    {
        List<NetworkCard> playinghand = new List<NetworkCard>();
        List<NetworkCard> jokers = cards.Where(c => c.cardTag.Value == "Joker").ToList();
        List<NetworkCard> nonJokers = cards.Where(c => c.cardTag.Value != "Joker").ToList();

        int maxSelected = nonJokers.GroupBy(g => GameManager.Instance.cardRank[g.cardTag.Value.ToString()]).Max(m => m.Count());
        List<NetworkCard> selected = nonJokers.GroupBy(g => GameManager.Instance.cardRank[g.cardTag.Value.ToString()]).Where(w => w.Count() == maxSelected).FirstOrDefault().ToList();


        if (selected == null) 
            return playinghand;

        playinghand.AddRange(selected);
       

        foreach(NetworkCard j in jokers)
        {
            if(playinghand.Count <= 6)
            {
                playinghand.Add(j);
            }
        }

        if (playinghand.Count >= 4)
            GameManager.Instance.revolution.Value = !GameManager.Instance.revolution.Value;

        return playinghand;
    }

    public int findTopRank()
    {

        // If the cards played is 1 by 1
        if (GameManager.Instance.currentCardAmount.Value == 1)
        {
           return GameManager.Instance.dropzoneCards[GameManager.Instance.dropzoneCards.Count - 1].CardRank;
        }

        // If there cards played is more than 1 by 1
        if (GameManager.Instance.currentCardAmount.Value > 1)
        {
            List<int> localCardTags = new List<int>();
            for (int i = 0; i < GameManager.Instance.currentCardAmount.Value; i++)
            {
                localCardTags.Add(GameManager.Instance.dropzoneCards[GameManager.Instance.dropzoneCards.Count - (i + 1)].CardRank);
            }
            int countJokers = localCardTags.Count(n => n == 16);

            // if there are less jokers than numbers sent one of the other cards that aren't jokers
            if (countJokers < GameManager.Instance.currentCardAmount.Value)
            {
                return localCardTags.Where(n => n != 16).ToArray()[0];
            }
            // If there are only jokers
            if (countJokers == GameManager.Instance.currentCardAmount.Value)
            {
                return 16;
            }
        }

        return 0;
    }


    public List<NetworkCard> playing()
    {
        Debug.Log("AI playing");
        int topRank = findTopRank(); 

        List <NetworkCard> jokers = cards.Where(c => c.cardTag.Value == "Joker").ToList();
        var notjokers = cards
                        .Where(c => c.cardTag.Value != "Joker")
                        .GroupBy(c => c.cardTag)
                        .OrderBy(g => GameManager.Instance.cardRank[g.Key.Value.ToString()])
                        .ToList();

        foreach (var n in notjokers)
        {
            int njRank = GameManager.Instance.cardRank[n.Key.Value.ToString()];

            //GameManager.Instance.revolution ? Debug.Log("There is a revolution") : Debug.Log("there is no revolution");
            if (GameManager.Instance.revolution.Value ? njRank > topRank : njRank < topRank)
                continue;

            int need = GameManager.Instance.currentCardAmount.Value;
            List<NetworkCard> selectedCards = new List<NetworkCard>(n);

            while((selectedCards.Count < need) && (jokers.Count > 0))
            {
                selectedCards.Add(jokers[0]);
                jokers.RemoveAt(0);
            }
            

            if (selectedCards.Count == need)
            {
                if (selectedCards.Count >= 4)
                    GameManager.Instance.setRevolutionClientRpc();
                return selectedCards;
            }

        }

        return new List<NetworkCard>();
    }

    public bool isPlayingThreeReversal(string cardname)
    {
        if(cardname == "3Spades" && GameManager.Instance.dropzoneCards.Count > 0)
        {
            if(findTopRank() == 16)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    // This is to wait when setting cards
    private IEnumerator PlayingAfterPlayer()
    {
        float waitTime = Random.Range(2.0f, 3.0f);
        //bool hasOnlyAI = true;
        //foreach(var ai in GameManager.Instance.aiPlayersId)
        //{
        //    if(GameManager.Instance.finishedPlayers.Contains(ai))
        //    {
        //        hasOnlyAI = false;
        //    }
        //}
        
        //if(GameManager.Instance.allPlayerId.Count - GameManager.Instance.aiPlayersId.Count == GameManager.Instance.finishedPlayers.Count && hasOnlyAI)
        //{
        //    waitTime = Random.Range(0.5f, 1.0f);
        //}


        
        Debug.Log($"AI will wait {waitTime}");
        yield return new WaitForSecondsRealtime(waitTime);

        List<NetworkCard> selectedCards = playing();


        if (selectedCards.Where(x => GameManager.Instance.cardRank[x.cardTag.Value.ToString()] == 8 ).Count() > 0)
        {
            GameManager.Instance.settingEightServerRpc();
        }
        if (selectedCards.Count == 1 && isPlayingThreeReversal(selectedCards[0].cardName.Value.ToString()))
        {
            GameManager.Instance.settingThreeReversalServerRpc();
        }


        if (selectedCards.Count > 0)
        {
            SendCardsToDropzone(selectedCards);
            GameManager.Instance.playerPassServerRpc(0, comHandCount.Value, aiId.Value);
        }
        else
        {
            GameManager.Instance.playerPassServerRpc(1, comHandCount.Value, aiId.Value);
        }
        waitTime = Random.Range(2.0f, 3.0f);
        yield return new WaitForSecondsRealtime(waitTime);
    }

    private IEnumerator FirstToDrop()
    {
        float waitTime = Random.Range(2.0f, 3.0f);
        Debug.Log($"AI {aiId.Value} will wait {waitTime}");
        yield return new WaitForSecondsRealtime(waitTime);

        List<NetworkCard> selectedCards = basicOpeningPlay();
        //List<NetworkCard> selectedCards = aggressiveOpeningPlay();


       



        Debug.Log($"{aiId.Value} selected cards. count {selectedCards.Count}");

        if (selectedCards.Count > 0)
        {
            Debug.Log($"Before setting new card amount for {aiId.Value}");
            GameManager.Instance.setNextCardAmountServerRpc(selectedCards.Count);

            Debug.Log($"Before setting pass to 0 {aiId.Value}");
            GameManager.Instance.playerPassServerRpc(0, comHandCount.Value, aiId.Value);

            Debug.Log($"Before sending to dropzone {aiId.Value}");
            SendCardsToDropzone(selectedCards);

           

            Debug.Log($"after all that. {aiId.Value}");


        }

        waitTime = Random.Range(2.0f, 3.0f);
        yield return new WaitForSecondsRealtime(waitTime);
    }

    public void SendCardsToDropzone(List<NetworkCard> selectedCards)
    {
        List<int> temp = new List<int>();
        List<GameManager.PlayedCardState> dc = new List<GameManager.PlayedCardState>();
        int count = selectedCards.Count;

        for (int i = 0; i < count; i++)
        {
            NetworkCard c = selectedCards[i];
            Debug.Log($"CardnametId count {cardNametoId.Count}");
            Debug.Log($"selected card: {c.cardName.Value.ToString()} to Id: {cardNametoId[c.cardName.Value.ToString()]}");
            Debug.Log($"selected card: {c.cardTag.Value.ToString()}");

            temp = GameManager.Instance.getRandomCardData(i, selectedCards.Count);
            Debug.Log($"temp X:{temp[0]}, Y:{temp[1]}, rotate{temp[2]}");
            if (GameManager.Instance.cardRank[c.cardTag.Value.ToString()] == 8)
            {
                GameManager.Instance.settingEightServerRpc();
            }
            
            if(isPlayingThreeReversal(c.cardName.Value.ToString()))
            {
                GameManager.Instance.settingThreeReversalServerRpc();
            }

            GameManager.PlayedCardState card = new GameManager.PlayedCardState(cardNametoId[c.cardName.Value.ToString()], aiId.Value, temp[0], temp[1], temp[2], GameManager.Instance.cardRank[c.cardTag.Value.ToString()]);
            dc.Add(card);

            cards.Remove(c);
        }

        GameManager.Instance.PlayCardServerRpc(dc.ToArray(), aiId.Value, comHandCount.Value - count);
        comHandCount.Value -= count;
    }

    public void ClearHand()
    {
        comHandCount.Value = 0;
    }
   

  
}
