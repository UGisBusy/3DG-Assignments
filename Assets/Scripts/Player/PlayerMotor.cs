using UnityEngine;

// run after the components that send requests
[DefaultExecutionOrder(100)]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    float turnSpeed = 720f;

    [SerializeField]
    float edgeProbeDistance = 0.3f;

    const float PROBE_HEIGHT = 0.25f;

    Rigidbody rb;
    CapsuleCollider capsule;
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

        capsule = GetComponent<CapsuleCollider>();
        capsule.material = new PhysicsMaterial
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
        Vector3 velocity = BlockEdges(moveVelocity);
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    private Vector3 BlockEdges(Vector3 velocity)
    {
        if (!HasGroundAt(new Vector3(velocity.x, 0f, 0f)))
            velocity.x = 0f;
        if (!HasGroundAt(new Vector3(0f, 0f, velocity.z)))
            velocity.z = 0f;
        return velocity;
    }

    private bool HasGroundAt(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.0001f)
            return true;

        Vector3 feet = new Vector3(rb.position.x, capsule.bounds.min.y, rb.position.z);
        Vector3 origin =
            feet
            + Vector3.up * PROBE_HEIGHT
            + direction.normalized * (capsule.radius + edgeProbeDistance);

        return Physics.Raycast(
            origin,
            Vector3.down,
            PROBE_HEIGHT * 2f,
            ~0,
            QueryTriggerInteraction.Ignore
        );
    }

    private void ApplyFacing(float dt)
    {
        Quaternion desired = Quaternion.LookRotation(faceDirection, Vector3.up);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, desired, turnSpeed * dt));
    }

    private Vector3 Flatten(Vector3 v) => new Vector3(v.x, 0f, v.z);
}
