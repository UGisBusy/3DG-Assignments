using UnityEngine;

// run after the components that send requests
[DefaultExecutionOrder(100)]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    float turnSpeed = 720f; // degrees per second

    Rigidbody rb;
    Vector3 moveVelocity;
    Vector3 faceDirection;

    public void SetMoveVelocity(Vector3 velocity) => moveVelocity = Flatten(velocity);

    public void FaceTowards(Vector3 direction)
    {
        direction = Flatten(direction);
        if (direction.sqrMagnitude > 0.0001f)
            faceDirection = direction;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // rotation is only changed by code, and interpolation keeps the camera from jittering
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        GetComponent<CapsuleCollider>().material = new PhysicsMaterial
        {
            dynamicFriction = 0f,
            staticFriction = 0f,
            frictionCombine = PhysicsMaterialCombine.Minimum,
        };

        faceDirection = Flatten(transform.forward);
    }

    private void FixedUpdate()
    {
        ApplyVelocity();
        ApplyFacing(Time.fixedDeltaTime);
    }

    private void ApplyVelocity()
    {
        rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);
    }

    private void ApplyFacing(float dt)
    {
        Quaternion desired = Quaternion.LookRotation(faceDirection, Vector3.up);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, desired, turnSpeed * dt));
    }

    private Vector3 Flatten(Vector3 v) => new Vector3(v.x, 0f, v.z);
}
