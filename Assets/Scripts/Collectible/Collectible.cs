using UnityEngine;

public enum CollectibleType { Coin }

public abstract class Collectible : MonoBehaviour
{
    public CollectibleType Type => GetCollectibleType();

    protected abstract CollectibleType GetCollectibleType();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        gameObject.SetActive(false);
        GameplayEvents.ItemCollected?.Invoke(Type);
    }
}
