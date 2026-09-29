using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private HudUI hudUI;

    GameplayManager gameplayManager;

    public void Init(GameplayManager gameplayManager)
    {
        this.gameplayManager = gameplayManager;
    }

    private void Awake()
    {
        hudUI = GetComponent<HudUI>();
    }

    private void Update()
    {
        hudUI.UpdateScore(gameplayManager.Score);
        hudUI.UpdateTargetCount(gameplayManager.TargetCount, gameplayManager.TotalTargetCount);
        hudUI.UpdateObstacleCount(gameplayManager.TotalObstacleCount);
    }
}
