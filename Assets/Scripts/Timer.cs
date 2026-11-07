using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Countdown timer that also acts as the player's health:
// getting hurt subtracts a penalty, and reaching zero loses the game.
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

    [Header("Health Bar")]
    [Tooltip("Fill image inside the bar background. Its width shrinks as time runs out.")]
    [SerializeField]
    private RectTransform barFill;

    [Tooltip("Bar color by remaining time: left = empty, right = full.")]
    [SerializeField]
    private Gradient barColor = DefaultBarColor();

    [Tooltip("What shakes on hit. Defaults to the bar background (Bar Fill's parent).")]
    [SerializeField]
    private RectTransform shakeTarget;

    [SerializeField]
    private float shakeDuration = 0.3f;

    [Tooltip("Max offset in UI pixels.")]
    [SerializeField]
    private float shakeStrength = 8f;

    private Image barImage;
    private Vector2 shakeRestPosition;
    private Coroutine shakeRoutine;

    private bool isRunning;

    private void Awake()
    {
        if (uiText == null)
            throw new System.NullReferenceException("Timer has no uiText");

        if (barFill != null)
        {
            // stretch the fill from the left edge; UpdateBar only moves its right edge
            barFill.anchorMin = Vector2.zero;
            barFill.offsetMin = Vector2.zero;
            barFill.offsetMax = Vector2.zero;
            barImage = barFill.GetComponent<Image>();

            if (shakeTarget == null)
                shakeTarget = barFill.parent as RectTransform;
        }

        if (shakeTarget != null)
            shakeRestPosition = shakeTarget.anchoredPosition;

        isRunning = false;
        GameplayEvents.StartTimer += StartTimer;
        GameplayEvents.PlayerHurt += GetPenalty;

        SetBar(1f); // full before the game starts
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
        SetBar(timeLeft / InitialstartTime);
    }

    private void StartTimer()
    {
        timeLeft = InitialstartTime;
        isRunning = true;
    }

    private void GetPenalty()
    {
        timeLeft -= penalty;
        Shake();
    }

    private void Shake()
    {
        if (shakeTarget == null) return;

        // restart if already shaking so back-to-back hits don't drift the bar
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(ShakeRoutine());
    }

    private System.Collections.IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            // shake fades out over the duration
            float strength = shakeStrength * (1f - elapsed / shakeDuration);
            shakeTarget.anchoredPosition = shakeRestPosition + Random.insideUnitCircle * strength;

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeTarget.anchoredPosition = shakeRestPosition;
        shakeRoutine = null;
    }

    private string formatText()
    {
        int totalSeconds = Mathf.Max(0, Mathf.CeilToInt(timeLeft));
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return $"{minutes:00}:{seconds:00}";
    }

    // ratio: 1 = full bar, 0 = empty bar
    private void SetBar(float ratio)
    {
        if (barFill == null) return;

        ratio = Mathf.Clamp01(ratio);
        barFill.anchorMax = new Vector2(ratio, 1f);

        if (barImage != null)
            barImage.color = barColor.Evaluate(ratio);
    }

    // red when low, yellow at half, green when full
    private static Gradient DefaultBarColor()
    {
        var gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(0.85f, 0.15f, 0.15f), 0f),
                new GradientColorKey(new Color(0.95f, 0.80f, 0.20f), 0.5f),
                new GradientColorKey(new Color(0.25f, 0.80f, 0.30f), 1f),
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f),
            });
        return gradient;
    }
}
