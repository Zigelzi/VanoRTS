using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Health : NetworkBehaviour
{
    [SerializeField] int maxHealth = 100;
    [SerializeField] [SyncVar] int currentHealth;

    public static event Action ServerOnDie;
    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        currentHealth = maxHealth;
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
        NetworkServer.Destroy(gameObject);
        ServerOnDie?.Invoke();
    }
    #endregion

    #region Client

    #endregion
}