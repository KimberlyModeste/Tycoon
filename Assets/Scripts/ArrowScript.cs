using Unity.Netcode;
using UnityEngine;

public class ArrowScript : NetworkBehaviour
{
    public float floatAmplitude = 1f;
    public float floatSpeed = 1f;

    private bool floatX = false;

    private Vector3 targetPos;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
       
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }
   
    public void setInital()
    {
        transform.localPosition = new Vector3(-640, -230, 180);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        targetPos = new Vector3(-640, -245, 0);
        floatX = false;
    }

    public void SetTurnWithFutureCurrent(ulong temp)
    {
        //ulong pos = NetworkManager.Singleton.LocalClient.ClientId;
        //int index = GameManager.Instance.allPlayerId.IndexOf(pos);
        //ulong target = temp
    }

 
    public void SetTurn(int temp)
    {
        ulong playerId = NetworkManager.Singleton.LocalClient.ClientId;
        int index = GameManager.Instance.allPlayerId.IndexOf(playerId);

        ulong target = GameManager.Instance.allPlayerId[temp];

        int ePos = (GameManager.Instance.allPlayerId.IndexOf(target) - index + 4) % 4;

        if (index == -1)
            ePos = 0;

        switch (ePos)
        {
            case 0:
                // Player(-640, -230--260) rot 180
                transform.localPosition = new Vector3(-640, -230, 180);
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                targetPos = new Vector3(-640, -245, 0);
                floatX = false;
                //floatY = true;
                break;

            case 1:
                // Right(660 - 680, -260) rot - 90
                transform.localPosition = new Vector3(660, -260, 0);
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
                targetPos = new Vector3(670, -260, 0);
                floatX = true;
                break;

            case 2:
                // Up(640, 200-260/230-260) rot = 0
                transform.localPosition = new Vector3(640, 230, 0);
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                targetPos = new Vector3(640, 245, 0);
                floatX = false;
                break;

            case 3:
                // Left(-660 - -680, 260) rot 90
                transform.localPosition = new Vector3(-660, 260, 0);
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                targetPos = new Vector3(-670, 260, 0);
                floatX = true;
                break;

            default:
                Debug.Log("Arrow case went to default.");
                break;



        }

    }





    public void Update()
    {
        //transform.position = new Vector3(Mathf.PingPong(Time.time, 3), transform.position.y, transform.position.z)
        //transform.localPosition = new Vector3(Mathf.PingPong(Time.t))


        //transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * 10f); //Time.deltaTime * 10f);


        //float offset = Mathf.Sin(Time.time * 2f) * 10f;

        //if(floatX)
        //{
        //    transform.localPosition = targetPos + new Vector3(offset, 0, 0);
        //}
        //else
        //{
        //    transform.localPosition = targetPos + new Vector3(0, offset, 0);
        //}

    }
}
