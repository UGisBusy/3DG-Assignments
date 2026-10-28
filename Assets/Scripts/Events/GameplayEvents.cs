using System;

public static class GameplayEvents
{
    public static Action<CollectibleType> ItemCollected;
    public static Action<LeverID> LeverTriggered;
    public static Action MonsterDied;

    public static Action AwakeMonsterLevel3;
    public static Action AwakeMonsterLevel5;

    public static Action<GateID> CloseGate;
}
