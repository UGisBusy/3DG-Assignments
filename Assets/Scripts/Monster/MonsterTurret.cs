using UnityEngine;

public class MonsterTurret : MonoBehaviour
{
    [SerializeField]
    private bool isTracking = false;

    Transform target;

    private void Update()
    {
        UpdateTracking();
    }

    public void StartTracking(Transform target)
    {
        this.target = target;
        isTracking = true;
    }

    public void DisableTracking()
    {
        target = null;
        isTracking = false;
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
}
