using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RtsNetworkManager : NetworkManager
{
    [SerializeField] GameObject playerBase;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        GameObject playerBaseInstance = Instantiate(
            playerBase,
            conn.identity.transform.position, 
            conn.identity.transform.rotation);

        NetworkServer.Spawn(playerBaseInstance, conn);
    }
}
