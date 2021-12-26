using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UI_GameLobby : MonoBehaviour
{
    [SerializeField] GameObject gameLobbyPanel;
    void Start()
    {
        gameLobbyPanel = transform.Find("Panel_Lobby").gameObject;
        RtsNetworkManager.OnClientConnectToLobby += HandleClientConnected;
    }

    void OnDestroy()
    {
        RtsNetworkManager.OnClientConnectToLobby -= HandleClientConnected;
    }

    void HandleClientConnected()
    {
        gameLobbyPanel.SetActive(true);
    }
}
