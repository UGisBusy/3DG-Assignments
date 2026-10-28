using System;

public static class SequenceEvents
{
    // State Transition
    public static Action ProceedLevel;
    public static Action Win;
    public static Action Timeout;

    // Generate new keybox
    public static Action<int> ResetKeyBoxId;
}
