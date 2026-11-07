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
    }

    public void GetDamaged(int amount, GameObject source)
    {
        if (IsInvincible) return;

        invincibleUntil = Time.time + invincibleDuration;
        GameplayEvents.PlayerHurt?.Invoke();
    }
}
