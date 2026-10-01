using TMPro;
using UnityEngine;

public class HudUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text targetCountText;
    [SerializeField] private TMP_Text obstacleCountText;

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void UpdateTargetCount(int current, int total)
    {
        targetCountText.text = $"Targets: {current} / {total}";
    }

    public void UpdateObstacleCount(int total)
    {
        obstacleCountText.text = $"Obstacle: {total}";
    }
}
