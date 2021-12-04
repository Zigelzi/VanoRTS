using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RtsNetworkPlayer : NetworkBehaviour
{
    [SerializeField] List<Unit> playerUnits = new List<Unit>();
    [SerializeField] List<Building> playerBuildings = new List<Building>();
    public List<Unit> PlayerUnits { get { return playerUnits; } }
    public List<Building> PlayerBuildings { get { return playerBuildings; } }

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        // Subscribe on server for actions triggered by unit spawning or despawning
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;

        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        // Unsubscribe on server for actions triggered by unit spawning or despawning
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;

        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
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

    void ServerHandleBuildingSpawned(Building building)
    {
        if (IsPlayers(building))
        {
            playerBuildings.Add(building);
        }
    }

    void ServerHandleBuildingDespawned(Building building)
    {
        if (IsPlayers(building))
        {
            playerBuildings.Remove(building);
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

    bool IsPlayers(Building building)
    {
        if (building.connectionToClient.connectionId == connectionToClient.connectionId)
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

        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if (isClientOnly || hasAuthority)
        {
            Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;

            Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
            Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
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

    void AuthorityHandleBuildingSpawned(Building building)
    {
        playerBuildings.Add(building);
    }

    void AuthorityHandleBuildingDespawned(Building building)
    {
        playerBuildings.Remove(building);
    }
    #endregion
}
