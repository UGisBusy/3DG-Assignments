using UnityEngine;

public class Breakable : MonoBehaviour, IDamageable
{
    [SerializeField]
    private Collectible keyPrefab;

    [SerializeField]
    private int id = 0;

    [SerializeField]
    private bool hasKey = false;

    private void Awake()
    {
        if (keyPrefab == null)
            throw new System.NullReferenceException("Breakable has no KeyPrefab");

        SequenceEvents.ResetKeyBoxId += OnResetKeyBoxId;
    }

    private void OnDestroy()
    {
        SequenceEvents.ResetKeyBoxId -= OnResetKeyBoxId;
    }

    public void GetDamaged(int amount, GameObject source)
    {
        if (!source.CompareTag("Player"))
            return;

        Break();
    }

    private void Break()
    {
        if (hasKey)
            DropKey();

        Destroy(gameObject);
    }

    private void DropKey()
    {
        Instantiate(keyPrefab, transform.position, Quaternion.identity);
    }

    private void OnResetKeyBoxId(int keyId)
    {
        hasKey = (id == keyId);
    }
}
