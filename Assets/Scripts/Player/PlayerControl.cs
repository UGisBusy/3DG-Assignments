using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;

    public float Yaw { get; private set; }
    public float Pitch { get; private set; }
    private float mouseSensitivity = 0.2f;
    private float minPitch = -80f;
    private float maxPitch = 80f;

    InputAction rayPickAction;
    private float maxRayDistance = 100f;

    public void EnableRayPick()
    {
        rayPickAction.Enable();
    }

    public void DisableRayPick()
    {
        rayPickAction.Disable();
    }

    private void Awake()
    {
        if (cameraPivot == null)
            throw new System.NullReferenceException("no camera pivot on player");
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
        Transform cameraTransform = Camera.main.transform;
        Target target = null;
        float distance = maxRayDistance;
        float maxDistanceCamera = maxRayDistance + Vector3.Distance(cameraTransform.position, transform.position) * 2;

        RaycastHit[] hits = Physics.RaycastAll(cameraTransform.position, cameraTransform.forward, maxDistanceCamera);
        foreach (RaycastHit hit in hits)
        {
            if (Vector3.Dot(hit.collider.transform.position - transform.position, transform.forward) < 0)
                continue;

            if (Vector3.Distance(hit.collider.transform.position, transform.position) > distance)
                continue;

            if (hit.collider.CompareTag("Target"))
            {
                target = hit.collider.GetComponent<Target>();
                if (target.IsBeingAttack)
                    continue;

                distance = Vector3.Distance(hit.collider.transform.position, transform.position);
            }
        }

        if (target == null)
        {
            // TODO: play error sound
            // Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 100, Color.yellow, 5f);
            return;
        }

        // Debug.DrawLine(cameraTransform.position, target.transform.position, Color.red, 5f);
        throwBoomerangAt(target);
    }

    private void throwBoomerangAt(Target target)
    {
        // TODO
        target.BeTargeted();
    }
}
