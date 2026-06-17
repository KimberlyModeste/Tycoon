using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IdChecker : NetworkBehaviour
{
    public ulong Id;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }

    public void setId(ulong id)
    {
        this.Id = id;
    }
}
