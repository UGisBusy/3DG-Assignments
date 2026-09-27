using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class GameplayManager : MonoBehaviour
{
    const int MAX_TOTAL_COUNT = 500;
    const int MIN_TOTAL_COUNT = 200;
    const int MIN_TARGET_COUNT = 100;
    const int MIN_OBSTACLE_COUNT = 100;

    [SerializeField] private PlayerControl player;

    StateMachine stateMachine;
    IState restState;
    IState runState;
    IState exitState;

    SpawnManager spawnManager;
    int targetCount => spawnManager.TargetCount;
    int obstacleCount => spawnManager.ObstacleCount;

    public void Init()
    {
        SetStates();
        SetLinks();

        spawnManager.Init();
    }

    public void Run()
    {
        stateMachine.EnterState(restState);
    }

    private void Awake()
    {
        stateMachine = new StateMachine();
        spawnManager = GetComponent<SpawnManager>();

        if (player == null)
            throw new System.NullReferenceException("player is null");
    }

    private void SetStates()
    {
        restState = new State(enter: EnterRestState);
        runState = new State(enter: EnterRunState, exit: ExitRunstate);
        exitState = new State(enter: ExitGameplay);
    }

    private void SetLinks()
    {
        // TODO
        InputAction proceedAction = new InputAction(binding: "<Keyboard>/l");
        InputAction exitAction = new InputAction(binding: "<Keyboard>/escape");

        restState.AddLink(new InputLink(runState, proceedAction));
        restState.AddLink(new InputLink(exitState, exitAction));

        runState.AddLink(new InputLink(restState, proceedAction));
        runState.AddLink(new InputLink(exitState, exitAction));
    }

    private void ExitGameplay()
    {
        SequenceEvents.ExitGameplay?.Invoke();
    }

    private void EnterRunState()
    {
        SpawnAll();
        player.EnableRayPick();
    }

    private void ExitRunstate()
    {
        player.DisableRayPick();
    }

    private void EnterRestState()
    {
        spawnManager.DespawnAll();
    }

    private void SpawnAll()
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
}
