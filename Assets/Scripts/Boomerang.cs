using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Boomerang : MonoBehaviour
{
    PlayerControl player;
    Target target;
    bool isReturning;
    float flySpeed = 15f;

    public void Init(PlayerControl player, Target target)
    {
        this.player = player;
        this.target = target;
        isReturning = false;
    }

    public void Launch()
    {
        transform.position = player.transform.position;
        gameObject.SetActive(true);

        target.BeTargeted();
        StartCoroutine(FlyTo(target.transform));
    }

    private void Awake()
    {
        gameObject.SetActive(false);
        GameplayEvents.DespawnBoomerang += OnDespawnBoomerang;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        GameplayEvents.DespawnBoomerang -= OnDespawnBoomerang;
    }

    private void OnDespawnBoomerang()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isReturning && other.gameObject == target.gameObject)
        {
            isReturning = true;
            target.BeAttacked();

            StopAllCoroutines();
            StartCoroutine(FlyTo(player.transform));
        }
        else if (isReturning && other.gameObject == player.gameObject)
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    private IEnumerator FlyTo(Transform targetTransform)
    {
        while (true)
        {
            Vector3 direction = targetTransform.position - transform.position;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);

            transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, flySpeed * Time.deltaTime);
            yield return null;
        }
    }
}