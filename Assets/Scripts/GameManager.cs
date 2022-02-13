using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public int turnsPlayed = 0;

    public void UpdateTurnsPlayed()
    {
        turnsPlayed++;
    }

}
