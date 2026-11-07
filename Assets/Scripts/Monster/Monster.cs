using UnityEngine;

public enum MonsterType
{
    Lv3,
    Lv5,
}

public class Monster : MonoBehaviour, IDamageable
{
    [SerializeField]
    private Collectible crystalPrefab;

    [SerializeField]
    private MonsterType type = MonsterType.Lv3;

    [SerializeField]
    private int hp = 3;

    MonsterTurret turret;
    MonsterHealthBar healthBar;
    int maxHp;

    private void Awake()
    {
        maxHp = hp;

        // use the bar on the prefab if there is one (to tweak its settings), otherwise add a default one
        healthBar = GetComponent<MonsterHealthBar>();
        if (healthBar == null)
            healthBar = gameObject.AddComponent<MonsterHealthBar>();

        turret = GetComponentInChildren<MonsterTurret>();
        if (turret == null)
            throw new System.NullReferenceException("Monster has no Turret");

        if (crystalPrefab == null)
            throw new System.NullReferenceException("Monster has no CrystalPrefab");

        if (type == MonsterType.Lv3)
            GameplayEvents.AwakeMonsterLevel3 += WakeUp;
        if (type == MonsterType.Lv5)
            GameplayEvents.AwakeMonsterLevel5 += WakeUp;
    }

    private void OnDestroy()
    {
        if (type == MonsterType.Lv3)
            GameplayEvents.AwakeMonsterLevel3 -= WakeUp;
        if (type == MonsterType.Lv5)
            GameplayEvents.AwakeMonsterLevel5 -= WakeUp;
    }

    private void WakeUp()
    {
        turret.StartTracking();
        Invoke("StartShooting", Random.Range(0, 2));
    }

    private void StartShooting() => turret.StartShooting();

    public void GetDamaged(int amount, GameObject source)
    {
        if (hp <= 0) return; // already dying this frame, don't count the kill twice

        hp -= amount;
        healthBar.SetRatio((float)hp / maxHp);

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
