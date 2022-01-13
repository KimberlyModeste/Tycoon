using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCard : MonoBehaviour
{

    private bool isSelect = false;
    public GameObject selectButton;

    void Start()
    {
        selectButton = GameObject.Find("Select Button");
    }


    public void onclick()
    {
        if (isSelect)
            deselectCard();
        else
            selectCard();
    }

    private void deselectCard()
    {
        string temp;
        transform.position = new Vector2(transform.position.x, transform.position.y - 60); //97
        isSelect = false;
        singleton.Instance.amountSelected -= 1;

        if (singleton.Instance.amountSelected == 0)
        {
            singleton.Instance.selectedTag = "null";
            selectButton.transform.GetChild(0).GetComponent<Text>().text = "Pass";
        }
           
        temp = transform.name.Replace("(Clone)", " (UnityEngine.GameObject)");
        singleton.Instance.holder.Remove(temp);
        Debug.Log(singleton.Instance.amountSelected);
    }

    private void selectCard()
    {
        string temp;
        if (singleton.Instance.selectedTag == "null" || singleton.Instance.selectedTag == "Joker")
        {
            singleton.Instance.selectedTag = this.tag;
        }

        if ((singleton.Instance.selectedTag == this.tag || this.tag == "Joker") && isSelect != true)
        {
            isSelect = true;
            transform.position = new Vector2(transform.position.x, transform.position.y + 60);
            singleton.Instance.amountSelected += 1;
            temp = transform.name.Replace("(Clone)", " (UnityEngine.GameObject)");
            singleton.Instance.holder.Add(temp);
            selectButton.transform.GetChild(0).GetComponent<Text>().text = "Select Card";
        }
    }
}
