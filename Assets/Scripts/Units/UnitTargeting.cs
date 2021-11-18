using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class UnitTargeting : NetworkBehaviour
{
    [SerializeField] Targetable target; 

    #region Server
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
