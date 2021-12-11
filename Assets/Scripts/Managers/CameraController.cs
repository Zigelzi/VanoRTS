using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class CameraController : NetworkBehaviour
{
    [SerializeField] [Range(0, 50f)] float panSpeed = 30f;
    [SerializeField] [Range(0, 50f)] float zoomSpeed = 20f;

    float panBorderThickness = 40f; // In px
    Transform cameraTransform;
    Controls controls;
    Vector2 previousInput;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        cameraTransform = transform.Find("CineCamera").transform;
        cameraTransform.gameObject.SetActive(true);

        controls = new Controls();

        controls.Player.MoveCamera.performed += SetPreviousInput;
        controls.Player.MoveCamera.canceled += SetPreviousInput;

        controls.Enable();
    }

    [ClientCallback]
    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority || !Application.isFocused) { return; }

        UpdateCameraPosition();
    }

    void SetPreviousInput(InputAction.CallbackContext ctx)
    {
        previousInput = ctx.ReadValue<Vector2>();
    }

    void UpdateCameraPosition()
    {
        Vector3 currentPosition = cameraTransform.position;

        if (previousInput == Vector2.zero)
        {
            HandleMouseInput();
        }
        else
        {
            HandleKeyboardInput();
        }
    }

    void HandleMouseInput()
    {
        Vector3 cursorMovement = Vector3.zero;
        Vector3 cursorPosition = Mouse.current.position.ReadValue();

        if (MouseWithinBounds(cursorPosition))
        {
            MoveCameraPositionWithMouse(cursorPosition);
        }
    }

    bool MouseWithinBounds(Vector3 cursorPosition)
    {
        if (cursorPosition.y >= 0 &&
            cursorPosition.y <= Screen.height &&
            cursorPosition.x >= 0 &&
            cursorPosition.x <= Screen.width)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void MoveCameraPositionWithMouse(Vector3 cursorPosition)
    {
        Vector3 cameraPosition = cameraTransform.position;
        if (cursorPosition.y >= Screen.height - panBorderThickness)
        {
            cameraPosition.z -= panSpeed * Time.deltaTime;
        }

        if (cursorPosition.y <= panBorderThickness)
        {
            cameraPosition.z += panSpeed * Time.deltaTime;
        }

        if (cursorPosition.x >= Screen.width - panBorderThickness)
        {
            cameraPosition.x -= panSpeed * Time.deltaTime;
        }

        if (cursorPosition.x <= panBorderThickness)
        {
            cameraPosition.x += panSpeed * Time.deltaTime;
        }

        cameraTransform.position = cameraPosition;
    }

    void HandleKeyboardInput()
    {
        cameraTransform.position -= new Vector3(previousInput.x, 0f, previousInput.y) * panSpeed * Time.deltaTime;
    }

    Vector3 ClampCameraMovement(Vector3 cameraPosition)
    {
        // Play area isn't centered to 0,0 so take the offset into account when clamping the camera position
        float leftBoundary = 0f; // Limit on X axis
        float rightBoundary = 200f; // Limit on X axis
        float topBoundary = 200f; // Limit on Z axis
        float bottomBoundary = -50f; // Limit on Z axis
        Vector3 clampedCameraPosition = cameraPosition;

        clampedCameraPosition.x = Mathf.Clamp(cameraPosition.x, leftBoundary, rightBoundary);
        clampedCameraPosition.z = Mathf.Clamp(cameraPosition.z, bottomBoundary, topBoundary);

        return clampedCameraPosition;
    }
}
