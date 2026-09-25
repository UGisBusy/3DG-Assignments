using UnityEngine;

public class SequenceManager : MonoBehaviour
{
    StateMachine stateMachine;
    IState launchState;
    IState gameplayState;
    IState exitState;

    public void Start()
    {
        Init();
    }

    private void Init()
    {
        stateMachine = new StateMachine();

        InitStates();
        SetLinks();
    }

    private void InitStates()
    {
        launchState = new State();
        gameplayState = new State();
        exitState = new State();
    }

    private void SetLinks()
    {
        EventWrapper StartGameplayWrapper = new EventWrapper
        {
            Subscribe = handler => SequenceEvents.StartGameplay += handler,
            Unsubscribe = handler => SequenceEvents.StartGameplay -= handler
        };

        EventWrapper ExitApplication = new EventWrapper
        {
            Subscribe = handler => SequenceEvents.ExitGameplay += handler,
            Unsubscribe = handler => SequenceEvents.ExitGameplay -= handler
        };

        launchState.AddLink(new Link(gameplayState, StartGameplayWrapper));

        gameplayState.AddLink(new Link(exitState, ExitApplication));
    }
}
