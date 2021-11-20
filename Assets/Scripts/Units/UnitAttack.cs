using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(UnitTargeting))]
[RequireComponent(typeof(UnitMovement))]
public class UnitAttack : NetworkBehaviour
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform projectileSpawnpoint;
    [SerializeField] [Range(0, 10f)] float fireRate = 1f;
    [SerializeField] [Range(0, 50f)] float turnRate = 10f;
    UnitTargeting targeting;
    UnitMovement unitMovement;

    float attackRange;
    float previousShotFiredTime;

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        targeting = GetComponent<UnitTargeting>();
        unitMovement = GetComponent<UnitMovement>();

        attackRange = unitMovement.ChaseRange + 2f;
    }

    [ServerCallback]
    void Update()
    {
        if (targeting.Target == null) { return; }

        if (IsInAttackRange())
        {
            FaceTarget();

            if (CanFireAgain())
            {
                SpawnProjectile();
            }
        }
        
    }

    [Server]
    bool IsInAttackRange()
    {
        Targetable target = targeting.Target;
        float distanceFromTargetSquared = (target.transform.position - transform.position).sqrMagnitude;
        float attackRangeSquared = attackRange * attackRange;

        if (distanceFromTargetSquared <= attackRangeSquared)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [Server]
    void FaceTarget()
    {
        Vector3 targetDirection = targeting.Target.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        float maxStepSize = turnRate * Time.deltaTime;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxStepSize);
    }

    [Server]
    bool CanFireAgain()
    {
        if (Time.time > (1 / fireRate) + previousShotFiredTime)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [Server]
    void SpawnProjectile()
    {
        Vector3 projectileDirection = targeting.Target.GetAimAtPoint().position - projectileSpawnpoint.position;
        Quaternion projectileRotation = Quaternion.LookRotation(projectileDirection);
        GameObject projectileInstance = Instantiate(projectilePrefab,
            projectileSpawnpoint.position,
            projectileRotation);

        NetworkServer.Spawn(projectileInstance, connectionToClient);

        previousShotFiredTime = Time.time;
    }

    #endregion
}
