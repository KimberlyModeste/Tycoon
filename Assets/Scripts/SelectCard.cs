using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class SelectCard : MonoBehaviour
{
    private bool isSelect = false;
    private int movement = 25;
    public GameObject playerAreaParent;

    public void onClick()
    {
        if(transform.parent != null)
        {
            //This is the player area
            GameObject parent = transform.parent.gameObject;

            if (!parent.name.Contains("Player")) return;

            Player player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();


            if (isSelect)
            {
                
                if (player.getSelectedCount() > 0)
                {
                    player.removeSelected(gameObject);
                }
                deselectCard();
            }
            else
            {

                Debug.Log($"(player.getSelectedCount() == 0: {player.getSelectedCount() == 0}");
                Debug.Log($"isGreaterThandropped: {isGreaterThandropped()}");

                if (((player.getSelectedCount() == 0)|| player.selectContains(gameObject.tag)|| (gameObject.tag == "Joker")||player.OnlyJokers()) && (isGreaterThandropped() || threeSpadeReversal()))
                {
                   player.setSelected(gameObject);
                   selectCard();
                }

            }

        }
    }

    public void deselectCard()
    {
        
        transform.position = new Vector2(transform.position.x, transform.position.y - movement);
        isSelect = false;
        
    }
    public void selectCard()
    {
        isSelect = true;
        transform.position = new Vector2(transform.position.x, transform.position.y + movement);
    }

    public bool isGreaterThandropped()
    {
        Debug.Log($"Gamemanage currentcardamount: {GameManager.Instance.currentCardAmount.Value}");
        //True means we're good to go
        //If there are no cards in dropzone lets continue
        if(GameManager.Instance.dropzoneCards.Count == 0)
        {
            return true;
        }
        
        // If the cards played is 1 by 1
        if(GameManager.Instance.currentCardAmount.Value == 1)
        {
            //If there is a revolution you can only play cards that are less than or equal to the ones in the dropzone else it's greater than or equal
            int test = GameManager.Instance.revolution.Value ? 1 : 2;
            Debug.Log($"If there is a revolution in select card it will be 1 else 2 ::: {test}");

            if (GameManager.Instance.revolution.Value ? GameManager.Instance.cardRank[gameObject.tag] <= GameManager.Instance.dropzoneCards[GameManager.Instance.dropzoneCards.Count - 1].CardRank : GameManager.Instance.cardRank[gameObject.tag] >= GameManager.Instance.dropzoneCards[GameManager.Instance.dropzoneCards.Count - 1].CardRank)
            {
                return true;
            }
            else 
            {
                return false;
            }

        }
       
        // If there cards played is more than 1 by 1
        if(GameManager.Instance.currentCardAmount.Value > 1)
        {
            List<int> localCardTags = new List<int>();
            for (int i = 0; i < GameManager.Instance.currentCardAmount.Value; i++)
            {
                localCardTags.Add(GameManager.Instance.dropzoneCards[GameManager.Instance.dropzoneCards.Count - (i + 1)].CardRank);
            }
            int countJokers = localCardTags.Count(n => n == 16);
            
            // if there are less jokers than numbers
            if(countJokers < GameManager.Instance.currentCardAmount.Value)
            {
                //If there is a revolution, you can play cards that are less than or equal to rank else we do it normally
                if (GameManager.Instance.revolution.Value ? GameManager.Instance.cardRank[gameObject.tag] <= localCardTags.Where(n => n != 16).ToArray()[0] : GameManager.Instance.cardRank[gameObject.tag] >= localCardTags.Where(n => n != 16).ToArray()[0])
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            // If there are only jokers
            if(countJokers == GameManager.Instance.currentCardAmount.Value)
            {
                if(GameManager.Instance.cardRank[gameObject.tag] == 16 /* || 3 of spades */)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }
                                      
        return false;
    }

    public bool threeSpadeReversal()
    {
        if(GameManager.Instance.currentCardAmount.Value == 1 && GameManager.Instance.dropzoneCards[GameManager.Instance.dropzoneCards.Count - 1].CardRank == 16 && gameObject.name == "3Spades")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
