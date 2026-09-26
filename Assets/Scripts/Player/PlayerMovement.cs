using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;

    float topSpeed = 10f;
    float acceleration = 40f;
    float stopThreshold = 0.15f;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
    }

    private void FixedUpdate()
    {
        Vector2 rawInput = moveAction.action.ReadValue<Vector2>();

        float dt = Time.fixedDeltaTime;
        Vector3 currentHorizontal = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        Vector2 nomralizedInput = Vector2.Normalize(rawInput);

        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
        Vector3 inputDirection = forward * nomralizedInput.y + right * nomralizedInput.x;

        Vector3 desiredVelocity = rawInput.magnitude > 0.0001f ? (inputDirection * topSpeed) : Vector3.zero;
        Vector3 horizontalVelocity = Vector3.MoveTowards(currentHorizontal, desiredVelocity, acceleration * dt);
        if (horizontalVelocity.magnitude < stopThreshold)
            horizontalVelocity = Vector3.zero;

        rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
    }
}

