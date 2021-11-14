﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] LayerMask layerMask = new LayerMask();
    [SerializeField] RectTransform selectionAreaElement;

    Camera mainCamera;
    List<Unit> selectedUnits = new List<Unit>();
    RtsNetworkPlayer player;
    Vector2 selectionStartPosition;

    public List<Unit> SelectedUnits { get { return selectedUnits; } }

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RtsNetworkPlayer>();
        }
        HandleSelection();
    }

    void HandleSelection()
    {
        if (MultiSelectEnabled())
        {
            SelectUnits();
        }
        else if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Player starts selecting units
            StartSelection();

        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            // Player confirms the selection
            ConfirmSelection();
            
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            // Player is selecting units
            UpdateSelectionArea();
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

    void ConfirmSelection()
    {
        SelectUnits();
        HideSelectionArea();
    }

    void SelectUnits()
    {
        if (IsDragSelecting())
        {
            SelectUnitsInSelectionArea();
        }
        else
        {
            SelectSingleUnit();
        }
        
    }

    bool IsDragSelecting()
    {
        if (selectionAreaElement.sizeDelta.magnitude != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void SelectSingleUnit()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            AddUnitToSelection(hit);
        }
    }

    void SelectUnitsInSelectionArea()
    {
        foreach (Unit unit in player.PlayerUnits)
        {
            if (IsWithingSelectionArea(unit))
            {
                selectedUnits.Add(unit);
                unit.Select();
            }
        }
    }

    bool IsWithingSelectionArea(Unit unit)
    {
        Vector2 min = selectionAreaElement.anchoredPosition - (selectionAreaElement.sizeDelta / 2);
        Vector2 max = selectionAreaElement.anchoredPosition + (selectionAreaElement.sizeDelta / 2);
        Vector3 unitScreenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);
        if (unitScreenPosition.x > min.x &&
            unitScreenPosition.y > min.y &&
            unitScreenPosition.x < max.x &&
            unitScreenPosition.y < max.y)
        {
            return true;
        }
        else { 
            return false;
        }
    }

    void StartSelection()
    {
        DeselectAllUnits();
        DrawSelectionArea();
    }

    void DeselectAllUnits()
    {
        foreach (Unit unit in selectedUnits)
        {
            unit.Deselect();
        }
        selectedUnits.Clear();
    }

    void DrawSelectionArea()
    {
        if (selectionAreaElement == null) { return; }

        selectionAreaElement.gameObject.SetActive(true);
        selectionStartPosition = Mouse.current.position.ReadValue();

        UpdateSelectionArea();
    }

    void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float selectionAreaWidth = mousePosition.x - selectionStartPosition.x;
        float selectionAreaHeight = mousePosition.y - selectionStartPosition.y;

        Vector2 selectionAreaSize = new Vector2(selectionAreaWidth, selectionAreaHeight);
        Vector2 selectionAreaAbsoluteSize = new Vector2(Mathf.Abs(selectionAreaWidth), Mathf.Abs(selectionAreaHeight));
        Vector2 selectionCenter = selectionStartPosition + selectionAreaSize / 2;

        selectionAreaElement.sizeDelta = selectionAreaAbsoluteSize;
        selectionAreaElement.anchoredPosition = selectionCenter;
    }

    void HideSelectionArea()
    {
        if (selectionAreaElement == null) { return; }

        selectionAreaElement.gameObject.SetActive(false);
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
