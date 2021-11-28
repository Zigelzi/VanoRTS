using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class GameManager : NetworkBehaviour
{
    List<BuildingBase> playerBases = new List<BuildingBase>();

    public static event Action<string> ClientOnGameOver;

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
    void ServerHandleBaseDespawned(BuildingBase despawnedBase)
    {
        playerBases.Remove(despawnedBase);

        if (playerBases.Count <= 1)
        {
            int playerId = playerBases[0].connectionToClient.connectionId;
            string winnerName = $"Player {playerId}";
            RpcGameOver(winnerName);


        }
    }

    #endregion

    #region Client
    [ClientRpc]
    void RpcGameOver(string winnerName)
    {
        ClientOnGameOver?.Invoke(winnerName);
    }

    #endregion
}

