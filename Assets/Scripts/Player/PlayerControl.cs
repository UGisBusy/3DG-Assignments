using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public float Yaw { get; private set; }
    public float Pitch { get; private set; }
    private float mouseSensitivity = 0.2f;
    private float minPitch = -80f;
    private float maxPitch = 80f;

    InputAction rayPickAction;

    public void EnableRayPick()
    {
        rayPickAction.Enable();
    }

    public void DisableRayPick()
    {
        rayPickAction.Disable();
    }

    private void Start()
    {
        Yaw = transform.eulerAngles.y;
        rayPickAction = new InputAction(binding: "<Mouse>/leftButton");
        rayPickAction.performed += onRayPick;
    }

    private void Update()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;

        Yaw += mouseDelta.x;
        Pitch = Mathf.Clamp(Pitch - mouseDelta.y, minPitch, maxPitch);

        transform.localRotation = Quaternion.Euler(0f, Yaw, 0f);
    }

    private void OnDestroy()
    {
        rayPickAction.performed -= onRayPick;
    }

    private void onRayPick(InputAction.CallbackContext context)
    {
        // TODO
    }
}
