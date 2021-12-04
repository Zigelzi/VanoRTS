using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class RtsNetworkBank : NetworkBehaviour
{
    [SerializeField] int startingGold = 100;
    [SerializeField] [SyncVar(hook = nameof(HandleGoldUpdate))] int currentGold = 0;

    public int StartingGold { get { return startingGold; } }

    public event Action<int> ClientOnGoldUpdate;

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        currentGold = startingGold;
    }



    #endregion

    #region Client
    void HandleGoldUpdate(int oldGoldValue, int newGoldValue)
    {
        ClientOnGoldUpdate?.Invoke(newGoldValue);
    }
    #endregion
}
