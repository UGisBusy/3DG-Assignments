using UnityEngine;

public class PeriodicSlider : MonoBehaviour
{
    public Vector3 axis = Vector3.up;

    [Tooltip("Half of the travel distance.")]
    public float amplitude = 0.4f;

    [Tooltip("Cycles per second.")]
    public float frequency = 0.3f;

    Vector3 restPosition;

    void Awake()
    {
        restPosition = transform.localPosition;
    }

    void Update()
    {
        float offset = amplitude * Mathf.Sin(2f * Mathf.PI * frequency * Time.time);
        transform.localPosition = restPosition + axis.normalized * offset;
    }

    // Cyan line in the Scene view = travel range.
    void OnDrawGizmosSelected()
    {
        Vector3 rest = Application.isPlaying ? restPosition : transform.localPosition;
        Vector3 a = rest - axis.normalized * amplitude;
        Vector3 b = rest + axis.normalized * amplitude;
        if (transform.parent != null) { a = transform.parent.TransformPoint(a); b = transform.parent.TransformPoint(b); }
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(a, b);
    }
}
