using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class SelectCard : NetworkBehaviour
{
    public PlayerManager playerManager;

    private bool isSelect = false;
    private bool isSelectable = true;
    public GameObject selectButton;
    public GameObject gameArea;
    public GameObject dropzone;

    void Start()
    {
        selectButton = GameObject.Find("Select Button");
        gameArea = GameObject.Find("GameArea");
        dropzone = GameObject.Find("Dropzone");
        if(!hasAuthority)
        {
            isSelectable = false;
        }

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
        if (!isSelectable) return;

        string temp;
        if (transform.parent == gameArea.transform)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y - 60); //97
            isSelect = false;
            singleton.Instance.amountSelected -= 1;

            if (singleton.Instance.amountSelected == 0)
            {
                singleton.Instance.selectedTag = "null";
                selectButton.transform.GetChild(0).GetComponent<Text>().text = "Pass";
            }

            temp = transform.name.Replace("(Clone)", "(Clone) (UnityEngine.GameObject)");
            singleton.Instance.holder.Remove(temp);
        }
    }

    private void selectCard()
    {
        if (!isSelectable) return;

        string temp;
        if (transform.parent == gameArea.transform)
        {
            if (singleton.Instance.selectedTag == "null" || singleton.Instance.selectedTag == "Joker")
            {
                singleton.Instance.selectedTag = this.tag;
            }

            if ((singleton.Instance.selectedTag == this.tag || this.tag == "Joker") && isSelect != true)
            {
                isSelect = true;
                transform.position = new Vector2(transform.position.x, transform.position.y + 60);
                singleton.Instance.amountSelected += 1;
                temp = transform.name.Replace("(Clone)", "(Clone) (UnityEngine.GameObject)");
                singleton.Instance.holder.Add(temp);
                selectButton.transform.GetChild(0).GetComponent<Text>().text = "Select Card";
            }
        }
    }

    public void onSelect()
    {
        if (singleton.Instance.amountSelected > 0)
        {
            singleton.Instance.amountCardsLeft = singleton.Instance.holder.Count;
            int[] randArray = { -30, -15, 0, 15, 30 };
            int randNum = Random.Range(0, 4);
            //Add cards to dropzone
            foreach (GameObject name in singleton.Instance.cardsSet)
            {

                for (int i = 0; i < singleton.Instance.holder.Count; i++)
                {
                    if (name.ToString() == singleton.Instance.holder[i])
                    {
                        isSelectable = false;
                        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
                        playerManager = networkIdentity.GetComponent<PlayerManager>();
                        playerManager.PlayCards(name, randArray[randNum]);
                    }

                }

            }

            singleton.Instance.selectedTag = "null";
            singleton.Instance.amountSelected = 0;
            singleton.Instance.holder.Clear();
            selectButton.transform.GetChild(0).GetComponent<Text>().text = "Pass";
        }
        else
        {
            selectButton.transform.GetChild(0).GetComponent<Text>().text = "Pass";
        }
    }

}
