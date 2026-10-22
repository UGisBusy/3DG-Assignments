using System;
using UnityEngine;

public static class GameplayEvents
{
    public static Action EnterRestState;
    public static Action EnterRunState;
    public static Action<Target> PlayerAttack;
    public static Action DespawnBoomerang;
    public static Action TargetScores;
    public static Action<CollectibleType> ItemCollected;
    public static Action<Vector3> BoomerangHit;
    public static Action InvalidSelection;
}
