using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
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
    float jumpSpeed = 10f;
    float groundCheckRadius = 0.2f;

    Rigidbody rb;
    CapsuleCollider capsule;

    public bool IsGrounded { get => CheckGrounded(); }

    InputAction switchMovementModeAction;
    InputAction jumpAction;

    private void Awake()
    {
        mode = Mode.slow;
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        capsule.material = new PhysicsMaterial
        {
            dynamicFriction = 0f,
            staticFriction = 0f,
            frictionCombine = PhysicsMaterialCombine.Minimum
        };

        switchMovementModeAction = new InputAction(binding: "<Keyboard>/space");
        switchMovementModeAction.performed += OnSwitchMode;

        jumpAction = new InputAction(binding: "<Keyboard>/f");
        jumpAction.performed += OnJump;
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        switchMovementModeAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        switchMovementModeAction.Disable();
        jumpAction.Disable();
    }

    private void OnDestroy()
    {
        switchMovementModeAction.performed -= OnSwitchMode;
        jumpAction.performed -= OnJump;
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

        float factor = (mode == Mode.slow) ? 1 : 2;

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

    private void OnJump(InputAction.CallbackContext context)
    {
        if (!IsGrounded)
            return;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpSpeed, rb.linearVelocity.z);
    }

    private bool CheckGrounded()
    {
        Vector3 feetPosition = capsule.bounds.center - new Vector3(0f, capsule.bounds.extents.y, 0f);
        Collider[] hits = Physics.OverlapSphere(feetPosition, groundCheckRadius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Ground") || hit.CompareTag("Wall") || hit.CompareTag("Target") || hit.CompareTag("Obstacle"))
                return true;
        }

        return false;
    }
}
