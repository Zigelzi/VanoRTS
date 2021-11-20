using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] float damage = 10f;
    [SerializeField] float launchForce = 10f;
    [SerializeField] [Range(0, 10f)] float lifetime = 5f;

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
