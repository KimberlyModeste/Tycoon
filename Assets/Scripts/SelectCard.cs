using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCard : MonoBehaviour
{
    private bool isSelect = false;

    public void onclick()
    {
        if (isSelect)
            deselectCard();
        else
            selectCard();
    }

    public void deselectCard()
    {
        transform.position = new Vector2(transform.position.x, 97);
        isSelect = false;
        singleton.Instance.amountSelected -= 1;

        if (singleton.Instance.amountSelected == 0)
            singleton.Instance.selectedTag = "null";

        Debug.Log(singleton.Instance.amountSelected);
    }

    public void selectCard()
    {

        if (singleton.Instance.selectedTag == "null")
        {
            singleton.Instance.selectedTag = this.tag;
        }

        if (singleton.Instance.selectedTag == this.tag && isSelect != true)
        {
            isSelect = true;
            transform.position = new Vector2(transform.position.x, 137);
            singleton.Instance.amountSelected += 1;
            Debug.Log(singleton.Instance.amountSelected);


        }
    }
}
