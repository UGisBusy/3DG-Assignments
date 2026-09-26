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
    SphereCollider cameraCollider;
    Vector3 offset = new Vector3(0, 2, -4);
    float zoomScale = 1f;
    float zoomSpeed = 0.1f;
    float minZoomScale = 0.5f;
    float maxZoomScale = 2.5f;

    InputAction switchViewAction;
    InputAction zoomAction;

    private void Start()
    {
        playerControl = GetComponentInParent<PlayerControl>();
        cameraCollider = GetComponentInChildren<SphereCollider>();
        pivotTransform = transform.parent;

        Physics.IgnoreCollision(cameraCollider, playerControl.GetComponent<Collider>(), true);
        SwitchToFirstPerson();

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
            transform.localPosition = offset * zoomScale;
            FixCameraGround();
        }
    }

    private void SwitchToFirstPerson()
    {
        pivotTransform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
        cameraCollider.enabled = false;
        mode = Mode.FirstPerson;
    }

    private void SwitchToThirdPerson()
    {
        transform.localRotation = Quaternion.identity;
        cameraCollider.enabled = true;
        mode = Mode.ThirdPerson;
    }

    private void FixCameraGround()
    {
        if (mode != Mode.ThirdPerson)
            return;

        Vector3 worldOffset = pivotTransform.rotation * offset;
        float desiredDistance = offset.magnitude * zoomScale;

        if (worldOffset.y < 0)
        {
            float groundLimitedScale = -pivotTransform.position.y / worldOffset.y;
            desiredDistance = Mathf.Min(desiredDistance, groundLimitedScale * offset.magnitude);
        }

        zoomScale = desiredDistance / offset.magnitude;
        if (zoomScale < minZoomScale)
        {
            zoomScale = 1;
            SwitchToFirstPerson();
        }
    }

    private void FixCameraWall()
    {
        if (mode != Mode.ThirdPerson)
            return;

        Vector3 worldOffset = pivotTransform.rotation * offset;
        float desiredDistance = offset.magnitude * zoomScale;

        RaycastHit[] hits = Physics.SphereCastAll(pivotTransform.position, cameraCollider.radius, worldOffset.normalized, desiredDistance);
        foreach (RaycastHit hit in hits)
        {
            if (!hit.collider.CompareTag("Wall"))
                continue;
            desiredDistance = Mathf.Min(desiredDistance, Mathf.Max(0, hit.distance - 1));
        }

        zoomScale = desiredDistance / offset.magnitude;
        if (zoomScale < minZoomScale)
        {
            zoomScale = 1;
            SwitchToFirstPerson();
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
            SwitchToThirdPerson();
        else
            SwitchToFirstPerson();
    }

    private void OnZoom(InputAction.CallbackContext context)
    {
        if (mode != Mode.ThirdPerson)
            return;

        float scroll = context.ReadValue<float>();
        zoomScale = Mathf.Clamp(zoomScale - scroll * zoomSpeed, minZoomScale, maxZoomScale);

        FixCameraGround();
        FixCameraWall();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Wall"))
            return;

        FixCameraWall();
    }
}
