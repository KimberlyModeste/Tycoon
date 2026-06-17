using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

using Debug = UnityEngine.Debug;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance { get; private set; }

    //public List<NetworkObject> allPlayers = new List<NetworkObject>();
    public NetworkObject dropzone;
    public GameObject playerAreaUI;



    private void Awake()
    {
        if(Instance == null)
        {
            Debug.Log(" Has gone to Player manager on Awake");
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    //public override void OnNetworkSpawn()
    //{
    //    Debug.Log("Has gone to Player manager on network spawn");
    //    base.OnNetworkSpawn();
    //}


    //public override void OnNetworkDespawn()
    //{
    //    Debug.Log("In despawn");

    //    base.OnNetworkDespawn();
    //}


}
