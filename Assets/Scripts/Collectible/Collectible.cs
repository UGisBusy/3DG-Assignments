using UnityEngine;

public class Collectible : AbstractCollectible
{
    protected override CollectibleType GetCollectibleType()
    {
        return CollectibleType.Collectible;
    }
}
