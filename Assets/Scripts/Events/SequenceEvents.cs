using System;

public static class SequenceEvents
{
    // State Transition
    public static Action ProceedLevel;
    public static Action Win;
    public static Action Timeout;

    public static Action<int> ResetKeyBoxId;

    // complete levels (finish task)
    public static Action CompleteLevel1;
    public static Action CompleteLevel2;
    public static Action CompleteLevel3;
    public static Action CompleteLevel4;

    // fired once at the moment each of the 5 puzzles is actually solved (used for scoring)
    public static Action PuzzleSolved;
}
