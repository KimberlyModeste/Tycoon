using Unity.Netcode;
using UnityEngine;

public class SelectButton : MonoBehaviour
{
   public void onClick()
    {
        Player player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
        int curplayIndex = GameManager.Instance.currentPlayer.Value;

        if (player == null) return;

        if (GameManager.Instance.allPlayerId[curplayIndex] == player.PlayerId)
        {
            Debug.Log($"Player has put in {player.getSelectedCount()} cards");
            if(player.getSelectedCount() >= 4)
            {
                Debug.Log("Player is in set revolution");
                GameManager.Instance.settingRevolutionServerRpc();
                //GameManager.Instance.setRevolutionClientRpc();
                //GameManager.Instance.revolution.Value = !GameManager.Instance.revolution.Value;
            }

            if (GameManager.Instance.currentCardAmount.Value == 0)
            {
                GameManager.Instance.setNextCardAmountServerRpc(player.getSelectedCount());
               
                player.addCardsToDropzone();
            }
            else if (player.getSelectedCount() == GameManager.Instance.currentCardAmount.Value)
            {
                player.addCardsToDropzone();
            }
        }
    }
}
