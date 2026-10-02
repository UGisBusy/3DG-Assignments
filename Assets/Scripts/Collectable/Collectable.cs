using UnityEngine;

public enum CollectableType { Coin }

public abstract class Collectable : MonoBehaviour
{
    public CollectableType Type => GetCollectableType();

    protected abstract CollectableType GetCollectableType();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        gameObject.SetActive(false);
        GameplayEvents.ItemCollected?.Invoke(Type);
    }
}
