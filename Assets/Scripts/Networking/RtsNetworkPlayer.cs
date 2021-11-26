using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RtsNetworkPlayer : NetworkBehaviour
{
    [SerializeField] List<Unit> playerUnits = new List<Unit>();
    public List<Unit> PlayerUnits { get { return playerUnits; } }

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
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        
        // If we're server, we're already subsribed for these events OnStartServer
        if (NetworkServer.active) { return; }
        
        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if (isClientOnly || hasAuthority)
        {
            Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        }
    }

    void AuthorityHandleUnitSpawned(Unit unit)
    { 
        playerUnits.Add(unit);
    }

    void AuthorityHandleUnitDespawned(Unit unit)
    {
        playerUnits.Remove(unit);
    }
    #endregion
}
