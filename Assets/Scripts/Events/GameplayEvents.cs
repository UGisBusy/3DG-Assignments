using System;

public static class GameplayEvents
{
    public static Action<CollectibleType> ItemCollected;
    public static Action<BridgeId> LeverTriggered;
    // player's attack landed on something (monster, box, lever); used for the hit sound
    public static Action TargetHit;
    public static Action MonsterDied;
    public static Action PlayerHurt;


    public static Action AwakeMonsterLevel3;
    public static Action AwakeMonsterLevel5;

    public static Action<GateId> CloseGate;

    public static Action StartTimer;
}
