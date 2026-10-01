using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text targetCountText;
    [SerializeField] private TMP_Text obstacleCountText;
    [SerializeField] private Image boomerangImage;

    Color boomerangReadyColor = Color.white;
    Color boomerangUnavailableColor = new Color(0.3f, 0.3f, 0.3f, 0.6f);

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

    public void UpdateBoomerang(bool hasBoomerang)
    {
        boomerangImage.color = hasBoomerang ? boomerangReadyColor : boomerangUnavailableColor;
    }
}
