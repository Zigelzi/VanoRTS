using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class UI_JoinGameMenu : MonoBehaviour
{
    [SerializeField] TMP_InputField ipInputField;
    [SerializeField] Button connectButton;

    // Start is called before the first frame update
    void OnEnable()
    {
        RtsNetworkManager.OnClientConnectToLobby += HandleClientConnected;
        RtsNetworkManager.OnClientDisconnectFromLobby += HandleClientDisconnected;
    }

    void OnDisable()
    {
        RtsNetworkManager.OnClientConnectToLobby -= HandleClientConnected;
        RtsNetworkManager.OnClientDisconnectFromLobby -= HandleClientDisconnected;
    }

    public void JoinGame()
    {
        ipInputField = GetComponentInChildren<TMP_InputField>();
        string ipAddress = ipInputField.text;

        NetworkManager.singleton.networkAddress = ipAddress;
        NetworkManager.singleton.StartClient();

        connectButton.interactable = false;
    }

    void HandleClientConnected()
    {
        connectButton.interactable = true;

        gameObject.SetActive(false);
    }

    void HandleClientDisconnected()
    {
        connectButton.interactable = true;
    }
}
