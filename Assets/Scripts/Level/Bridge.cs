using UnityEngine;

public enum BridgeId
{
    None = 0,
    Lv4_1 = 1 << 0,
    Lv4_2 = 1 << 1,
    Lv4_3 = 1 << 2,
    Lv4_4 = 1 << 3,
    Lv5_1 = 1 << 4,
    Lv5_2 = 1 << 5,
    Lv5_3 = 1 << 6,
    Lv5_4 = 1 << 7,
}

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]
public class Bridge : MonoBehaviour
{
    [SerializeField]
    private BridgeId id;

    [SerializeField]
    private bool isOnDefault = false;

    private bool isOn = false;

    MeshRenderer meshRenderer;
    Collider bridgeCollider;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        bridgeCollider = GetComponent<Collider>();

        isOn = isOnDefault;
        SetVisible(isOn);

        GameplayEvents.LeverTriggered += toggle;
    }

    private void OnDestroy()
    {
        GameplayEvents.LeverTriggered -= toggle;
    }

    private void toggle(BridgeId code)
    {
        if ((code & id) == 0)
            return;

        isOn = !isOn;
        SetVisible(isOn);
    }

    private void SetVisible(bool visible)
    {
        meshRenderer.enabled = visible;
        bridgeCollider.enabled = visible;
    }
}
