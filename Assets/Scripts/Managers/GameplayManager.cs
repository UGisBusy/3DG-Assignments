using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayManager : MonoBehaviour
{
    StateMachine stateMachine;
    IState restState;
    IState runState;
    IState exitState;

    SpawnManager spawnManager;

    InputAction proceedStateAction;

    public void Init()
    {
        stateMachine = new StateMachine();

        SetStates();
        SetLinks();

        spawnManager = GetComponent<SpawnManager>();
        spawnManager.Init();

        // TODO: testing perpose
        proceedStateAction = new InputAction(binding: "<Keyboard>/l");
        proceedStateAction.performed += OnProceedStatePerformed;
        proceedStateAction.Enable();
    }

    public void Run()
    {
        stateMachine.EnterState(restState);
    }

    private void OnDestroy()
    {
        // TODO: testing perpose
        proceedStateAction.performed -= OnProceedStatePerformed;
        proceedStateAction.Disable();
    }

    private void SetStates()
    {
        restState = new State(enter: EnterRestState);
        runState = new State(enter: EnterRunState);
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
        runState.AddLink(new Link(restState, ProceedStateWrapper));
    }

    private void ExitGameplay()
    {
        SequenceEvents.ExitGameplay?.Invoke();
    }

    private void EnterRunState()
    {
        // TODO
        int targetAmount = 200;
        spawnManager.SpawnTargets(targetAmount, out targetAmount);

        int obstacleAmount = 200;
        spawnManager.SpawnObstacles(obstacleAmount, out obstacleAmount);
    }

    private void EnterRestState()
    {
        spawnManager.DespawnAll();
    }

    private void OnProceedStatePerformed(InputAction.CallbackContext context)
    {
        GameplayEvents.ProceedState?.Invoke();
    }
}
