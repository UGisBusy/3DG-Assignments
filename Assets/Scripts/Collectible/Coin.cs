using UnityEngine;

public class Coin : Collectible
{
    [SerializeField] private Transform model;

    float spinSpeed = 180f;
    float yaw;

    protected override CollectibleType GetCollectibleType()
    {
        return CollectibleType.Coin;
    }

    private void Awake()
    {
        if (model == null)
            throw new System.NullReferenceException("coin model is null");
    }

    private void Start()
    {
        yaw = Random.Range(0f, 360f);
    }

    private void Update()
    {
        yaw = (yaw + spinSpeed * Time.deltaTime) % 360f;
        model.rotation = Quaternion.Euler(0f, yaw, 90f);
    }
}
