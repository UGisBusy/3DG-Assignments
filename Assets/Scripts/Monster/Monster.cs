using UnityEngine;

public class Monster : MonoBehaviour, IDamageable
{
    [SerializeField]
    private Collectible crystalPrefab;

    [SerializeField]
    private int hp = 3;

    MonsterTurret turret;

    private void Awake()
    {
        turret = GetComponentInChildren<MonsterTurret>();
        if (turret == null)
            throw new System.NullReferenceException("Monster has no Turret");

        if (crystalPrefab == null)
            throw new System.NullReferenceException("Monster has no CrystalPrefab");
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

        // TODO: sound
        GameplayEvents.MonsterDied?.Invoke();
        DropCrystal();
        Destroy(gameObject);
    }

    private void DropCrystal()
    {
        Instantiate(crystalPrefab, transform.position, Quaternion.identity);
    }
}
