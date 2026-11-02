using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{

    [SerializeField]
    private TMP_Text uiText; 

    [SerializeField]
    private float InitialstartTime = 300;

    [SerializeField]
    private float penalty = 5f;

    [SerializeField]
    private float timeLeft;

    private bool isRunning;

    private void Awake()
    {
        if (uiText == null)
            throw new System.NullReferenceException("Timer has no uiText");
        
        isRunning = false;
        GameplayEvents.StartTimer += StartTimer;
        GameplayEvents.PlayerHurt += GetPenalty;
    }

    private void OnDestroy()
    {
        GameplayEvents.StartTimer -= StartTimer;
        GameplayEvents.PlayerHurt -= GetPenalty;
    }

    private void Update()
    {
        if (!isRunning) return;

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0)
        {
            SequenceEvents.Timeout?.Invoke(); 
            isRunning = false;
            timeLeft = 0;
        }

        uiText.text = formatText();
    }

    private void StartTimer()
    {
        timeLeft = InitialstartTime;
        isRunning = true;
    }

    private void GetPenalty()
    {
        timeLeft -= penalty;
    }

    private string formatText()
    {
        int totalSeconds = Mathf.Max(0, Mathf.CeilToInt(timeLeft));
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return $"{minutes:00}:{seconds:00}";
    }
}
