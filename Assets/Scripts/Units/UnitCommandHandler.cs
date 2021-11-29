using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandHandler : MonoBehaviour
{
    [SerializeField] LayerMask clicableLayer;
    
    Camera mainCamera;
    UnitSelectionHandler unitSelectionHandler;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.ClientOnGameOver += ClientHandleGameOver;

        unitSelectionHandler = GetComponent<UnitSelectionHandler>();
        mainCamera = Camera.main;
    }

    void OnDestroy()
    {
        GameManager.ClientOnGameOver -= ClientHandleGameOver;
    }

    // Update is called once per frame
    void Update()
    {
        HandleStop();
        HandleMouseRightClick();
    }

    void HandleStop()
    {
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            StopMoving();
        }
    }

    void StopMoving()
    {
        List<Unit> selectedUnits = unitSelectionHandler.SelectedUnits;
        foreach (Unit unit in selectedUnits)
        {
            unit.GetUnitMovement().CmdStop();
        }
    }

    void HandleMouseRightClick()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Ray clickedPosition = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            TargetOrMoveTo(clickedPosition);
        }
    }

    void TargetOrMoveTo(Ray clickedPosition)
    {
        RaycastHit rayHit;
        Targetable target;
        if (Physics.Raycast(clickedPosition, out rayHit, Mathf.Infinity, clicableLayer))
        {
            if (rayHit.collider.TryGetComponent<Targetable>(out target)) {
                if (target.hasAuthority)
                {
                    // Clicked friendly unit and unit should only move to the clicked position
                    TryMove(clickedPosition);
                    return;
                }

                // Clicked enemy unit and unit should target it
                TryTarget(target);
                return;
            }
        }

        // Clicked ground and unit should only move to the clicked position
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

    void ClientHandleGameOver(string winnerName)
    {
        DisableCommandHandling();
    }

    void DisableCommandHandling()
    {
        enabled = false;
    }
}
