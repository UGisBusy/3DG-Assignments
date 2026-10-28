using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField]
    private OrbitCamera orbitCamera;

    public OrbitCamera OrbitCamera => orbitCamera;

    private void Awake()
    {
        if (orbitCamera == null)
            throw new System.NullReferenceException("player has no orbit camera");
    }

    public void GetDamaged(int amount, GameObject source)
    {
        // TODO
    }
}
