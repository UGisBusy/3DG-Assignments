using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private HudUI hudUI;
    [SerializeField] private MinimapUI minimapUI;

    GameplayManager gameplayManager;

    public void Init(GameplayManager gameplayManager)
    {
        this.gameplayManager = gameplayManager;
    }

    private void Awake()
    {
        hudUI = GetComponent<HudUI>();
        minimapUI = GetComponent<MinimapUI>();
    }

    private void Update()
    {
        hudUI.UpdateScore(gameplayManager.Score);
        hudUI.UpdateTargetCount(gameplayManager.TargetCount, gameplayManager.TotalTargetCount);
        hudUI.UpdateObstacleCount(gameplayManager.TotalObstacleCount);
        hudUI.UpdateBoomerang(gameplayManager.PlayerHasBoomerang);
        hudUI.UpdateTimer(gameplayManager.ElapsedTime);
        minimapUI.UpdatePlayer(gameplayManager.PlayerPosition, gameplayManager.PlayerYaw);
    }
}
