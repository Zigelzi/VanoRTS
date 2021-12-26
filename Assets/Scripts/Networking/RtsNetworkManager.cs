using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System;

public class RtsNetworkManager : NetworkManager
{
    [SerializeField] GameObject playerBase;
    [SerializeField]GameManager gameManagerPrefab;

    public static event Action OnClientConnectToLobby;
    public static event Action OnClientDisconnectFromLobby;

    #region Server
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        RtsNetworkPlayer player = conn.identity.GetComponent<RtsNetworkPlayer>();
        Color playerColor = VanoUtilities.GenerateRandomColor();

        player.SetPlayerColor(playerColor);

        //GameObject playerBaseInstance = Instantiate(
        //    playerBase,
        //    conn.identity.transform.position, 
        //    conn.identity.transform.rotation);

        //NetworkServer.Spawn(playerBaseInstance, conn);
    }

    public override void OnServerSceneChanged(string newSceneName)
    {
        base.OnServerSceneChanged(newSceneName);

        if (IsMapScene())
        {
            GameManager instantiatedGameManager =  Instantiate(gameManagerPrefab);
            NetworkServer.Spawn(instantiatedGameManager.gameObject);
        }
    }

    bool IsMapScene()
    {
        string mapName = "Scene_Map";
        if (SceneManager.GetActiveScene().name.StartsWith(mapName))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region Client
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        OnClientConnectToLobby?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        OnClientDisconnectFromLobby?.Invoke();
    }

    #endregion
}
