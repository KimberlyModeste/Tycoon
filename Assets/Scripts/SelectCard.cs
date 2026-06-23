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


    /* When clicked check if the card exists and then if the one being clicked on belongs to the player. 
     * If the card being selected is already selected and the selected cards are more than 0, remove it from the players hand and deseclect the card. 
     * If:
     *   The player count is 0 || The card being selected has the same tag as those already selected || The card selected is a Joker || There are only Jokers selected so far
     * AND
     *   The card selected is greater than the one in the dropzone || A 3 spade reversal is possible
     */
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

    // Put the card down and set the select to false
    public void deselectCard()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y - movement);
        isSelect = false;

    }
    // Move the card up and set the selected card to true
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

            /* If there is a revolution:
             *      And the card rank of the current card is less than the cards in the dropzone, Return True
             * If there is not a revolution:
             *      And the card rand of the current card is greater than the cards in the dropzone, Return True
             * Else, return False
             */
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

            /* Depending on the current card count (2, 3, 4, 5, 6)
             *   Add that many cards from the end of the top of the dropzone to localCardTags
             */
            for (int i = 0; i < GameManager.Instance.currentCardAmount.Value; i++)
            {
                localCardTags.Add(GameManager.Instance.dropzoneCards[GameManager.Instance.dropzoneCards.Count - (i + 1)].CardRank);
            }

            // Checking how many jokers there are
            int countJokers = localCardTags.Count(n => n == 16);
            
            // If there is another card besides a joker
            if(countJokers < GameManager.Instance.currentCardAmount.Value)
            {
                /* If there is a revolution:
                 *      And the card rank of the current card is less than the first card that isn't a joker in the dropzone, Return True
                 * If there is not a revolution:
                 *      And the card rand of the current card is greater than the first card that isn't a joker in the dropzone, Return True
                 * Else, return False
                 */
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
                // If the card being dropped is also a joker true else false
                if(GameManager.Instance.cardRank[gameObject.tag] == 16)
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
        // If there is only one card and the current card in the dropzone is a Joker, if a 3 of spade is trying to be played retrun true.
        // Else return false.
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
