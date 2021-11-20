using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Mirror;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] [Range(0, 50f)] float turnSpeed = 20f;
    Camera mainCamera;
    NavMeshAgent navAgent;
    UnitTargeting targeting;    

    #region Server

    [ServerCallback]
    void Update()
    {
        
        if (targeting.Target != null)
        {
            ChaseTarget();
        }
        ResetPathAtDestination();
    }

    void ChaseTarget()
    {
        if (!IsInAttackRange())
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

    bool IsInAttackRange()
    {
        Targetable target = targeting.Target;
        float distanceFromTargetSquared = (target.transform.position - transform.position).sqrMagnitude;
        float attackRangeSquared = targeting.AttackRange * targeting.AttackRange;

        if (distanceFromTargetSquared <= attackRangeSquared)
        {
            return true;
        }
        else
        {
            return false;
        }
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

    #region Client

    #endregion
}
