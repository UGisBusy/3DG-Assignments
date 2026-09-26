using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayManager : MonoBehaviour
{
    const int MAX_TOTAL_COUNT = 500;
    const int MIN_TOTAL_COUNT = 200;
    const int MIN_TARGET_COUNT = 100;
    const int MIN_OBSTACLE_COUNT = 100;

    StateMachine stateMachine;
    IState restState;
    IState runState;
    IState exitState;

    SpawnManager spawnManager;

    int targetCount => spawnManager.TargetCount;
    int obstacleCount => spawnManager.ObstacleCount;

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
        int totalCountDesired = (int)Random.Range(MIN_TOTAL_COUNT, MAX_TOTAL_COUNT);
        int targetCountDesired = (int)Random.Range(MIN_TARGET_COUNT, totalCountDesired - MIN_OBSTACLE_COUNT);
        int obstacleCountDesired = totalCountDesired - targetCountDesired;

        int targetCount = 0, obstacleCount = 0;

        while (targetCount < MIN_TARGET_COUNT)
            spawnManager.SpawnTargets(targetCountDesired, out targetCount);

        while (obstacleCount < MIN_OBSTACLE_COUNT)
            spawnManager.SpawnObstacles(obstacleCountDesired, out obstacleCount);
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
