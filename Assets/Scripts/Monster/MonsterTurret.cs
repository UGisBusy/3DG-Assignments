using System.Collections;
using UnityEngine;

public class MonsterTurret : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private MonsterProjectile projectilePrefab;

    [SerializeField]
    private float cooldown = 2f;

    [SerializeField]
    private float projectileSpeed = 8f;

    [SerializeField]
    private float projectileLifetime = 4f;

    [SerializeField]
    private float aimHeight = 1.15f;

    [SerializeField]
    private bool isTracking = false;

    bool isShooting = false;

    private void Awake()
    {
        if (projectilePrefab == null)
            throw new System.NullReferenceException("Turret has no ProjectilePrefab");

        if (target == null)
            throw new System.NullReferenceException("Turret has no Target");
    }

    private void Update()
    {
        UpdateTracking();
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    public void StartTracking() => isTracking = true;

    public void StopTracking() => isTracking = false;

    public void StartShooting()
    {
        if (isShooting)
            return;
        isShooting = true;
        InvokeRepeating("Shoot", cooldown, cooldown);
    }

    public void StopShooting()
    {
        isShooting = false;
        CancelInvoke();
    }

    private void UpdateTracking()
    {
        transform.rotation = Quaternion.identity;

        if (!isTracking || target == null)
            return;

        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    private void Shoot()
    {
        Vector3 pos = new Vector3(transform.position.x, aimHeight, transform.position.z);
        MonsterProjectile proj = Instantiate(projectilePrefab, pos, transform.rotation);
        proj.Init(gameObject, projectileSpeed, projectileLifetime);
    }
}
