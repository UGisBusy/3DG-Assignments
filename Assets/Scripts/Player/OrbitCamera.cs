using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCamera : MonoBehaviour
{
    [SerializeField]
    Transform target;

    [SerializeField]
    Vector3 offset = new Vector3(0, 3, -4);

    [SerializeField]
    float xRot = 20;

    [SerializeField]
    float mouseSensitivity = 0.2f;

    public float yRot { get; private set; }
    public Quaternion PlanarRotation => Quaternion.Euler(0f, yRot, 0f);
    public Vector3 PlanarForward => PlanarRotation * Vector3.forward;
    public Vector3 PlanarRight => PlanarRotation * Vector3.right;

    private void Awake()
    {
        if (target == null)
            throw new System.NullReferenceException("orbit camera has no target");

        yRot = target.eulerAngles.y;
    }

    private void Update()
    {
        yRot += Mouse.current.delta.ReadValue().x * mouseSensitivity;
    }

    private void LateUpdate()
    {
        transform.position = target.position + PlanarRotation * offset;
        transform.rotation = Quaternion.Euler(xRot, yRot, 0f);
    }
}
