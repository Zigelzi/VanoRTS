using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class UnitTargeting : NetworkBehaviour
{
    [SerializeField] Targetable target;
    public Targetable Target { get { return target; } }

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();

        GameManager.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        GameManager.ServerOnGameOver -= ServerHandleGameOver;
    }

    [Server]
    void ServerHandleGameOver()
    {
        ClearTarget();
    }

    [Command]
    public void CmdSetTarget(GameObject targetGameObject)
    {
        Targetable newTarget;

        if (targetGameObject.TryGetComponent<Targetable>(out newTarget))
        {
            target = newTarget;
        }
    }

    [Server]
    public void ClearTarget()
    {
        target = null;
    }
    #endregion
}
