using Unity.Netcode;
using UnityEngine;


public class ReadyScript : NetworkBehaviour
{
    public void onClick()
    {
        Destroy(this.gameObject);
        NetworkUI.Instance.setClickserverRpc();
        
    }
}
