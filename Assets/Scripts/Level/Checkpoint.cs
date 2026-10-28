using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    bool isTrigger = false;

    private void OnTriggerEnter(Collider collider)
    {
        if (isTrigger || !collider.CompareTag("Player"))
            return;

        SequenceEvents.ProceedLevel?.Invoke();
        Destroy(gameObject);
        isTrigger = true;
    }
}
