using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

[RequireComponent(typeof(Health))]
public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject unitPrefab;
    [SerializeField] Transform unitSpawnPointPosition;
    [SerializeField] [Range(0, 10)] int maxQueueSize = 5;
    [SerializeField] Queue<Unit> unitQueue = new Queue<Unit>();

    RtsNetworkPlayer player;
    PlayerBank bank;

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        player = NetworkClient.connection.identity.GetComponent<RtsNetworkPlayer>();
        bank = player.GetComponent<PlayerBank>();
    }

    [Server]
    void QueueUnit()
    {
        Unit queuedUnit = unitPrefab.GetComponent<Unit>();
        if (unitQueue.Count <= maxQueueSize && bank.HasGold(queuedUnit.BuildingCost))
        {
            unitQueue.Enqueue(queuedUnit);
            StartCoroutine(SpawnUnit(queuedUnit));
        }
    }

    [Server]
    IEnumerator SpawnUnit(Unit spawnedUnit)
    {
        yield return new WaitForSeconds(spawnedUnit.BuildingTime);
        CmdSpawnUnit();
    }

    [Command]
    void CmdSpawnUnit()
    {
        GameObject spawnedUnitInstance = Instantiate(unitPrefab, unitSpawnPointPosition.position, Quaternion.identity);
        NetworkServer.Spawn(spawnedUnitInstance, connectionToClient);
        unitQueue.Dequeue();  
    }

    #endregion

    #region Client
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!hasAuthority) { return; }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            QueueUnit();
        }
    }
    #endregion
}
