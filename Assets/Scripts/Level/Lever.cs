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

    private BridgeId code;

    public void Awake()
    {
        code = BridgeCodes.TryGetValue(id, out BridgeId c) ? c : BridgeId.None;
    }

    public void GetDamaged(int amount, GameObject source)
    {
        if (!source.CompareTag("Player"))
            return;

        if (code == BridgeId.None)
            return;

        GameplayEvents.LeverTriggered?.Invoke(code);
    }
}
