using System.Collections.Generic;
using UnityEngine;

public enum LeverId
{
    Lv4_1,
    Lv4_2,
    Lv4_3,
    Lv4_4,
    Lv5_1,
    Lv5_2,
    Lv5_3,
    Lv5_4,
}

public class Lever : MonoBehaviour, IDamageable
{
    readonly Dictionary<LeverId, BridgeId> BridgeCodes = new()
    {
        { LeverId.Lv4_1, BridgeId.Lv4_1 | BridgeId.Lv4_4 },
        { LeverId.Lv4_2, BridgeId.Lv4_2 | BridgeId.Lv4_4 },
        { LeverId.Lv4_3, BridgeId.Lv4_1 | BridgeId.Lv4_2 | BridgeId.Lv4_4 },
        { LeverId.Lv4_4, BridgeId.Lv4_1 | BridgeId.Lv4_3 | BridgeId.Lv4_4 },
        { LeverId.Lv5_1, BridgeId.Lv5_1 },
        { LeverId.Lv5_2, BridgeId.Lv5_2 },
        { LeverId.Lv5_3, BridgeId.Lv5_3 },
        { LeverId.Lv5_4, BridgeId.Lv5_4 },
    };

    [SerializeField]
    private LeverId id;

    // parts of the lever model (filled in by 3DG > Build Lever Prefab)
    [Header("Look")]
    [Tooltip("Pivot at the bottom of the handle; rotated to tilt the handle.")]
    [SerializeField]
    private Transform handlePivot;

    [SerializeField]
    private Renderer knob;

    [SerializeField]
    private Material offMaterial;

    [SerializeField]
    private Material onMaterial;

    [Tooltip("How far the handle leans, in degrees.")]
    [SerializeField]
    private float tiltAngle = 30f;

    private BridgeId code;
    private bool isOn; // starts off

    public void Awake()
    {
        code = BridgeCodes.TryGetValue(id, out BridgeId c) ? c : BridgeId.None;

        // levers are puzzle switches in levels 4 and 5
        MinimapIcon.Attach(gameObject, MinimapIconShape.Diamond, new Color(1f, 0.55f, 0.1f), size: 2f, sortingOrder: 2);

        ApplyLook();
    }

    public void GetDamaged(int amount, GameObject source)
    {
        if (!source.CompareTag("Player"))
            return;

        GameplayEvents.TargetHit?.Invoke();

        if (code == BridgeId.None)
            return;

        // each hit flips the lever, matching its platforms toggling
        isOn = !isOn;
        ApplyLook();

        GameplayEvents.LeverTriggered?.Invoke(code);
    }

    // off: handle leans left, red knob. on: handle leans right, green knob.
    private void ApplyLook()
    {
        if (handlePivot != null)
            handlePivot.localRotation = Quaternion.Euler(0f, 0f, isOn ? -tiltAngle : tiltAngle);

        Material material = isOn ? onMaterial : offMaterial;
        if (knob != null && material != null)
            knob.sharedMaterial = material;
    }
}
