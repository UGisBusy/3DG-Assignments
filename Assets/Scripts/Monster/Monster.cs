using UnityEngine;

public class Monster : MonoBehaviour, IDamageable
{
    [SerializeField]
    private int hp = 3;

    MonsterTurret turret;

    private void Awake()
    {
        turret = GetComponentInChildren<MonsterTurret>();
        if (turret == null)
            throw new System.NullReferenceException("Monster has no Turret");
    }

    private void Start()
    {
        // TODO: start when player enter room
        turret.StartTracking();
        turret.StartShooting();
    }

    public void GetDamaged(int amount, GameObject source)
    {
        hp -= amount;
        if (hp <= 0)
            Die();
    }

    private void Die()
    {
        hp = 0;

        // TODO: drop heart, sound
        GameplayEvents.MonsterDied?.Invoke();
        Destroy(gameObject);
    }
}
