using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class SequenceManager : MonoBehaviour
{
    const int BOX_COUNT = 12;
    const string GAME_SCENE = "Game";

    [Header("Debug")]
    [SerializeField]
    string currentStateName = "None";

    [SerializeField]
    private int pickupCount = 0;

    public int PickupCount => pickupCount;
    static SequenceManager instance;

    State StartState;
    State Level1State;
    State Level2State;
    State Level3State;
    State Level4State;
    State Level5State;
    State WinState;
    State LossState;
    StateMachine stateMachine;

    InputAction ExitAction;

    private void Awake()
    {
        // prevent having two SequenceManager instance
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        stateMachine = new StateMachine((newState) => currentStateName = newState.Name);

        ExitAction = new InputAction(binding: "<Keyboard>/escape");
        ExitAction.performed += _ => ExitApp();
        ExitAction.Enable();

        SceneManager.sceneLoaded += OnGameSceneLoaded;

        GameplayEvents.ItemCollected += _ => pickupCount++;
    }

    private void Start()
    {
        if (instance != this)
            return;

        SetStates();
        SetLinks();

        stateMachine.EnterState(StartState);
    }

    private void SetStates()
    {
        StartState = new State("Start");
        Level1State = new State("Level1", enter: EnterLevel1);
        Level2State = new State("Level2");
        Level3State = new State("Level3");
        Level4State = new State("Level4");
        Level5State = new State("Level5");
        WinState = new State("Win");
        LossState = new State("Loss");
    }

    private void SetLinks()
    {
        // play, replay
        StartState.AddLink(
            new InputLink(Level1State, new InputAction(binding: "<Keyboard>/space"))
        );
        WinState.AddLink(new InputLink(Level1State, new InputAction(binding: "<Keyboard>/r")));
        LossState.AddLink(new InputLink(Level1State, new InputAction(binding: "<Keyboard>/r")));

        // proceed
        EventWrapper proceedEW = new EventWrapper
        {
            Subscribe = h => SequenceEvents.ProceedLevel += h,
            Unsubscribe = h => SequenceEvents.ProceedLevel -= h,
        };
        Level1State.AddLink(new Link(Level2State, proceedEW));
        Level2State.AddLink(new Link(Level3State, proceedEW));
        Level3State.AddLink(new Link(Level4State, proceedEW));
        Level4State.AddLink(new Link(Level5State, proceedEW));

        // win
        EventWrapper winEW = new EventWrapper
        {
            Subscribe = h => SequenceEvents.Win += h,
            Unsubscribe = h => SequenceEvents.Win -= h,
        };
        Level5State.AddLink(new Link(WinState, winEW));

        // loss
        EventWrapper timeoutEW = new EventWrapper
        {
            Subscribe = h => SequenceEvents.Timeout += h,
            Unsubscribe = h => SequenceEvents.Timeout -= h,
        };
        Level1State.AddLink(new Link(LossState, timeoutEW));
        Level2State.AddLink(new Link(LossState, timeoutEW));
        Level3State.AddLink(new Link(LossState, timeoutEW));
        Level4State.AddLink(new Link(LossState, timeoutEW));
        Level5State.AddLink(new Link(LossState, timeoutEW));
    }

    private void EnterLevel1()
    {
        SceneManager.LoadScene(GAME_SCENE);
    }

    private void OnGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // reset pickup count
        pickupCount = 0;

        // reset key box id
        SequenceEvents.ResetKeyBoxId?.Invoke(Random.Range(0, BOX_COUNT));
    }

    private void ExitApp()
    {
        Debug.Log("Exit App");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
