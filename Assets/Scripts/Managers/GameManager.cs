using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    List<Base> playerBases = new List<Base>();

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        Base.ServerOnBaseSpawned += ServerHandleBaseSpawned;
        Base.ServerOnBaseDespawned += ServerHandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        Base.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
        Base.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
    }

    [Server]
    void ServerHandleBaseSpawned(Base spawnedBase)
    {
        playerBases.Add(spawnedBase);
    }

    [Server]
    void ServerHandleBaseDespawned(Base spawnedBase)
    {
        playerBases.Remove(spawnedBase);

        if (playerBases.Count <= 1)
        {
            Debug.Log("Game over!");
        }
    }

    #endregion
}
