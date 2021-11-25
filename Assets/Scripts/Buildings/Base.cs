using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class Base : NetworkBehaviour
{
    Health health;

    public static event Action<Base> ServerOnBaseSpawned;
    public static event Action<Base> ServerOnBaseDespawned;

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
    }

    #endregion

    #region Clien

    #endregion
}
