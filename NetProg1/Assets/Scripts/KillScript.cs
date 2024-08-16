using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KillScript : NetworkBehaviour
{
    public void Destroy()
    {
        Invoke( "DestroyInternal",1);
    }

    private void DestroyInternal()
    {
        GetComponent<NetworkObject>().Despawn();
    }
}
