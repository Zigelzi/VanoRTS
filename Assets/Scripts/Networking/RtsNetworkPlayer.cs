﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RtsNetworkPlayer : NetworkBehaviour
{
    [SerializeField] List<Unit> playerUnits = new List<Unit>();

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        // Subscribe on server for actions triggered by unit spawning or despawning
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        // Unsubscribe on server for actions triggered by unit spawning or despawning
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
    }

    void ServerHandleUnitSpawned(Unit unit)
    {
        if (IsPlayers(unit))
        {
            playerUnits.Add(unit);
        }
    }

    void ServerHandleUnitDespawned(Unit unit)
    {
        if (IsPlayers(unit))
        {
            playerUnits.Remove(unit);
        }
    }

    bool IsPlayers(Unit unit)
    {
        if (unit.connectionToClient.connectionId == connectionToClient.connectionId)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region Client
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isClientOnly)
        {
            Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if (isClientOnly)
        {
            Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        }
    }

    void AuthorityHandleUnitSpawned(Unit unit)
    { 
        if (!hasAuthority) { return; }

        playerUnits.Add(unit);
    }

    void AuthorityHandleUnitDespawned(Unit unit)
    {
        if (!hasAuthority) { return; }
        playerUnits.Remove(unit);
    }
    #endregion
}
