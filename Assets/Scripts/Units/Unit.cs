using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
public class Unit : NetworkBehaviour
{
    [SerializeField] UnityEvent onSelected;
    [SerializeField] UnityEvent onDeselected;


    #region Client
    [Client]
    public void Select()
    {
        if(!hasAuthority) { return;  }
        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority) { return; }
        onDeselected?.Invoke();
    }
    #endregion
}
