using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ResourceGenerator : NetworkBehaviour
{
    [SerializeField] int goldGenerationAmount = 10;
    [SerializeField][Range(0, 60)] int goldGenerationInterval = 1;

    PlayerBank bank;
    RtsNetworkPlayer player;

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        StartCoroutine(GenerateResource());
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        StopCoroutine(GenerateResource());
    }

    void Update()
    {
        if (player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RtsNetworkPlayer>();
            bank = player.GetComponent<PlayerBank>();
        }
    }

    [Server]
    IEnumerator GenerateResource()
    {
        bank.AddGold(goldGenerationAmount);
        yield return new WaitForSeconds(goldGenerationInterval);
    }
    #endregion
}
