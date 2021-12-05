using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerBank : NetworkBehaviour
{
    [SerializeField] int startingGold = 100;
    [SerializeField] [SyncVar(hook = nameof(ClientHandleGoldUpdate))] int currentGold = 0;

    public int StartingGold { get { return startingGold; } }

    public event Action<int> ClientOnGoldUpdate;

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        currentGold = startingGold;
    }

    [Server]
    public bool HasGold(int goldAmount)
    {
        if (currentGold - goldAmount >= 0)
        {
            return true;
        }
        else 
        {
            return false;
        }
    }

    [Server]
    public void ConsumeGold(int goldAmount)
    {
        if (HasGold(goldAmount))
        {
            currentGold -= goldAmount;
        }
        else
        {
            Debug.Log($"Player doesn't have {goldAmount} to be consumed");
            Debug.Log($"Current gold: {currentGold}");
        }
    }

    [Server]
    public void AddGold(int goldAmount)
    {
        // Prevent reducing current gold by adding negative amount.
        if (goldAmount < 0) { return; }

        // TODO: Add maximum gold limit?
        currentGold += goldAmount;
    }

    #endregion

    #region Client
    void ClientHandleGoldUpdate(int oldGoldValue, int newGoldValue)
    {
        ClientOnGoldUpdate?.Invoke(newGoldValue);
    }
    #endregion
}
