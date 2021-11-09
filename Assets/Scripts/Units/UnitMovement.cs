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
    void CmdMove(Ray destination)
    {
        if (Physics.Raycast(destination, out RaycastHit rayHit, Mathf.Infinity))
        {
            navAgent.SetDestination(rayHit.point);
        }
    }

   

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        
        mainCamera = Camera.main;
    }

    [ClientCallback]
    void Update()
    {
        if (!hasAuthority) { return; }

        HandleMovementCommand();
    }

    void HandleMovementCommand()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Ray destination = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            CmdMove(destination);
        }

    }



    #endregion
}
