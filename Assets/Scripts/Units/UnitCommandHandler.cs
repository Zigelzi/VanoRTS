using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandHandler : MonoBehaviour
{
    [SerializeField] LayerMask unitClickLayer;
    
    Camera mainCamera;
    UnitSelectionHandler unitSelectionHandler;

    // Start is called before the first frame update
    void Start()
    {
        unitSelectionHandler = GetComponent<UnitSelectionHandler>();
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseRightClick();
    }

    void HandleMouseRightClick()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Ray clickedPosition = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            TargetOrMove(clickedPosition);
            TryMove(clickedPosition);
        }
    }

    void TargetOrMove(Ray clickedPosition)
    {
        RaycastHit rayHit;
        Targetable target;
        if (Physics.Raycast(clickedPosition, out rayHit, Mathf.Infinity, unitClickLayer))
        {
            if (rayHit.collider.TryGetComponent<Targetable>(out target)) {
                if (target.hasAuthority)
                {
                    TryMove(clickedPosition);
                    return;
                }

                TryTarget(target);
                return;
            }
        }

        TryMove(clickedPosition);
    }

    void TryMove(Ray clickedPosition)
    {
        List<Unit> selectedUnits = unitSelectionHandler.SelectedUnits;
        foreach(Unit unit in selectedUnits)
        {
            unit.GetUnitMovement().CmdMove(clickedPosition);
        }
    }

    void TryTarget(Targetable target)
    {
        List<Unit> selectedUnits = unitSelectionHandler.SelectedUnits;
        foreach (Unit unit in selectedUnits)
        {
            unit.GetUnitTargeting().CmdSetTarget(target.gameObject);
        }
    }
}
