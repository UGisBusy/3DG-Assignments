using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MonsterProjectile : MonoBehaviour
{
    GameObject owner;
    float speed = 4f;
    float lifetime = 5f;

    public void Init(GameObject owner, float speed, float lifetime)
    {
        this.owner = owner;
        this.speed = speed;
        this.lifetime = lifetime;
    }

    private void Awake()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += transform.forward * (speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().GetDamaged(1, owner);
            Destroy(gameObject);
        }

        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
