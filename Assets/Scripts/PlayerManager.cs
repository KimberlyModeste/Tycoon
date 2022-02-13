using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerManager : NetworkBehaviour
{

    //Clubs
    public GameObject ACcard;
    public GameObject TwoCcard;
    public GameObject ThreeCcard;
    public GameObject FourCcard;
    public GameObject FiveCcard;
    public GameObject SixCcard;
    public GameObject SevenCcard;
    public GameObject EightCcard;
    public GameObject NineCcard;
    public GameObject TenCcard;
    public GameObject JCcard;
    public GameObject QCcard;
    public GameObject KCcard;
    //Spades 
    public GameObject AScard;
    public GameObject TwoScard;
    public GameObject ThreeScard;
    public GameObject FourScard;
    public GameObject FiveScard;
    public GameObject SixScard;
    public GameObject SevenScard;
    public GameObject EightScard;
    public GameObject NineScard;
    public GameObject TenScard;
    public GameObject JScard;
    public GameObject QScard;
    public GameObject KScard;
    //Diamonds
    public GameObject ADcard;
    public GameObject TwoDcard;
    public GameObject ThreeDcard;
    public GameObject FourDcard;
    public GameObject FiveDcard;
    public GameObject SixDcard;
    public GameObject SevenDcard;
    public GameObject EightDcard;
    public GameObject NineDcard;
    public GameObject TenDcard;
    public GameObject JDcard;
    public GameObject QDcard;
    public GameObject KDcard;
    //Hearts
    public GameObject AHcard;
    public GameObject TwoHcard;
    public GameObject ThreeHcard;
    public GameObject FourHcard;
    public GameObject FiveHcard;
    public GameObject SixHcard;
    public GameObject SevenHcard;
    public GameObject EightHcard;
    public GameObject NineHcard;
    public GameObject TenHcard;
    public GameObject JHcard;
    public GameObject QHcard;
    public GameObject KHcard;

    //Jokers
    public GameObject Joker1;
    public GameObject Joker2;

    public GameObject playerArea;
    public GameObject enemyArea1;
    public GameObject enemyArea2;
    public GameObject enemyArea3;
    public GameObject dropzone;
    public GameObject readyButton;
    public GameObject selectButton;

    List<GameObject> cards = new List<GameObject>();
    private int count = 0;

    public override void OnStartClient()
    {
        base.OnStartClient();

        readyButton = GameObject.Find("Button");
        playerArea = GameObject.Find("GameArea");
        enemyArea1 = GameObject.Find("EnemyArea1");
        enemyArea2 = GameObject.Find("EnemyArea2");
        enemyArea3 = GameObject.Find("EnemyArea3");
        dropzone = GameObject.Find("Dropzone");


        singleton.Instance.netIdHolder.Add(netId);
        
    }

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();

        cards.Add(ThreeCcard); cards.Add(ThreeDcard); cards.Add(ThreeHcard); cards.Add(ThreeScard);
        cards.Add(FourCcard); cards.Add(FourDcard); cards.Add(FourHcard); cards.Add(FourScard);
        cards.Add(FiveCcard); cards.Add(FiveDcard); cards.Add(FiveHcard); cards.Add(FiveScard);
        cards.Add(SixCcard); cards.Add(SixDcard); cards.Add(SixHcard); cards.Add(SixScard);
        cards.Add(SevenCcard); cards.Add(SevenDcard); cards.Add(SevenHcard); cards.Add(SevenScard);
        cards.Add(EightCcard); cards.Add(EightDcard); cards.Add(EightHcard); cards.Add(EightScard);
        cards.Add(NineCcard); cards.Add(NineDcard); cards.Add(NineHcard); cards.Add(NineScard);
        cards.Add(TenCcard); cards.Add(TenDcard); cards.Add(TenHcard); cards.Add(TenScard);
        cards.Add(JCcard); cards.Add(JDcard); cards.Add(JHcard); cards.Add(JScard);
        cards.Add(QCcard); cards.Add(QDcard); cards.Add(QHcard); cards.Add(QScard);
        cards.Add(KCcard); cards.Add(KDcard); cards.Add(KHcard); cards.Add(KScard);
        cards.Add(ACcard); cards.Add(ADcard); cards.Add(AHcard); cards.Add(AScard);
        cards.Add(TwoCcard); cards.Add(TwoDcard); cards.Add(TwoHcard); cards.Add(TwoScard);
        cards.Add(Joker1); cards.Add(Joker2);

        //playerArea.GetComponent<SetEnemies>().myNetId = netId;
    }
  
    [Command]
    public void CmdDealCards()
    {
        List<int> playerRand = new List<int>();
         
        if (singleton.Instance.cardsSet.Count/14 < 2)
        {
            while (playerRand.Count < 14)
            {
                int rand = Random.Range(0, cards.Count);
                if (!singleton.Instance.placeHolder.Contains(rand))
                {
                    singleton.Instance.placeHolder.Add(rand);
                    singleton.Instance.readyPlayer = 13;
                    playerRand.Add(rand);
                    playerRand.Sort();
                }

            }
        }
        else
        {
            while (playerRand.Count < 13)
            {
                int rand = Random.Range(0, cards.Count);
                if (!singleton.Instance.placeHolder.Contains(rand))
                {
                    singleton.Instance.placeHolder.Add(rand);
                    singleton.Instance.readyPlayer = 12;
                    playerRand.Add(rand);
                    playerRand.Sort();
                }

            }
        }

        for (int i = 0; i < playerRand.Count; i++)
        {
            GameObject card = Instantiate(cards[playerRand[i]], new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(card, connectionToClient);
            RpcShowCard(card, "Dealt", singleton.Instance.readyPlayer);
        }

        count = 0;
    }

    public void PlayCards(GameObject card, int num)
    {
        CmdPlayCards(card, num);
    }

    [Command]
    void CmdPlayCards(GameObject card, int num)
    {
        RpcShowCard(card, "Played", num);
        if(isServer)
        {
            UpdateTurnsPlayed();
        }
    }


    [Server]
    void UpdateTurnsPlayed()
    {
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.UpdateTurnsPlayed();
        RpcLogToClients("Turns Played: " + gm.turnsPlayed);
    }

    [ClientRpc]
    void RpcLogToClients(string message)
    {
        Debug.Log(message);
    }

    [ClientRpc]
    void RpcShowCard(GameObject card, string type, int max)
    {
        if (type == "Dealt")
        {

          // if(hasAuthority)
            singleton.Instance.cardsSet.Add(card);
            //If its itself
            if (hasAuthority)
            {
                if (readyButton.activeSelf == true)
                    readyButton.SetActive(false);
                card.transform.SetParent(playerArea.transform, false);
            }
            else
            {

                if (playerArea.GetComponent<SetEnemies>().myNetId == singleton.Instance.netIdHolder[0] || 
                    playerArea.GetComponent<SetEnemies>().myNetId == singleton.Instance.netIdHolder[3] ||
                    playerArea.GetComponent<SetEnemies>().myNetId == 0)
                {
                    if (!playerArea.GetComponent<SetEnemies>().isEnemy1)
                    {
                        card.transform.SetParent(enemyArea1.transform, false);
                        count++;
                        if (count > max)
                        {
                            playerArea.GetComponent<SetEnemies>().isEnemy1 = true;
                            count = 0;
                        }
                    }

                    else if (!playerArea.GetComponent<SetEnemies>().isEnemy2)
                    {
                        card.transform.SetParent(enemyArea2.transform, false);
                        count++;
                        if (count > max)
                        {
                            playerArea.GetComponent<SetEnemies>().isEnemy2 = true;
                            count = 0;
                        }
                    }

                    else if (!playerArea.GetComponent<SetEnemies>().isEnemy3)
                    {
                        card.transform.SetParent(enemyArea3.transform, false);
                        count++;
                        if (count > max)
                        {
                            playerArea.GetComponent<SetEnemies>().isEnemy3 = true;
                            count = 0;
                        }
                    }
                }

                else if (playerArea.GetComponent<SetEnemies>().myNetId == singleton.Instance.netIdHolder[1])
                {
                    if (!playerArea.GetComponent<SetEnemies>().isEnemy1)
                    {
                        card.transform.SetParent(enemyArea3.transform, false);
                        count++;
                        if (count > max)
                        {
                            playerArea.GetComponent<SetEnemies>().isEnemy1 = true;
                            count = 0;
                        }
                    }

                    else if (!playerArea.GetComponent<SetEnemies>().isEnemy2)
                    {
                        card.transform.SetParent(enemyArea1.transform, false);
                        count++;
                        if (count > max)
                        {
                            playerArea.GetComponent<SetEnemies>().isEnemy2 = true;
                            count = 0;
                        }
                    }

                    else if (!playerArea.GetComponent<SetEnemies>().isEnemy3)
                    {
                        card.transform.SetParent(enemyArea2.transform, false);
                        count++;
                        if (count > max)
                        {
                            playerArea.GetComponent<SetEnemies>().isEnemy3 = true;
                            count = 0;
                        }
                    }
                }

                else if (playerArea.GetComponent<SetEnemies>().myNetId == singleton.Instance.netIdHolder[2])
                {
                    if (!playerArea.GetComponent<SetEnemies>().isEnemy1)
                    {
                        card.transform.SetParent(enemyArea2.transform, false);
                        count++;
                        if (count > max)
                        {
                            playerArea.GetComponent<SetEnemies>().isEnemy1 = true;
                            count = 0;
                        }
                    }

                    else if (!playerArea.GetComponent<SetEnemies>().isEnemy2)
                    {
                        card.transform.SetParent(enemyArea3.transform, false);
                        count++;
                        if (count > max)
                        {
                            playerArea.GetComponent<SetEnemies>().isEnemy2 = true;
                            count = 0;
                        }
                    }

                    else if (!playerArea.GetComponent<SetEnemies>().isEnemy3)
                    {
                        card.transform.SetParent(enemyArea1.transform, false);
                        count++;
                        if (count > max)
                        {
                            playerArea.GetComponent<SetEnemies>().isEnemy3 = true;
                            count = 0;
                        }
                    }
                }

                card.GetComponent<CardFlipper>().Flip();

            }

        }
        else if (type == "Played")
        {
            Debug.Log("Amount Cards Left"+singleton.Instance.amountCardsLeft);
            Debug.Log("AmountSelected "+ singleton.Instance.amountSelected);
            //singleton.Instance.amountCardsLeft--;
            //if (singleton.Instance.amountCardsLeft > 1)
            //{
            //    dropzone.GetComponent<GridLayoutGroup>().spacing = new Vector2(-120, 0);
            //    Debug.Log("In 120");
            //}
            //else
            //{
            //    Debug.Log("In 10");
               
            //}
            dropzone.GetComponent<GridLayoutGroup>().spacing = new Vector2(-10, 0);
            card.transform.SetParent(dropzone.transform, false);
            card.transform.Rotate(0, 0, max);
            if (!hasAuthority)
            {
                card.GetComponent<CardFlipper>().Flip();
            }

        }
    }
}