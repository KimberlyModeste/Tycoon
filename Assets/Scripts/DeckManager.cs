using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class DeckManager : NetworkBehaviour
{
   
    public static DeckManager Instance { get; private set; }


    public NetworkObject cardPrefab;

    public List<NetworkObject> deck = new List<NetworkObject>();
    public NetworkObject cardBack;
    public NetworkVariable<NetworkObjectReference> cardBackRef = new NetworkVariable<NetworkObjectReference>();

    public bool hasSetDeck = false;
    public bool hasDealtCards = false;
    public bool hasSetOpponents = false;

    public string spritesheetPath = "Persona/PersonaResource";
    


    //This makes the DeckManager available for everything
    private void Awake()
    {
        if (Instance == null)
        {
            Debug.Log("In Deck Manager Awake");
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if(IsServer)
        {
            Debug.Log("in server thing");
        }
    }


    [ServerRpc]
    //This sets the deck for the players to use
    public void setDeckServerRpc() 
    {
        if (!IsServer) return;
        Debug.Log("In the setDeckServer");

        List<Sprite> cardSprites = new List<Sprite>(Resources.LoadAll<Sprite>(spritesheetPath));
        NetworkCard.SetGlobalSpriteList(cardSprites);



        for (int i = 0; i < cardSprites.Count; i++)
        {
            Sprite s = cardSprites[i];

            NetworkObject newCard = Instantiate(cardPrefab, new Vector2(0, 0), Quaternion.identity);
            NetworkCard nc = newCard.GetComponent<NetworkCard>();
            string tagname = "Back";
            string name = "Back";
            string suit = s.name.Substring(s.name.Length - 1);


            if (suit == "C" || suit == "D" || suit == "H" || suit == "S")
                tagname = s.name.Substring(0, s.name.Length - 1);
            switch (suit)
            {
                case "C":
                    name = tagname + "Clubs";
                    break;

                case "D":
                    name = tagname + "Diamonds";
                    break;

                case "H":
                    name = tagname + "Hearts";
                    break;

                case "S":
                    name = tagname + "Spades";
                    break;
                case "W":
                    tagname = "Joker";
                    name = tagname + s.name.Substring(0, 1);
                    break;

                default:
                    break;
            }

            nc.InitCard(i, name, tagname);
            newCard.Spawn(true);


            //newCard.hideFlags = HideFlags.HideInHierarchy;



            if (s.name.Substring(s.name.Length - 1) == "B")
            {

                cardBack = newCard;
                cardBackRef.Value = cardBack;
            }
            else
            {
                deck.Add(newCard);
            }

        }
        Debug.Log("DM Set deck count: " + deck.Count);

        if (deck.Count > 52)
        {
            hasSetDeck = true;
        }
    }


}
