using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectButtonScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject dropzone;
    public GameObject canvas;

    void Start()
    {
        canvas = GameObject.Find("Main Canvas");
        dropzone = GameObject.Find("Dropzone");
    }

    public void onClick()
    {
        if (singleton.Instance.amountSelected > 0)
        {
            //add cards to dropzone
            Debug.Log("We have cards");
          /*  foreach (Transform card in singleton.Instance.holder)
            {
                card.SetParent(dropzone.transform, false);
            }
          */
        }
        else
        {
            Debug.Log("N0 cards");
            //Add a warning that there is no cards selected
        }

    }

}

