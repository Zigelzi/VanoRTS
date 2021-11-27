using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

[RequireComponent(typeof(Health))]
public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject unitPrefab;
    
    Health health;
    Vector3 unitSpawnPointPosition;
    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();
        unitSpawnPointPosition = transform.Find("SpawnPoint_Unit").position;

        health = GetComponent<Health>();
        health.ServerOnDie += ServerHandleDeath;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        health.ServerOnDie -= ServerHandleDeath;
    }

    [Command]
    void CmdSpawnUnit()
    {

        GameObject spawnedUnit = Instantiate(unitPrefab, unitSpawnPointPosition, Quaternion.identity);
        NetworkServer.Spawn(spawnedUnit, connectionToClient);
    }

    [Server]
    void ServerHandleDeath()
    {
        // Commented for time being since base is same object as spawner
        //NetworkServer.Destroy(gameObject);
        
    }

    #endregion

    #region Client
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!hasAuthority) { return; }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            CmdSpawnUnit();
        }
    }
    #endregion
}
