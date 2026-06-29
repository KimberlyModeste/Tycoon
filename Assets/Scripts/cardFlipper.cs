using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardFlipper : MonoBehaviour
{
    private string packname = "Standard";
    private Sprite cardBack;
    private Sprite cardFront;

    public void Flip()
    {
        Sprite currentSprite = gameObject.GetComponent<Image>().sprite;
        cardBack = Resources.Load<Sprite>(packname + "/CardBack");
       
        if (gameObject.name[1] != 'o')
        {
            if (gameObject.name[1] == '0')
                cardFront = Resources.Load<Sprite>(packname + "/10" + gameObject.name[2]);

            else
                cardFront = Resources.Load<Sprite>(packname + "/" + gameObject.name[0] + gameObject.name[1]);
        }
        else if (gameObject.name[5] == '2')
        {
            cardFront = Resources.Load<Sprite>(packname + "/Joker2");
        }
        else
        {
            cardFront = Resources.Load<Sprite>(packname + "/Joker");
        }




        if (currentSprite == cardFront)
        {
            gameObject.GetComponent<Image>().sprite = cardBack;
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = cardFront;
        }
        
    }
}
