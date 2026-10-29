using UnityEngine;

public enum LeverID
{
    Level2_1,
    Level2_2,
    Level2_3,
    Level2_4,
    Level3,
}

public class Lever : MonoBehaviour, IDamageable
{
    [SerializeField]
    private LeverID id;

    public void GetDamaged(int amount, GameObject source)
    {
        if (!source.CompareTag("Player"))
            return;

        GameplayEvents.LeverTriggered?.Invoke(id);
        Debug.Log($"Lever {id} triggered");
    }
}
