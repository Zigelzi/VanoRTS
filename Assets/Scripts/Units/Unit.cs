using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
public class Unit : NetworkBehaviour
{
    [SerializeField] UnityEvent onSelected;
    [SerializeField] UnityEvent onDeselected;
    UnitMovement unitMovement;

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        ServerOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        ServerOnUnitDespawned?.Invoke(this);
    }
    #endregion

    #region Client
    public override void OnStartClient()
    {
        base.OnStartClient();  

        unitMovement = GetComponent<UnitMovement>();

        if (isClientOnly || hasAuthority)
        {
            AuthorityOnUnitSpawned?.Invoke(this);
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if (isClientOnly || hasAuthority)
        {
            AuthorityOnUnitDespawned?.Invoke(this);
        }
    }

    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
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
