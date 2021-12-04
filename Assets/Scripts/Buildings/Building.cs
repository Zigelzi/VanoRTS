using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : NetworkBehaviour
{
    [SerializeField] Sprite icon;
    [SerializeField] int price = 100;
    [SerializeField] int buildingId = -1;
    [SerializeField] GameObject buildingPreview;

    public int BuildingId { get { return buildingId; } }
    public Sprite Icon { get { return icon; } }
    public int Price { get { return price; } }

    public static event Action<Building> ServerOnBuildingSpawned;
    public static event Action<Building> ServerOnBuildingDespawned;

    public static event Action<Building> AuthorityOnBuildingSpawned;
    public static event Action<Building> AuthorityOnBuildingDespawned;

    public GameObject BuildingPreview { get { return buildingPreview; } }

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        ServerOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        base.OnStartServer();

        ServerOnBuildingDespawned?.Invoke(this);
    }

    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        AuthorityOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopAuthority()
    {
        base.OnStartAuthority();

        AuthorityOnBuildingDespawned?.Invoke(this);
    }


    #endregion
}
