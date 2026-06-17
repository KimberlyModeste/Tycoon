using Unity.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkCard : NetworkBehaviour
{

    public NetworkVariable<int> spriteIndex = new NetworkVariable<int>();
    public NetworkVariable<FixedString64Bytes> cardName = new NetworkVariable<FixedString64Bytes>();
    public NetworkVariable<FixedString64Bytes> cardTag = new NetworkVariable<FixedString64Bytes>();
    public static List<Sprite> allSprites;


    private int pendingSpriteIndex;
    private FixedString32Bytes pendingName;
    private FixedString32Bytes pendingTag;

    public static void SetGlobalSpriteList(List<Sprite> sprites)
    {
        allSprites = sprites;
    }
    public override void OnNetworkSpawn()
    {
        // Apply values ONLY when the object has finished spawning
        if (!IsServer) return;

        spriteIndex.Value = pendingSpriteIndex;
        cardName.Value = pendingName;
        cardTag.Value = pendingTag;
    }

    public void InitCard(int sprite, string name, string tag)
    {
        pendingSpriteIndex = sprite;
        pendingName = name;
        pendingTag = tag;
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }
}
