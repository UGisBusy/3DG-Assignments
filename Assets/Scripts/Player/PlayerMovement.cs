using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private InputActionReference moveAction;

    [SerializeField]
    private OrbitCamera orbitCamera;

    float topSpeed = 10f;
    float acceleration = 40f;
    float stopThreshold = 0.15f;

    PlayerMotor motor;
    Vector3 moveVelocity;

    private void Awake()
    {
        if (orbitCamera == null)
            throw new System.NullReferenceException("player movement has no orbit camera");

        motor = GetComponent<PlayerMotor>();
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();

        // stop walking when this component is turned off
        moveVelocity = Vector3.zero;
        if (motor != null)
            motor.SetMoveVelocity(Vector3.zero);
    }

    private void FixedUpdate()
    {
        Vector2 rawInput = moveAction.action.ReadValue<Vector2>();
        bool hasInput = rawInput.magnitude > 0.0001f;

        Vector2 normalizedInput = Vector2.Normalize(rawInput);
        Vector3 moveDirection =
            orbitCamera.PlanarForward * normalizedInput.y
            + orbitCamera.PlanarRight * normalizedInput.x;

        Move(hasInput ? moveDirection : Vector3.zero);

        if (hasInput)
            motor.FaceTowards(moveDirection);
    }

    private void Move(Vector3 moveDirection)
    {
        Vector3 desiredVelocity = moveDirection * topSpeed;

        moveVelocity = Vector3.MoveTowards(
            moveVelocity,
            desiredVelocity,
            acceleration * Time.fixedDeltaTime
        );
        if (moveVelocity.magnitude < stopThreshold)
            moveVelocity = Vector3.zero;

        motor.SetMoveVelocity(moveVelocity);
    }
}
