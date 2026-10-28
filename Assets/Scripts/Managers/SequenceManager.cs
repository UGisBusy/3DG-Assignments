using UnityEngine;

public class SequenceManager : MonoBehaviour
{
    [SerializeField]
    private int breakableBoxKeyId = 0;

    public int BreakableBoxKeyId => breakableBoxKeyId;

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

        stateMachine = new StateMachine();
    }

    public void Start()
    {
        SetStates();
        SetLinks();
    }

    private void SetStates() { }

    private void SetLinks() { }
}
