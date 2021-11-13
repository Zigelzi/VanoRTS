using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandHandler : MonoBehaviour
{
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
        HandleMovementCommand();
    }

    void HandleMovementCommand()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Ray destination = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            TryMove(destination);
        }
    }

    void TryMove(Ray destination)
    {
        List<Unit> selectedUnits = unitSelectionHandler.SelectedUnits;
        foreach(Unit unit in selectedUnits)
        {
            unit.GetUnitMovement().CmdMove(destination);
        }
    }
}
