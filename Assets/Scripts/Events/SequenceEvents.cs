using System;

public static class SequenceEvents
{
    // State Transition
    public static Action ProceedLevel;
    public static Action Win;
    public static Action Timeout;

    // Generate new keybox
    public static Action<int> ResetKeyBoxId;

    // complete levels (finish task)
    public static Action CompleteLevel1;
    public static Action CompleteLevel2;
    public static Action CompleteLevel3;
    public static Action CompleteLevel4;
}
