﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Mirror;
using UnityEngine.InputSystem;

public class UI_BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Building building;
    [SerializeField] LayerMask floorMask = new LayerMask();

    bool isPreviewing = false;
    Camera mainCamera;
    RtsNetworkPlayer player;
    GameObject buildingPreviewInstance;
    Renderer buildingRendererInstance;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RtsNetworkPlayer>();
        }

        if (building != null)
        {
            SetButtonSprite(building);
            SetButtonPrice(building);
        }
        if (isPreviewing)
        {
            MoveBuildingPreview();
        }
    }

    void SetButtonSprite(Building building)
    {
        Image buttonImage = GetComponent<Image>();
        Sprite buildingSprite = building.Icon;

        buttonImage.sprite = buildingSprite;
    }

    void SetButtonPrice(Building building)
    {
        TMP_Text buttonText = GetComponentInChildren<TMP_Text>();
        buttonText.text = building.Price.ToString();
    }

    void MoveBuildingPreview()
    {
        if (buildingPreviewInstance == null) { return; }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
        {
            if (!buildingPreviewInstance.activeSelf)
            {
                // Activate the building when user moves to cursor to valid floor
                buildingPreviewInstance.SetActive(true);
            }

            buildingPreviewInstance.transform.position = hit.point;
        }
        else
        {
            buildingPreviewInstance.SetActive(false);
        }

        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && !isPreviewing)
        {
            buildingPreviewInstance = Instantiate(building.BuildingPreview);
            buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();

            // Initially disable the building, so that preview isn't displayed in area where it can't be placed
            buildingPreviewInstance.SetActive(false);

            isPreviewing = true;
        }
        else if (eventData.button == PointerEventData.InputButton.Left && isPreviewing)
        {
            if (buildingPreviewInstance == null) { return; }

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
            {
                // Place building
                Debug.Log($"Built building to position {hit.point}");
            }

            Destroy(buildingPreviewInstance);

            isPreviewing = false;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

}
