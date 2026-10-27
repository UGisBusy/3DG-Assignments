using UnityEngine;

public class SequenceManager : MonoBehaviour
{
    static SequenceManager instance;

    StateMachine stateMachine;

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
    }

    private void Init()
    {
        stateMachine = new StateMachine();

        SetStates();
        SetLinks();
    }

    private void SetStates() { }

    private void SetLinks() { }
}
