using UnityEngine;

public enum CollectibleType
{
    Collectible,
}

public abstract class AbstractCollectible : MonoBehaviour
{
    public CollectibleType Type => GetCollectibleType();

    protected abstract CollectibleType GetCollectibleType();

    // dropped puzzle items (key, crystals) show on the minimap until picked up
    protected virtual void Awake()
    {
        MinimapIcon.Attach(gameObject, MinimapIconShape.Circle, new Color(0.3f, 1f, 0.5f), size: 1.5f, sortingOrder: 3);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        gameObject.SetActive(false);
        GameplayEvents.ItemCollected?.Invoke(Type);
    }
}
