using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TeamColorManager : NetworkBehaviour
{
    [SerializeField] Renderer[] colorRenderers = new Renderer[0];

    [SyncVar (hook = nameof(HandleTeamColorUpdated)) ] 
    Color playerColor;

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        RtsNetworkPlayer player = connectionToClient.identity.GetComponent<RtsNetworkPlayer>();
        playerColor = player.PlayerColor;
    }

    #endregion

    #region Client
    void HandleTeamColorUpdated(Color oldColor, Color newColor)
    {
        foreach (Renderer renderer in colorRenderers)
        {
            renderer.material.SetColor("_BaseColor", newColor);
        }
    }

    #endregion
}
