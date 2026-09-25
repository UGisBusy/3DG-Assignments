using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayManager : MonoBehaviour
{
    StateMachine stateMachine;
    IState restState;
    IState runState;
    IState exitState;

    InputAction proceedStateAction;

    public void Init()
    {
        stateMachine = new StateMachine();

        SetStates();
        SetLinks();

        // TODO: testing perpose
        proceedStateAction = new InputAction(binding: "<Keyboard>/space");
        proceedStateAction.performed += OnProceedStatePerformed;
        proceedStateAction.Enable();
    }

    public void Run()
    {
        stateMachine.EnterState(restState);
    }

    private void SetStates()
    {
        restState = new State();
        runState = new State();
        exitState = new State(enter: ExitGameplay);
    }

    private void SetLinks()
    {
        EventWrapper ProceedStateWrapper = new EventWrapper
        {

            Subscribe = handler => GameplayEvents.ProceedState += handler,
            Unsubscribe = handler => GameplayEvents.ProceedState -= handler
        };

        restState.AddLink(new Link(runState, ProceedStateWrapper));
        runState.AddLink(new Link(exitState, ProceedStateWrapper));
    }

    private void ExitGameplay()
    {
        SequenceEvents.ExitGameplay?.Invoke();
    }

    private void OnDestroy()
    {
        // TODO: testing perpose
        proceedStateAction.performed -= OnProceedStatePerformed;
        proceedStateAction.Disable();
    }

    private void OnProceedStatePerformed(InputAction.CallbackContext context)
    {
        GameplayEvents.ProceedState?.Invoke();
    }
}
