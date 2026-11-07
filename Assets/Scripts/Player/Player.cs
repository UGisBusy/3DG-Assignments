using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField]
    private OrbitCamera orbitCamera;

    [Tooltip("Seconds the player ignores further hits after being hurt.")]
    [SerializeField]
    private float invincibleDuration = 1f;

    private float invincibleUntil;

    public OrbitCamera OrbitCamera => orbitCamera;

    public bool IsInvincible => Time.time < invincibleUntil;

    private void Awake()
    {
        if (orbitCamera == null)
            throw new System.NullReferenceException("player has no orbit camera");

        // arrow on top of every other icon, pointing where the player faces
        MinimapIcon.Attach(gameObject, MinimapIconShape.Arrow, new Color(0.2f, 0.6f, 1f), size: 3f, sortingOrder: 10);

        // no control on the intro, win and lose screens
        // (restarting reloads the scene, which brings control back)
        UIEvents.ShowIntro += DisableControl;
        UIEvents.HideIntro += EnableControl;
        UIEvents.ShowWin += DisableControl;
        UIEvents.ShowLoss += DisableControl;
    }

    private void OnDestroy()
    {
        UIEvents.ShowIntro -= DisableControl;
        UIEvents.HideIntro -= EnableControl;
        UIEvents.ShowWin -= DisableControl;
        UIEvents.ShowLoss -= DisableControl;
    }

    private void DisableControl() => SetControl(false);

    private void EnableControl() => SetControl(true);

    // turns the player's input-driven components on/off
    // (mouse look reads raw mouse movement, so pausing time alone wouldn't stop it)
    private void SetControl(bool canControl)
    {
        orbitCamera.enabled = canControl;
        if (TryGetComponent(out PlayerMovement movement)) movement.enabled = canControl;
        if (TryGetComponent(out PlayerAttack attack)) attack.enabled = canControl;
    }

    public void GetDamaged(int amount, GameObject source)
    {
        if (IsInvincible) return;

        invincibleUntil = Time.time + invincibleDuration;
        GameplayEvents.PlayerHurt?.Invoke();
    }
}
