using UnityEngine;

public class Breakable : MonoBehaviour, IDamageable
{
    [SerializeField]
    private SequenceManager sequenceManager;

    [SerializeField]
    private Collectible keyPrefab;

    [SerializeField]
    private int id = 0;

    private void Awake()
    {
        if (sequenceManager == null)
            throw new System.NullReferenceException("Breakable has no SequenceManager");

        if (keyPrefab == null)
            throw new System.NullReferenceException("Breakable has no KeyPrefab");
    }

    public void GetDamaged(int amount, GameObject source)
    {
        if (!source.CompareTag("Player"))
            return;

        Break();
    }

    private void Break()
    {
        if (HasKey())
            SpawnKey();

        Destroy(gameObject);
    }

    private bool HasKey()
    {
        return sequenceManager.BreakableBoxKeyId == id;
    }

    private void SpawnKey()
    {
        Instantiate(keyPrefab, transform.position, Quaternion.identity);
    }
}
