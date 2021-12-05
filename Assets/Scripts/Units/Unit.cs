using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
public class Unit : NetworkBehaviour
{
    [SerializeField] bool isTargetable = true;
    [SerializeField] int buildingCost = 30;
    [SerializeField] UnityEvent onSelected;
    [SerializeField] UnityEvent onDeselected;

    Health health;
    UnitMovement unitMovement;
    UnitTargeting unitTargeting;
    
    public bool IsTargetable { get { return isTargetable; } }
    public int BuildingCost { get { return buildingCost; } }

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        ServerOnUnitSpawned?.Invoke(this);
        health = GetComponent<Health>();

        health.ServerOnDie += ServerHandleDeath;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        ServerOnUnitDespawned?.Invoke(this);

        health.ServerOnDie -= ServerHandleDeath;
    }

    [Server]
    void ServerHandleDeath()
    {
        NetworkServer.Destroy(gameObject);
    }
    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        unitMovement = GetComponent<UnitMovement>();
        unitTargeting = GetComponent<UnitTargeting>();

        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if (hasAuthority)
        {
            AuthorityOnUnitDespawned?.Invoke(this);
        }
    }

    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    public UnitTargeting GetUnitTargeting()
    {
        return unitTargeting;
    }

    [Client]
    public void Select()
    {
        if(!hasAuthority) { return;  }
        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority) { return; }
        onDeselected?.Invoke();
    }
    #endregion
}
