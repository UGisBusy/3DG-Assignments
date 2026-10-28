using UnityEngine;

public class GateBlock : MonoBehaviour
{
    public void Open()
    {
        gameObject.SetActive(false);
    }

    public void Close()
    {
        gameObject.SetActive(true);
    }
}
