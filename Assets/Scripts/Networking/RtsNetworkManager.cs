using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System;

public class RtsNetworkManager : NetworkManager
{
    [SerializeField] GameManager gameManagerPrefab;
    [SerializeField] GameObject playerBasePrefab;
    [SerializeField] List<RtsNetworkPlayer> players = new List<RtsNetworkPlayer>();
    bool isGameInProgress = false;
    string mapName = "Scene_Map";

    public List<RtsNetworkPlayer> Players { get { return players; } }

    public static event Action OnClientConnectToLobby;
    public static event Action OnClientDisconnectFromLobby;

    #region Server
    public override void OnStopServer()
    {
        Players.Clear();

        isGameInProgress = false;
    }

    public void StartGame()
    {
        if (players.Count >= 2)
        {
            isGameInProgress = true;
            ServerChangeScene(mapName);
        }
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        if (!isGameInProgress) { return; }

        conn.Disconnect();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        RtsNetworkPlayer disconnectedPlayer = conn.identity.GetComponent<RtsNetworkPlayer>();
        players.Remove(disconnectedPlayer);

        base.OnServerDisconnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        RtsNetworkPlayer player = conn.identity.GetComponent<RtsNetworkPlayer>();
        players.Add(player);

        Color playerColor = VanoUtilities.GenerateRandomColor();
        player.SetPlayerColor(playerColor);

        if (players.Count == 1)
        {
            player.SetPartyOwner(true);
        }
        
    }

    public override void OnServerSceneChanged(string newSceneName)
    {
        base.OnServerSceneChanged(newSceneName);

        if (IsMapScene())
        {
            GameManager instantiatedGameManager =  Instantiate(gameManagerPrefab);
            NetworkServer.Spawn(instantiatedGameManager.gameObject);

            SpawnPlayers();
        }
    }

    bool IsMapScene()
    {
        if (SceneManager.GetActiveScene().name.StartsWith(mapName))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void SpawnPlayers()
    {
        foreach (RtsNetworkPlayer player in players)
        {
            GameObject playerBaseInstance = Instantiate(
            playerBasePrefab,
            GetStartPosition().position,
            Quaternion.identity);

            NetworkServer.Spawn(playerBaseInstance, player.connectionToClient);
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

    public override void OnStopClient()
    {
        base.OnStopClient();

        players.Clear();
    }

    #endregion
}
