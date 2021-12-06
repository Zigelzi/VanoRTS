using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using Mirror;
using System;

[RequireComponent(typeof(Health))]
public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject unitPrefab;
    [SerializeField] Transform unitSpawnPointPosition;
    [SerializeField] [Range(0, 10)] int maxQueueSize = 5;
    [SerializeField] float spawnMoveRange = 7f;
    [SerializeField] Queue<Unit> unitQueue = new Queue<Unit>();

    [SerializeField]
    [SyncVar(hook = nameof(ServerHandleQueueSizeUpdated))]
    int currentQueueSize = 0;

    RtsNetworkPlayer player;
    PlayerBank bank;

    public event Action<int> ServerOnUnitQueueSizeUpdated;
    public event Action<Unit> ServerOnUnitQueued;

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
        if (unitQueue.Count < maxQueueSize && bank.HasGold(queuedUnit.BuildingCost))
        {
            bank.ConsumeGold(queuedUnit.BuildingCost);
            unitQueue.Enqueue(queuedUnit);

            currentQueueSize = unitQueue.Count;
            ServerOnUnitQueued?.Invoke(queuedUnit);

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
        currentQueueSize = unitQueue.Count;

        MoveSpawnedUnit(spawnedUnitInstance);
    }

    [Server]
    void MoveSpawnedUnit(GameObject spawnedUnit)
    {
        UnitMovement spawnedUnitMovement = spawnedUnit.GetComponent<UnitMovement>();
        Vector3 spawnOffset = UnityEngine.Random.insideUnitSphere * spawnMoveRange;
        spawnOffset.y = spawnedUnit.transform.position.y; 
        Vector3 spawnDestination = spawnedUnit.transform.position + spawnOffset;

        spawnedUnitMovement.ServerMove(spawnDestination);
    }

    void ServerHandleQueueSizeUpdated(int oldQueueSize, int newQueueSize)
    {
        ServerOnUnitQueueSizeUpdated?.Invoke(newQueueSize);
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
