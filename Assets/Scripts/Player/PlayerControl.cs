using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public float Yaw { get; private set; }
    public float Pitch { get; private set; }
    private float mouseSensitivity = 0.2f;
    private float minPitch = -80f;
    private float maxPitch = 80f;

    private void Start()
    {
        Yaw = transform.eulerAngles.y;
    }

    private void Update()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;

        Yaw += mouseDelta.x;
        Pitch = Mathf.Clamp(Pitch - mouseDelta.y, minPitch, maxPitch);

        transform.localRotation = Quaternion.Euler(0f, Yaw, 0f);
    }
}
