using System;

public static class GameplayEvents
{
    public static Action EnterRestState;
    public static Action EnterRunState;
    public static Action<Target> PlayerAttack;
    public static Action DespawnBoomerang;
    public static Action TargetScores;
}
