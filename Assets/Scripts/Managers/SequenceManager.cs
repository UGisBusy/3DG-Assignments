using UnityEngine;

public class SequenceManager : MonoBehaviour
{
    [SerializeField]
    private int breakableBoxKeyId = 0;

    // Debug
    [SerializeField]
    private int pickupCount = 0;

    public int BreakableBoxKeyId => breakableBoxKeyId;
    public int PickupCount => pickupCount;

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

        // TODO: move to start game
        GameplayEvents.ItemCollected += _ => pickupCount++;
    }

    public void Start()
    {
        SetStates();
        SetLinks();
    }

    private void SetStates() { }

    private void SetLinks() { }
}
