using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayManager : MonoBehaviour
{
    const int MAX_TOTAL_COUNT = 500;
    const int MIN_TOTAL_COUNT = 200;
    const int MIN_TARGET_COUNT = 100;
    const int MIN_OBSTACLE_COUNT = 100;
    const int TARGET_SCORE = 10;

    [SerializeField] private PlayerControl player;
    [SerializeField] private GameObject boomerangPrefab;
    [SerializeField] private RestArea restArea;
    [SerializeField] private UIManager uIManager;

    public int TotalTargetCount { get => spawnManager == null ? 0 : spawnManager.TotalTargetCount; }
    public int TotalObstacleCount { get => spawnManager == null ? 0 : spawnManager.TotalObstacleCount; }
    public int TargetCount { get => spawnManager == null ? 0 : spawnManager.TargetCount; }
    public int Score { get; private set; }
    public bool PlayerHasBoomerang { get => player.HasBoomerang; }
    public float ElapsedTime { get; private set; }

    StateMachine stateMachine;
    IState restState;
    IState runState;
    IState exitState;

    SpawnManager spawnManager;
    bool isTimerRunning;

    public void Init()
    {
        SetStates();
        SetLinks();

        spawnManager.Init();
        uIManager.Init(this);
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

        if (boomerangPrefab == null)
            throw new System.NullReferenceException("boomerangPrefab is null");

        if (restArea == null)
            throw new System.NullReferenceException("rest area is null");

        if (uIManager == null)
            throw new System.NullReferenceException("uIManager is null");

        Cursor.lockState = CursorLockMode.Locked;
        Score = 0;
    }

    private void Update()
    {
        if (isTimerRunning)
            ElapsedTime += Time.deltaTime;
    }

    private void SetStates()
    {
        restState = new State(enter: EnterRestState);
        runState = new State(enter: EnterRunState, exit: ExitRunstate);
        exitState = new State(enter: ExitGameplay);
    }

    private void SetLinks()
    {
        InputAction exitAction = new InputAction(binding: "<Keyboard>/escape");

        EventWrapper enterRunEventWrapper = new EventWrapper
        {
            Subscribe = handler => GameplayEvents.EnterRunState += handler,
            Unsubscribe = handler => GameplayEvents.EnterRunState -= handler
        };

        EventWrapper enterRestEventWrapper = new EventWrapper
        {
            Subscribe = handler => GameplayEvents.EnterRestState += handler,
            Unsubscribe = handler => GameplayEvents.EnterRestState -= handler
        };

        restState.AddLink(new Link(runState, enterRunEventWrapper));
        restState.AddLink(new InputLink(exitState, exitAction));

        runState.AddLink(new Link(restState, enterRestEventWrapper));
        runState.AddLink(new InputLink(exitState, exitAction));
    }

    private void ExitGameplay()
    {
        SequenceEvents.ExitGameplay?.Invoke();
    }

    private void EnterRunState()
    {
        Score = 0;
        SpawnAll();
        ElapsedTime = 0f;
        isTimerRunning = true;
        player.EnableAttack();
        restArea.EnableCheckEnter();
        GameplayEvents.PlayerAttack += OnPlayerAttack;
        GameplayEvents.TargetScores += OnTargetScores;
    }

    private void ExitRunstate()
    {
        isTimerRunning = false;
        player.DisableAttack();
        GameplayEvents.PlayerAttack -= OnPlayerAttack;
        GameplayEvents.TargetScores -= OnTargetScores;
        GameplayEvents.DespawnBoomerang?.Invoke();
    }

    private void EnterRestState()
    {
        spawnManager.DespawnAll();
        restArea.EnableCheckExit();
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

    private void OnPlayerAttack(Target target)
    {
        GameObject obj = Instantiate(boomerangPrefab);
        Boomerang boomerang = obj.GetComponent<Boomerang>();
        boomerang.Init(player, target);
        boomerang.Launch();
    }

    private void OnTargetScores()
    {
        if (stateMachine.CurrentState != runState)
            return;
        Score += TARGET_SCORE;
    }
}
