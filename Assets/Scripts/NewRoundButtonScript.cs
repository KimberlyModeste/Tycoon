using Unity.Netcode;
using UnityEngine;

public class NewRoundButtonScript : NetworkBehaviour
{
    public void onClick()
    {
        Destroy(this.gameObject);

    }
}
