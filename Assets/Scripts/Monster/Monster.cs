using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;

    MonsterTurret turret;

    private void Awake()
    {
        if (playerTransform == null)
            throw new System.NullReferenceException("Monster has no PlayerTransform");

        turret = GetComponentInChildren<MonsterTurret>();
        if (turret == null)
            throw new System.NullReferenceException("Monster has no Turret");

        // TODO
        turret.StartTracking(playerTransform);
    }
}
