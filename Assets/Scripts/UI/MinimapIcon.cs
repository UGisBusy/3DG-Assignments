using UnityEngine;

// keeps a minimap icon lying flat above its parent, whatever way the parent tumbles
public class MinimapIcon : MonoBehaviour
{
    [SerializeField] private float height = 2f;
    [SerializeField] private bool followYaw = false;

    private void LateUpdate()
    {
        Transform parent = transform.parent;
        float yaw = followYaw ? parent.eulerAngles.y : 0f;

        transform.SetPositionAndRotation(parent.position + Vector3.up * height, Quaternion.Euler(90f, yaw, 0f));
    }
}
