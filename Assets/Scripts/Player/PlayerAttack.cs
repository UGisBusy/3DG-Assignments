using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Note to future:
// PlayerAttack turn off PlayerMovement, there may be more class want to do that.
// Move the toggle job to a central place to manager, other class simply request it
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private float range = 2f;

    [SerializeField]
    private float arcAngle = 120f;

    [SerializeField]
    private int damage = 1;

    [SerializeField]
    private float attackTime = 0.2f;

    OrbitCamera orbitCamera;
    PlayerMotor motor;
    PlayerMovement movement;
    InputAction attackAction;
    Vector3 attackDirection;
    bool isAttacking;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();

        motor = GetComponent<PlayerMotor>();

        Player player = GetComponent<Player>();
        orbitCamera = player.OrbitCamera;

        attackAction = new InputAction(name: "PlayerAttack", binding: "<Mouse>/leftButton");
        attackAction.performed += OnAttack;
    }

    private void OnEnable()
    {
        attackAction.Enable();
    }

    private void OnDisable()
    {
        attackAction.Disable();

        if (isAttacking)
        {
            isAttacking = false;
            movement?.Enable();
            StopAllCoroutines();
        }
    }

    private void OnDestroy()
    {
        attackAction.Dispose();
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (isAttacking)
            return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        movement?.Disable();

        attackDirection = orbitCamera.PlanarForward;
        motor.FaceTowards(attackDirection);
        Sweep();
        yield return new WaitForSeconds(attackTime);

        isAttacking = false;
        movement?.Enable();
    }

    private void Sweep()
    {
        Vector3 offset = new Vector3(0, -0.2f, 0);
        Vector3 origin = transform.position + offset;
        Collider[] hits = Physics.OverlapSphere(origin, range);
        HashSet<IDamageable> targets = new HashSet<IDamageable>();

        foreach (Collider c in hits)
        {
            // skip player
            if (c.transform.IsChildOf(transform))
                continue;

            IDamageable target = c.GetComponentInParent<IDamageable>();
            if (target == null)
                continue;

            if (!IsInArc(c.ClosestPoint(origin)))
                continue;

            if (!targets.Add(target))
                continue;

            target.GetDamaged(damage, gameObject);
        }
    }

    private bool IsInArc(Vector3 point)
    {
        Vector3 toTarget = point - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude < 0.0001f)
            return true;

        return Vector3.Angle(attackDirection, toTarget) <= arcAngle / 2f;
    }

    private void OnDrawGizmosSelected()
    {
        const int segments = 16;
        Vector3 offset = new Vector3(0, -0.2f, 0);
        Vector3 origin = transform.position + offset;
        Gizmos.color = Color.red;

        Vector3 previous = origin;
        for (int i = 0; i <= segments; i++)
        {
            float angle = -arcAngle / 2f + arcAngle * i / segments;
            Vector3 point = origin + Quaternion.Euler(0f, angle, 0f) * transform.forward * range;
            Gizmos.DrawLine(previous, point);
            previous = point;
        }
        Gizmos.DrawLine(previous, origin);
    }
}
