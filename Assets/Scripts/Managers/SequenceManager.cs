using UnityEngine;
using UnityEngine.SceneManagement;

public class SequenceManager : MonoBehaviour
{
    const string GameplaySceneName = "Gameplay";

    static SequenceManager instance;

    StateMachine stateMachine;
    IState launchState;
    IState gameplayState;
    IState exitState;

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
    }

    public void Start()
    {
        Init();
        stateMachine.EnterState(launchState);
    }

    private void Init()
    {
        stateMachine = new StateMachine();

        SetStates();
        SetLinks();
    }

    private void SetStates()
    {
        launchState = new State(enter: EnterLaunch);
        gameplayState = new State(enter: EnterGameplay);
        exitState = new State(enter: ExitApp);
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

    private void EnterLaunch()
    {
        SequenceEvents.StartGameplay.Invoke();
    }

    private void EnterGameplay()
    {
        SceneManager.sceneLoaded += OnGameplaySceneLoaded;
        SceneManager.LoadScene(GameplaySceneName, LoadSceneMode.Single);
    }

    private void OnGameplaySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnGameplaySceneLoaded;

        GameplayManager gameplayManager = FindFirstObjectByType<GameplayManager>();
        gameplayManager.Init();
        gameplayManager.Run();
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
