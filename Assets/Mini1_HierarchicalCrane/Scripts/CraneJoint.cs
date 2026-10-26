using UnityEngine;
using UnityEngine.InputSystem;

public class CraneJoint : MonoBehaviour
{
    public enum Mode { Manual, AutoCW, AutoCCW, Swing }
    public Mode mode = Mode.Manual;
    public Vector3 axis = Vector3.up;
    public float speed = 30f;

    [Header("Manual mode")]
    public Key cwKey = Key.None;
    public Key ccwKey = Key.None;

    [Header("Limits")]
    public bool useLimits = false;
    public float minAngle = -30f;
    public float maxAngle = 30f;

    float angle;
    int swingDirection = 1;
    Quaternion restRotation;

    void Awake()
    {
        restRotation = transform.localRotation;
    }

    void Update()
    {
        switch (mode)
        {
            case Mode.Manual:
                angle += ManualInput() * speed * Time.deltaTime;
                break;

            case Mode.AutoCW:
                angle += speed * Time.deltaTime;
                break;

            case Mode.AutoCCW:
                angle -= speed * Time.deltaTime;
                break;

            case Mode.Swing:
                angle += swingDirection * speed * Time.deltaTime;
                if (angle >= maxAngle) { angle = maxAngle; swingDirection = -1; }
                if (angle <= minAngle) { angle = minAngle; swingDirection = 1; }
                break;
        }

        if (mode != Mode.Swing)
        {
            if (useLimits)
                angle = Mathf.Clamp(angle, minAngle, maxAngle);
            else
                angle = Mathf.Repeat(angle, 360f);
        }

        transform.localRotation = restRotation * Quaternion.AngleAxis(angle, axis);
    }

    // +1 while the CW key is held, -1 while the CCW key is held.
    float ManualInput()
    {
        var kb = Keyboard.current;
        if (kb == null) return 0f;

        float input = 0f;
        if (cwKey != Key.None && kb[cwKey].isPressed) input += 1f;
        if (ccwKey != Key.None && kb[ccwKey].isPressed) input -= 1f;
        return input;
    }

    // Yellow line in the Scene view = rotation axis.
    void OnDrawGizmosSelected()
    {
        Quaternion parent = transform.parent != null ? transform.parent.rotation : Quaternion.identity;
        Quaternion rest = Application.isPlaying ? restRotation : transform.localRotation;
        Vector3 dir = parent * rest * axis.normalized;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position - dir * 2f, transform.position + dir * 2f);
    }
}
