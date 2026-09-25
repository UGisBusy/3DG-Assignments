using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    enum Mode
    {
        FirstPerson,
        ThirdPerson,
    }

    Mode mode;
    PlayerControl playerControl;
    Transform pivotTransform;
    Vector3 offset = new Vector3(0, 2, -4);
    float zoomScale = 1f;
    float zoomSpeed = 0.1f;
    float minZoomScale = 0.5f;
    float maxZoomScale = 2.5f;

    InputAction switchViewAction;
    InputAction zoomAction;

    private void Start()
    {
        mode = Mode.FirstPerson;
        playerControl = GetComponentInParent<PlayerControl>();
        pivotTransform = transform.parent;

        switchViewAction = new InputAction(binding: "<Keyboard>/leftShift");
        switchViewAction.performed += OnSwitchView;
        switchViewAction.Enable();

        zoomAction = new InputAction(binding: "<Mouse>/scroll/y");
        zoomAction.performed += OnZoom;
        zoomAction.Enable();
    }

    private void Update()
    {
        if (mode == Mode.FirstPerson)
        {
            transform.localRotation = Quaternion.Euler(playerControl.Pitch, 0f, 0f);
        }
        else
        {
            pivotTransform.localRotation = Quaternion.Euler(playerControl.Pitch, 0f, 0f);
        }
    }

    private void OnDestroy()
    {
        switchViewAction.performed -= OnSwitchView;
        switchViewAction.Disable();

        zoomAction.performed -= OnZoom;
        zoomAction.Disable();
    }

    private void OnSwitchView(InputAction.CallbackContext context)
    {
        if (mode == Mode.FirstPerson)
        {
            transform.localRotation = Quaternion.identity;
            transform.localPosition = offset * zoomScale;
            mode = Mode.ThirdPerson;
        }
        else
        {
            pivotTransform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;
            mode = Mode.FirstPerson;
        }
    }

    private void OnZoom(InputAction.CallbackContext context)
    {
        float scroll = context.ReadValue<float>();
        zoomScale = Mathf.Clamp(zoomScale - scroll * zoomSpeed, minZoomScale, maxZoomScale);

        if (mode == Mode.ThirdPerson)
        {
            transform.localPosition = offset * zoomScale;
        }
    }
}
