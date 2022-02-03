using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class singleton : MonoBehaviour
{
    public static singleton Instance { get; private set; }

    public string packname = "Standard";                        //The Package name
    public string selectedTag = "null";                         //Which number is currently selected
    public int amountSelected = 0;                              //How many cards are currently selected
    public int readyPlayer = 0;                             
    public List<string> holder = new List<string>();            //Collects the name of the cards currently selected.
    public List<int> placeHolder = new List<int>();             //Collects the random order of all players
    public List<GameObject> cardsSet = new List<GameObject>();  //Add every card on the board to this for reference.
    

    private void Start()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
}
