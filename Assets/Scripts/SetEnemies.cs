using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SetEnemies : NetworkBehaviour
{
    public PlayerManager playerManager;

    [SyncVar]
    public bool isEnemy1 = false;  
    
    [SyncVar]
    public bool isEnemy2 = false;

    [SyncVar]
    public bool isEnemy3 = false;

    [SyncVar]
    public int enemyCount = 0;
}
