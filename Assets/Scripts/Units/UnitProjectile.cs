using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] float launchForce = 10f;
    [SerializeField] [Range(0, 10f)] float lifetime = 5f;
    [SerializeField] int damage = 10;

    Rigidbody projectileRb;
    // Start is called before the first frame update
    void Start()
    {
        projectileRb = GetComponent<Rigidbody>();
        LaunchProjectile();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        Invoke(nameof(DestroySelf), lifetime);
    }

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {

        if (IsEnemy(other) && other.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(damage);
            DestroySelf();
        }
    }

    [Server]
    bool IsEnemy(Collider other)
    {
        NetworkIdentity networkIdentity;
        if (other.TryGetComponent<NetworkIdentity>(out networkIdentity))
        {
            if (networkIdentity.connectionToClient != connectionToClient)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    void LaunchProjectile()
    {
        if (projectileRb == null) { return; }
        projectileRb.velocity = transform.forward * launchForce;
    }

    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
