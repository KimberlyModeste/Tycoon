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
    public uint myNetId = 0;

    private void Start()
    {
        singleton.Instance.netIdHolder.Sort();
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        playerManager = networkIdentity.GetComponent<PlayerManager>();

        myNetId = playerManager.netId;

        if(myNetId == 0)
        {
            myNetId = singleton.Instance.netIdHolder[0];
        }
    }

}
