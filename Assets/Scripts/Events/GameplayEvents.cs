using System;

public static class GameplayEvents
{
    public static Action<CollectibleType> ItemCollected;
    public static Action MonsterDied;
}
