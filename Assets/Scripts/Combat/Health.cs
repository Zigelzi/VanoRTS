using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Health : NetworkBehaviour
{
    [SerializeField] int maxHealth = 100;
    [SerializeField] [SyncVar(hook = nameof(HandleHealthUpdate))] int currentHealth;

    public int MaxHealth { get { return maxHealth; } }

    public event Action ServerOnDie;
    public event Action<int, int> ClientOnHealthUpdate;

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        BuildingBase.ServerOnPlayerDefeat += ServerHandlePlayerDefeat;

        currentHealth = maxHealth;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        BuildingBase.ServerOnPlayerDefeat -= ServerHandlePlayerDefeat;
    }

    [Server]
    void ServerHandlePlayerDefeat(int playerConnectionId)
    {
        if (playerConnectionId == connectionToClient.connectionId)
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    [Server]
    public void TakeDamage(int damageAmount)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damageAmount;
        }
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    [Server]
    void Die()
    {
        ServerOnDie?.Invoke();
    }
    #endregion

    #region Client

    void HandleHealthUpdate(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdate?.Invoke(newHealth, maxHealth);
    }

    #endregion
}