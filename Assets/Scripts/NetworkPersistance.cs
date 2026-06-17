using Unity.Netcode;
using UnityEngine;

public class NetworkPersistance : NetworkBehaviour
{
   void Awake()
    {
         DontDestroyOnLoad(gameObject);
    }
}
