using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Obstacle : MonoBehaviour
{
    Rigidbody rb;

    public void Push(Vector3 force)
    {
        rb.AddForce(force, ForceMode.Impulse);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
}
