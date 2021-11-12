using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] LayerMask layerMask = new LayerMask();
    Camera mainCamera;
    List<Unit> selectedUnits = new List<Unit>();

    void Start()
    {
        mainCamera = Camera.main;    
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            ClearSelection();

        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            SelectAllUnitsInArea();
        }
    }

    void ClearSelection()
    {
        DeselectAllUnits();

        selectedUnits.Clear();
    }

    void DeselectAllUnits()
    {
        foreach (Unit unit in selectedUnits)
        {
            unit.Deselect();
        }
    }

    void SelectAllUnitsInArea()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            AddUnitToSelection(hit);
        }
    }

    void AddUnitToSelection(RaycastHit hit)
    {
        if (hit.collider.TryGetComponent<Unit>(out Unit hitUnit)) {
            if (!hitUnit.hasAuthority) { return; }
            
            selectedUnits.Add(hitUnit);
            SelectAllUnits();
        }
    }

    void SelectAllUnits()
    {
        foreach (Unit unit in selectedUnits)
        {
            unit.Select();
        }
    }

}
