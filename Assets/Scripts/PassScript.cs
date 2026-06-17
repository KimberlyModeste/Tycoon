using Unity.Netcode;
using UnityEngine;

public class PassScript : MonoBehaviour
{
    public void onClick()
    {
        Player player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
        int val = GameManager.Instance.currentPlayer.Value;
        if (GameManager.Instance.allPlayerId[val] == player.PlayerId)
        {
            Debug.Log("Passing");
            player.callingPlayerPass(1, player.PlayerHandCount.Value, player.PlayerId);

        }
    }

}
