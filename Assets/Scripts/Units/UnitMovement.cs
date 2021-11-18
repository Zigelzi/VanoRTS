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
    UnitTargeting targeting;    

    #region Server

    [ServerCallback]
    void Update()
    {
        ResetPathAtDestination();
    }

    void ResetPathAtDestination()
    {
        if (navAgent.hasPath && navAgent.remainingDistance < navAgent.stoppingDistance)
        {
            navAgent.ResetPath();
        }
    }
    public override void OnStartServer()
    {
        base.OnStartServer();

        navAgent = GetComponent<NavMeshAgent>();
        targeting = GetComponent<UnitTargeting>();
    }

    [Command]
    public void CmdMove(Ray destination)
    {
        targeting.ClearTarget();

        if (Physics.Raycast(destination, out RaycastHit rayHit, Mathf.Infinity))
        {
            navAgent.SetDestination(rayHit.point);
        }
    }

    #endregion

    #region Client

    #endregion
}
