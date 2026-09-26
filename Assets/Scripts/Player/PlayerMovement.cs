using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;

    enum Mode
    {
        slow,
        fast
    }
    Mode mode;
    float topSpeed = 10f;
    float acceleration = 40f;
    float stopThreshold = 0.15f;

    Rigidbody rb;

    InputAction switchMovementModeAction;

    private void Start()
    {
        mode = Mode.slow;
        rb = GetComponent<Rigidbody>();

        switchMovementModeAction = new InputAction(binding: "<Keyboard>/space");
        switchMovementModeAction.performed += OnSwitchMode;
        switchMovementModeAction.Enable();

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

        float factor = (mode == Mode.slow) ? 1 : 3;

        Vector3 desiredVelocity = rawInput.magnitude > 0.0001f ? (inputDirection * topSpeed * factor) : Vector3.zero;
        Vector3 horizontalVelocity = Vector3.MoveTowards(currentHorizontal, desiredVelocity, acceleration * dt * factor);
        if (horizontalVelocity.magnitude < stopThreshold)
            horizontalVelocity = Vector3.zero;

        rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
    }

    private void OnSwitchMode(InputAction.CallbackContext context)
    {
        if (mode == Mode.slow)
            mode = Mode.fast;
        else
            mode = Mode.slow;
    }
}

