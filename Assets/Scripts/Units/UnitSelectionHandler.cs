using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] LayerMask layerMask = new LayerMask();
    Camera mainCamera;
    List<Unit> selectedUnits = new List<Unit>();

    public List<Unit> SelectedUnits { get { return selectedUnits; } }

    void Start()
    {
        mainCamera = Camera.main;    
    }

    void Update()
    {
        if (MultiSelectEnabled())
        {
            SelectAllUnitsInArea();
        }
        else if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            ResetSelection();

        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            SelectAllUnitsInArea();
        }
    }

    bool MultiSelectEnabled()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && Keyboard.current.shiftKey.isPressed)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void ResetSelection()
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
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
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
