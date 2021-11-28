using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class BuildingBase : NetworkBehaviour
{
    Health health;

    public static event Action<BuildingBase> ServerOnBaseSpawned;
    public static event Action<BuildingBase> ServerOnBaseDespawned;
    public static event Action<int> ServerOnPlayerDefeat;

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        health = GetComponent<Health>();

        health.ServerOnDie += ServerHandleBaseDestroyed;

        ServerOnBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        health.ServerOnDie -= ServerHandleBaseDestroyed;
        ServerOnBaseDespawned?.Invoke(this);
    }

    [Server]
    void ServerHandleBaseDestroyed()
    {
        NetworkServer.Destroy(gameObject);
        ServerOnPlayerDefeat?.Invoke(connectionToClient.connectionId);
    }

    #endregion

    #region Client

    #endregion
}
