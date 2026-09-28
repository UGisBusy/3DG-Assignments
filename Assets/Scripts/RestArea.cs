
using UnityEngine;

public class RestArea : MonoBehaviour
{
    enum CheckMode
    {
        None,
        Enter,
        Exit
    }

    CheckMode checkMode;

    public void EnableCheckEnter()
    {
        checkMode = CheckMode.Enter;
    }

    public void EnableCheckExit()
    {
        checkMode = CheckMode.Exit;
    }

    private void Awake()
    {
        checkMode = CheckMode.None;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (checkMode != CheckMode.Enter || !other.gameObject.CompareTag("Player"))
            return;

        GameplayEvents.EnterRestState?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (checkMode != CheckMode.Exit || !other.gameObject.CompareTag("Player"))
            return;

        GameplayEvents.EnterRunState?.Invoke();
    }
}
