using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Mirror;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] [Range(0, 30f)] float chaseRange = 10f;
    NavMeshAgent navAgent;
    UnitTargeting targeting;    

    public float ChaseRange { get { return chaseRange; } }

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        navAgent = GetComponent<NavMeshAgent>();
        targeting = GetComponent<UnitTargeting>();

        GameManager.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        GameManager.ServerOnGameOver -= ServerHandleGameOver;
    }

    [ServerCallback]
    void Update()
    {
        
        if (targeting.Target != null)
        {
            ChaseTarget();
        }
        ResetPathAtDestination();
    }

    [Server]
    void ServerHandleGameOver()
    {
        navAgent.ResetPath();
    }

    void ChaseTarget()
    {
        if (!IsInChaseRange())
        {
            navAgent.SetDestination(targeting.Target.transform.position);
        }
        else if (navAgent.hasPath)
        {
            navAgent.ResetPath();
            
        }
        else
        {
            transform.LookAt(targeting.Target.transform.position);
        }
    }

    [Server]
    bool IsInChaseRange()
    {
        Targetable target = targeting.Target;
        float distanceFromTargetSquared = (target.transform.position - transform.position).sqrMagnitude;
        float chaseRangeSquared = chaseRange * chaseRange;

        if (distanceFromTargetSquared <= chaseRangeSquared)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [Server]
    void ResetPathAtDestination()
    {
        if (navAgent.hasPath && navAgent.remainingDistance < navAgent.stoppingDistance)
        {
            navAgent.ResetPath();
        }
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

    [Command]
    public void CmdStop()
    {
        targeting.ClearTarget();

        if (navAgent.hasPath)
        {
            navAgent.ResetPath();
        }
    }

    #endregion
}
