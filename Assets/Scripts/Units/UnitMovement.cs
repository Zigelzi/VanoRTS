using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

using Mirror;
public class UnitMovement : NetworkBehaviour
{
    Camera mainCamera;
    NavMeshAgent navAgent;

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        navAgent = GetComponent<NavMeshAgent>();
    }

    [Command]
    public void CmdMove(Ray destination)
    {
        if (Physics.Raycast(destination, out RaycastHit rayHit, Mathf.Infinity))
        {
            navAgent.SetDestination(rayHit.point);
        }
    }

    #endregion

    #region Client

    #endregion
}
