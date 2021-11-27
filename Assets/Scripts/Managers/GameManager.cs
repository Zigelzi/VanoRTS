using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    List<BuildingBase> playerBases = new List<BuildingBase>();

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        BuildingBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
        BuildingBase.ServerOnBaseDespawned += ServerHandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        BuildingBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
        BuildingBase.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
    }

    [Server]
    void ServerHandleBaseSpawned(BuildingBase spawnedBase)
    {
        playerBases.Add(spawnedBase);
    }

    [Server]
    void ServerHandleBaseDespawned(BuildingBase spawnedBase)
    {
        playerBases.Remove(spawnedBase);

        if (playerBases.Count <= 1)
        {
            Debug.Log("Game over!");
        }
    }

    #endregion
}
