using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UI_GameLobby : MonoBehaviour
{
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject gameLobbyPanel;
    [SerializeField] GameObject startGameButton;
    void Start()
    {
        mainMenuPanel = transform.parent.transform.Find("MainMenu").gameObject;
        gameLobbyPanel = transform.Find("Panel_Lobby").gameObject;

        RtsNetworkManager.OnClientConnectToLobby += HandleClientConnected;
        RtsNetworkPlayer.AuthorityOnPartyOwnerUpdated += AuthorityHandlePartyOwnerUpdated;
    }

    void OnDestroy()
    {
        RtsNetworkManager.OnClientConnectToLobby -= HandleClientConnected;
        RtsNetworkPlayer.AuthorityOnPartyOwnerUpdated -= AuthorityHandlePartyOwnerUpdated;
    }


    void HandleClientConnected()
    {
        gameLobbyPanel.SetActive(true);
    }

    void AuthorityHandlePartyOwnerUpdated(bool isPartyOwner)
    {
        startGameButton.SetActive(isPartyOwner);
    }

    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<RtsNetworkPlayer>().CmdStartGame();
    }
}
