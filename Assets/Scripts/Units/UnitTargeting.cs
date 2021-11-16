using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitTargeting : MonoBehaviour
{
    [SerializeField] LayerMask targetableLayer = new LayerMask();
    [SerializeField] Unit targetUnit;
    Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        HandleTargeting();
    }

    void HandleTargeting()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            TargetUnit();
        }
    }

    void TargetUnit()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        Unit clickedUnit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetableLayer))
        {
            clickedUnit = hit.transform.GetComponent<Unit>();

            if (clickedUnit == null) { 
                targetUnit = null;
            }
            else if (clickedUnit.IsTargetable && !clickedUnit.hasAuthority) {
                targetUnit = clickedUnit;
            }
            else
            {
                targetUnit = null;
            }
            
        }
    }
}
