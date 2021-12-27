using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class UI_GameLobby : MonoBehaviour
{
    [SerializeField] GameObject gameLobbyPanel;
    [SerializeField] GameObject startGameButton;
    [SerializeField] GameObject playerPanels;

    TMP_Text[] playerNames;

    void Start()
    {
        gameLobbyPanel = transform.Find("Panel_Lobby").gameObject;
        GetPlayerNameText();

        RtsNetworkManager.OnClientConnectToLobby += HandleClientConnected;
        RtsNetworkPlayer.AuthorityOnPartyOwnerUpdated += AuthorityHandlePartyOwnerUpdated;
        RtsNetworkPlayer.ClientOnPlayerInfoUpdated += ClientHandlePlayerInfoUpdated;
    }

    void OnDestroy()
    {
        RtsNetworkManager.OnClientConnectToLobby -= HandleClientConnected;
        RtsNetworkPlayer.AuthorityOnPartyOwnerUpdated -= AuthorityHandlePartyOwnerUpdated;
        RtsNetworkPlayer.ClientOnPlayerInfoUpdated -= ClientHandlePlayerInfoUpdated;
    }

    void GetPlayerNameText()
    {
        playerPanels = gameLobbyPanel.transform.Find("Player_Panels").gameObject;
        playerNames = playerPanels.GetComponentsInChildren<TMP_Text>();
    }


    void HandleClientConnected()
    {
        gameLobbyPanel.SetActive(true);
    }

    void AuthorityHandlePartyOwnerUpdated(bool isPartyOwner)
    {
        startGameButton.SetActive(isPartyOwner);
    }

    void ClientHandlePlayerInfoUpdated()
    {
        RtsNetworkManager networkManager = (RtsNetworkManager)NetworkManager.singleton;
        List<RtsNetworkPlayer> players = networkManager.Players;

        UpdatePresentPlayerNames(players);
        UpdateEmptyPlayerNames(players);

        if (players.Count >= 2)
        {

        }

    }

    void UpdatePresentPlayerNames(List<RtsNetworkPlayer> players)
    {
        for (int i = 0; i < players.Count; i++)
        {
            playerNames[i].text = players[i].PlayerName;
        }
    }

    void UpdateEmptyPlayerNames(List<RtsNetworkPlayer> players)
    {
        for (int i = players.Count; i < playerNames.Length; i++)
        {
            playerNames[i].text = "Waiting for player...";
        }
    }

    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<RtsNetworkPlayer>().CmdStartGame();
    }
}
