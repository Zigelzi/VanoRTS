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

    RtsNetworkPlayer player;
    PlayerBank bank;

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        player = NetworkClient.connection.identity.GetComponent<RtsNetworkPlayer>();
        bank = player.GetComponent<PlayerBank>();
    }

    [Command]
    void CmdSpawnUnit()
    {
        Unit spawnedUnit = unitPrefab.GetComponent<Unit>();
        if (bank.HasGold(spawnedUnit.BuildingCost))
        {
            bank.ConsumeGold(spawnedUnit.BuildingCost);
            GameObject spawnedUnitInstance = Instantiate(unitPrefab, unitSpawnPointPosition.position, Quaternion.identity);
            NetworkServer.Spawn(spawnedUnitInstance, connectionToClient);
        }
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
