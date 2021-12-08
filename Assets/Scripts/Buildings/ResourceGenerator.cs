using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ResourceGenerator : NetworkBehaviour
{
    [SerializeField] int goldGenerationAmount = 10;
    [SerializeField][Range(0, 60)] int goldGenerationInterval = 1;

    bool generationEnabled;
    PlayerBank bank;
    RtsNetworkPlayer player;

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        GameManager.ServerOnGameOver += ServerHandleGameOver;

        player = connectionToClient.identity.GetComponent<RtsNetworkPlayer>();
        bank = player.GetComponent<PlayerBank>();

        generationEnabled = true;
        StartCoroutine(GenerateResource());
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        GameManager.ServerOnGameOver -= ServerHandleGameOver;

        StopCoroutine(GenerateResource());
        generationEnabled = false;

    }

    void ServerHandleGameOver()
    {
        StopCoroutine(GenerateResource());
    }

    [Server]
    IEnumerator GenerateResource()
    {
        while (generationEnabled)
        {
            bank.AddGold(goldGenerationAmount);
            yield return new WaitForSeconds(goldGenerationInterval);
        }
        
    }
    #endregion

    #region Client


    #endregion
}
