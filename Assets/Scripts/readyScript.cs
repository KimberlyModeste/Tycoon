using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class readyScript : NetworkBehaviour
{

    public PlayerManager PlayerManager;
    //instantiate types of cards
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



    List<GameObject> cards = new List<GameObject>();

    public GameObject readyButton;
    public GameObject selectButton;

    public GameObject canvas;
    public GameObject playerArea;

    private string packname = "Standard";
    //C D H S
    private string[] cardNames = {"/3C", "/3D", "/3H", "/3S", "/4C", "/4D", "/4H", "/4S",
     "/5C", "/5D", "/5H", "/5S", "/6C", "/6D", "/6H", "/6S", "/7C", "/7D", "/7H", "/7S",
     "/8C", "/8D", "/8H", "/8S", "/9C", "/9D", "/9H", "/9S", "/10C", "/10D", "/10H", "/10S", 
     "/JC", "/JD", "/JH", "/JS", "/QC", "/QD", "/QH", "/QS", "/KC", "/KD", "/KH", "/KS", 
     "/AC", "/AD", "/AH", "/AS", "/2C", "/2D", "/2H", "/2S","/Joker", "/Joker2" };

    /*
    * public GameObject enemyArea1;
    * public GameObject enemyArea2;
    * public GameObject enemyArea3;*/

    void Start()
    {
        //Change the Theme of cards
        //Clubs
        ACcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/AC");
        TwoCcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/2C");
        ThreeCcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/3C");
        FourCcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/4C");
        FiveCcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/5C");
        SixCcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/6C");
        SevenCcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/7C");
        EightCcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/8C");
        NineCcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/9C");
        TenCcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/10C");
        JCcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/JC");
        QCcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/QC");
        KCcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/KC");

        //Spades
        AScard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/AS");
        TwoScard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/2S");
        ThreeScard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/3S");
        FourScard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/4S");
        FiveScard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/5S");
        SixScard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/6S");
        SevenScard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/7S");
        EightScard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/8S");
        NineScard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/9S");
        TenScard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/10S");
        JScard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/JS");
        QScard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/QS");
        KScard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/KS");

        //Diamonds
        ADcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/AD");
        TwoDcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/2D");
        ThreeDcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/3D");
        FourDcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/4D");
        FiveDcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/5D");
        SixDcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/6D");
        SevenDcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/7D");
        EightDcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/8D");
        NineDcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/9D");
        TenDcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/10D");
        JDcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/JD");
        QDcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/QD");
        KDcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/KD");

        //Hearts
        AHcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/AH");
        TwoHcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/2H");
        ThreeHcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/3H");
        FourHcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/4H");
        FiveHcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/5H");
        SixHcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/6H");
        SevenHcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/7H");
        EightHcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/8H");
        NineHcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/9H");
        TenHcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/10H");
        JHcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/JH");
        QHcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/QH");
        KHcard.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/KH");

        //Jokers
        Joker1.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/Joker");
        Joker2.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + "/Joker2");


        canvas = GameObject.Find("Main Canvas");
        selectButton.SetActive(false);

        
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



        //Adding all the cards to the deck in order 
        //C D H S

        //for (int i = 0; i < 54; i++) 
        //{
        //    GameObject card = Instantiate(ACcard, new Vector2(0, 0), Quaternion.identity);
        //    card.GetComponent<Image>().sprite = Resources.Load<Sprite>(packname + cardNames[i]);
        //    if(cardNames[i][1] != "A")
        //    card.tag = cardNames[i][1].ToString();
        //    else
        //    card.tag ="Ace";
        //    cards.Add(card);
        //}

    }

    public void onClick()
    { 
        selectButton.SetActive(true);

        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        // Debug.Log(PlayerManager.netId);

        PlayerManager.CmdDealCards();
    }

    private void Update()
    {

        if(readyButton.activeSelf)
        if(readyButton.transform.childCount == 0)
        {
            GameObject nrText = new GameObject("Text");
            nrText.transform.SetParent(readyButton.transform, false);
            nrText.AddComponent<Text>().text = "READY!!";
            nrText.GetComponent<Text>().fontSize = 50;
            nrText.GetComponent<Text>().color = Color.black;
            nrText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            nrText.GetComponent<Text>().font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf"); ;
            nrText.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            nrText.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);

        }

        if (selectButton.activeSelf)
        if (selectButton.transform.childCount==0)
        {
            GameObject nsText = new GameObject("Text");
            nsText.transform.SetParent(selectButton.transform, true);
            nsText.AddComponent<Text>().text = "Pass";
            nsText.GetComponent<Text>().fontSize = 30;
            nsText.GetComponent<Text>().color = Color.black;
            nsText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            nsText.GetComponent<Text>().font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf"); ;
            nsText.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            nsText.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            nsText.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            nsText.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

                if (!nsText.activeSelf)
                    nsText.SetActive(true);
        } 
    }

}
