using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum GateID
{
    Lv1,
    Lv2,
    Lv3,
    Lv4,
}

public class Gate : MonoBehaviour
{
    [SerializeField]
    private GateID id;

    EventWrapper openEW;
    EventWrapper<GateID> closeEW;
    GateBlock block;

    private void Awake()
    {
        block = GetComponentInChildren<GateBlock>();
        if (block == null)
            throw new System.NullReferenceException("Gate has no GateBlock");

        InitEventWrappers();

        if (openEW != null)
            openEW.Subscribe(block.Open);
        if (closeEW != null)
            closeEW.Subscribe(Close);
    }

    private void OnDestroy()
    {
        if (openEW != null)
            openEW.Unsubscribe(block.Open);
        if (closeEW != null)
            closeEW.Unsubscribe(Close);
    }

    private void Close(GateID id)
    {
        if (this.id == id)
            block.Close();
    }

    private void InitEventWrappers()
    {
        closeEW = new EventWrapper<GateID>
        {
            Subscribe = h => GameplayEvents.CloseGate += h,
            Unsubscribe = h => GameplayEvents.CloseGate -= h,
        };

        switch (id)
        {
            case GateID.Lv1:
                openEW = new EventWrapper
                {
                    Subscribe = h => SequenceEvents.CompleteLevel1 += h,
                    Unsubscribe = h => SequenceEvents.CompleteLevel1 -= h,
                };
                break;
            case GateID.Lv2:
                openEW = new EventWrapper
                {
                    Subscribe = h => SequenceEvents.CompleteLevel2 += h,
                    Unsubscribe = h => SequenceEvents.CompleteLevel2 -= h,
                };
                break;
            case GateID.Lv3:
                openEW = new EventWrapper
                {
                    Subscribe = h => SequenceEvents.CompleteLevel3 += h,
                    Unsubscribe = h => SequenceEvents.CompleteLevel3 -= h,
                };
                break;
            case GateID.Lv4:
                openEW = new EventWrapper
                {
                    Subscribe = h => SequenceEvents.CompleteLevel4 += h,
                    Unsubscribe = h => SequenceEvents.CompleteLevel4 -= h,
                };
                break;
        }
    }
}
