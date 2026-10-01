using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    public bool IsBeingAttack { get; private set; }
    Rigidbody rb;

    public void BeTargeted()
    {
        IsBeingAttack = true;
    }

    public void BeAttacked(Vector3 force)
    {
        rb.isKinematic = false;
        rb.AddForce(force, ForceMode.Impulse);
        StartCoroutine(StartDesappearing());
    }

    private void Awake()
    {
        IsBeingAttack = false;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private IEnumerator StartDesappearing()
    {
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
        GameplayEvents.TargetScores?.Invoke();
    }
}
