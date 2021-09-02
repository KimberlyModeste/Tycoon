using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class readyScript : MonoBehaviour
{
    //instantiate types of cards
    public GameObject card;
    public GameObject card2;

    List<GameObject> cards = new List<GameObject>();

    public GameObject readyButton;
    public GameObject passButton;
    public GameObject selectButton;

    public GameObject canvas;
    public GameObject playerArea;
    public GameObject dropzone;

    /*private string packname = "-Omori";
    
    * public GameObject enemyArea1;
    * public GameObject enemyArea2;
    * public GameObject enemyArea3;*/




    void Start()

    {
        //Add all the cards ill just use a loop here
        /* card.GetComponent<Image>().sprite = Resources.Load<Sprite>([packName] + "nameSprite"); 
        card.GetComponent<Image>().sprite = Resources.Load<Sprite>("Joker"+ packname);*/

        canvas = GameObject.Find("Main Canvas");
        cards.Add(card);
        cards.Add(card); cards.Add(card); cards.Add(card); cards.Add(card); cards.Add(card); //Delete this when done testing
        cards.Add(card2); cards.Add(card2); cards.Add(card2); cards.Add(card2); cards.Add(card2); cards.Add(card2);

    }

    public void onClick()
    {
        /*There are 52 cards in a standard deck but i need to check if there are still cards
        left change to i < 13
        
         */
        Destroy(readyButton);
        for (var i = 0; i < 5; i++)
        {

            GameObject rand = cards[Random.Range(0, cards.Count)];
            GameObject playerCard = Instantiate(rand, new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(playerArea.transform, false);
            cards.Remove(rand);

            /*
           GameObject enemy1Card = Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
           enemy1Card.transform.SetParent(enemyArea1.transform, false);

           GameObject enemy2Card = Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
           enemy2Card.transform.SetParent(enemyArea2.transform, false);

           GameObject enemy3Card = Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
           enemy3Card.transform.SetParent(enemyArea3.transform, false);

        */ 
        }
        GameObject playerSelect = Instantiate(selectButton, new Vector2(634, -230), Quaternion.identity);
        playerSelect.transform.SetParent(canvas.transform, false);

        GameObject playerPass = Instantiate(passButton, new Vector2(634, -313), Quaternion.identity);
        playerPass.transform.SetParent(canvas.transform, false);

        GameObject dropPlace = Instantiate(dropzone, new Vector2(0, 0), Quaternion.identity);
        dropPlace.transform.SetParent(canvas.transform, false);


    }
}
