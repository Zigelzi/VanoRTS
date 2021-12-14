using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Mirror;
using Cinemachine;

public class UI_MinimapController : MonoBehaviour, IPointerClickHandler, IDragHandler
{
    [SerializeField] float mapScale = 25f;
    [SerializeField] Vector3 cameraOffset = new Vector3(0f, 0f, -35f);

    RectTransform minimap;
    Transform playerCameraTransform;
    

    void Start()
    {
        minimap = transform.Find("RawImage_Minimap_Map").GetComponent<RectTransform>();    
    }

    void Update()
    {
        if (playerCameraTransform == null && NetworkClient.connection.identity != null)
        {
            playerCameraTransform = NetworkClient.connection.identity.GetComponentInChildren<CinemachineVirtualCamera>().transform;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        MoveCamera();
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveCamera();
    }

    void MoveCamera()
    {
        Vector3 newCameraPosition;
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        bool isWithinMinimap = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            minimap,
            mousePosition,
            null,
            out Vector2 localPoint
            );

        Vector2 minimapClickPosition = GetClickPositionOnMinimap(localPoint);


        if (isWithinMinimap)
        {
            newCameraPosition = new Vector3(
                Mathf.Lerp(-mapScale, mapScale, minimapClickPosition.x),
                playerCameraTransform.position.y,
                Mathf.Lerp(-mapScale, mapScale, minimapClickPosition.y)
                );

            playerCameraTransform.position = newCameraPosition + cameraOffset;
        }
        
    }

    Vector2 GetClickPositionOnMinimap(Vector2 localPoint)
    {
        float minimapWidthRatio = (localPoint.x - minimap.rect.x) / minimap.rect.width;
        float minimapHeightRatio = (localPoint.y - minimap.rect.y) / minimap.rect.height;

        Vector2 clickPositionOnMinimap = new Vector2(minimapWidthRatio, minimapHeightRatio);

        return clickPositionOnMinimap;
    }

    
}
