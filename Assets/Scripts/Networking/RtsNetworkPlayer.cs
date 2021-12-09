using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RtsNetworkPlayer : NetworkBehaviour
{
    [SerializeField] Building[] buildings = new Building[0];
    [SerializeField] List<Unit> playerUnits = new List<Unit>();
    [SerializeField] List<Building> playerBuildings = new List<Building>();
    [SerializeField] LayerMask notBuildableLayer = new LayerMask();

    private PlayerBank bank;

    public List<Unit> PlayerUnits { get { return playerUnits; } }
    public List<Building> PlayerBuildings { get { return playerBuildings; } }

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        bank = GetComponent<PlayerBank>();

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

    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 position)
    {
        GameObject selectedBuildingGameObject = FindBuilding(buildingId);
        Building selectedBuilding = selectedBuildingGameObject.GetComponent<Building>();

        if (selectedBuildingGameObject == null) { return; }

        // TODO: Check that there isn't other buildings in the area already
        if (bank.HasGold(selectedBuilding.Price) && IsPlaceablePosition(selectedBuilding, position))
        {
            bank.ConsumeGold(selectedBuilding.Price);
            GameObject builtBuilding = Instantiate(selectedBuildingGameObject, position, Quaternion.identity);
            NetworkServer.Spawn(builtBuilding, connectionToClient);
        }
    }

    bool IsPlaceablePosition(Building building, Vector3 position)
    {
        BoxCollider buildingCollider = building.GetComponent<BoxCollider>();
        Vector3 distanceToEdge = buildingCollider.size / 2;

        if (!Physics.CheckBox(position + buildingCollider.center, 
            distanceToEdge, 
            Quaternion.identity, 
            notBuildableLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    GameObject FindBuilding(int buildingId)
    {
        foreach (Building searchedBuilding in buildings)
        {
            if (searchedBuilding.BuildingId == buildingId)
            {
                return searchedBuilding.gameObject;
            }
        }

        return null;
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
