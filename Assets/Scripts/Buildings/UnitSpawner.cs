using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject unitPrefab;
    Vector3 unitSpawnPointPosition;
    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();
        unitSpawnPointPosition = transform.Find("SpawnPoint_Unit").position;
    }

    [Command]
    void CmdSpawnUnit()
    {

        GameObject spawnedUnit = Instantiate(unitPrefab, unitSpawnPointPosition, Quaternion.identity);
        NetworkServer.Spawn(spawnedUnit, connectionToClient);
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
