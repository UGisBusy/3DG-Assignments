using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;

    public bool HasBoomerang { get; private set; }

    public float Yaw { get; private set; }
    public float Pitch { get; private set; }
    private float mouseSensitivity = 0.2f;
    private float minPitch = -80f;
    private float maxPitch = 80f;

    InputAction rayPickAction;
    private float maxRayDistance = 50f;

    private float attackCooldown = 2f;

    public void EnableAttack()
    {
        HasBoomerang = true;
        rayPickAction.Enable();
    }

    public void DisableAttack()
    {
        HasBoomerang = false;
        rayPickAction.Disable();
        StopAllCoroutines();
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
        rayPickAction.performed += OnRayPick;
        rayPickAction.Disable();
        HasBoomerang = false;
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
        rayPickAction.performed -= OnRayPick;
    }

    private void OnRayPick(InputAction.CallbackContext context)
    {
        if (!HasBoomerang)
            return;

        Collider hitCollider = null;
        Transform cameraTransform = Camera.main.transform;
        float distance = maxRayDistance;
        float maxDistanceCamera = maxRayDistance * 1.5f + Vector3.Distance(cameraTransform.position, transform.position);

        RaycastHit[] hits = Physics.RaycastAll(cameraTransform.position, cameraTransform.forward, maxDistanceCamera);
        foreach (RaycastHit hit in hits)
        {
            if (Vector3.Dot(hit.collider.transform.position - transform.position, transform.forward) < 0)
                continue;

            if (Vector3.Distance(hit.collider.transform.position, transform.position) > distance)
                continue;

            if (!hit.collider.CompareTag("Target") && !hit.collider.CompareTag("Obstacle"))
                continue;

            hitCollider = hit.collider;
            distance = Vector3.Distance(hit.collider.transform.position, transform.position);
        }

        if (
            hitCollider == null ||
            !hitCollider.CompareTag("Target") ||
            hitCollider.GetComponent<Target>().IsBeingAttack
        )
        {
            // TODO: play error sound
            // Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 100, Color.yellow, 5f);
            return;
        }

        // Debug.DrawLine(cameraTransform.position, target.transform.position, Color.red, 5f);
        GameplayEvents.PlayerAttack?.Invoke(hitCollider.GetComponent<Target>());
        HasBoomerang = false;
        StartCoroutine(StartAttackCooldown());
    }

    private IEnumerator StartAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        HasBoomerang = true;
    }
}
