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
    [SyncVar(hook = nameof(ClientHandleQueueSizeUpdated))]
    int currentQueueSize = 0;

    [SyncVar(hook = nameof(ClientHandleBuildingStarted))]
    bool isBuildingUnit = false;

    RtsNetworkPlayer player;
    PlayerBank bank;

    public event Action<int> ClientOnUnitQueueSizeUpdated;
    public event Action<Unit> ClientOnUnitBuildingStarted;

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        player = connectionToClient.identity.GetComponent<RtsNetworkPlayer>();
        bank = player.GetComponent<PlayerBank>();
    }

    [Command]
    void CmdQueueUnit()
    {
        Unit queuedUnit = unitPrefab.GetComponent<Unit>();
        if (unitQueue.Count < maxQueueSize && bank.HasGold(queuedUnit.BuildingCost))
        {
            bank.ConsumeGold(queuedUnit.BuildingCost);
            unitQueue.Enqueue(queuedUnit);

            currentQueueSize = unitQueue.Count;

            if (!isBuildingUnit)
            {
                StartCoroutine(StartBuildingUnit(queuedUnit));
            }
        }
    }

    [Server]
    IEnumerator StartBuildingUnit(Unit builtUnit)
    {
        isBuildingUnit = true;
        yield return new WaitForSeconds(builtUnit.BuildingTime);
        SpawnUnit();
    }

    [Server]
    void SpawnUnit()
    {
        Unit nextUnitInQueue;
        GameObject spawnedUnitInstance = Instantiate(unitPrefab, unitSpawnPointPosition.position, Quaternion.identity);
        NetworkServer.Spawn(spawnedUnitInstance, connectionToClient);
        unitQueue.Dequeue();
        currentQueueSize = unitQueue.Count;
        isBuildingUnit = false;

        MoveSpawnedUnit(spawnedUnitInstance);

        if (unitQueue.Count > 0)
        {
            nextUnitInQueue = unitQueue.Peek();
            StartCoroutine(StartBuildingUnit(nextUnitInQueue));
        }
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
    #endregion

    #region Client
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!hasAuthority) { return; }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            CmdQueueUnit();
        }
    }

    void ClientHandleQueueSizeUpdated(int oldQueueSize, int newQueueSize)
    {
        ClientOnUnitQueueSizeUpdated?.Invoke(newQueueSize);
    }

    void ClientHandleBuildingStarted(bool oldBuildingState, bool newBuildingState)
    {
        Unit builtUnit = unitPrefab.GetComponent<Unit>();
        if (newBuildingState)
        {
            ClientOnUnitBuildingStarted?.Invoke(builtUnit);
        }
    }
    #endregion
}
