using UnityEngine;

public class Spin : MonoBehaviour
{

    float spinSpeed = 900f;
    float yaw;
    private void Update()
    {
        yaw = (yaw + spinSpeed * Time.deltaTime) % 360f;
        transform.rotation = Quaternion.Euler(90f, yaw, 0f);
    }
}
