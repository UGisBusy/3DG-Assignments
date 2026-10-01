using UnityEngine;
using UnityEngine.UI;

public class MinimapUI : MonoBehaviour
{
    [SerializeField] private RawImage mapImage;
    [SerializeField] private LayerMask cullingMask = ~0;
    Vector2 areaCenter = Vector2.zero;
    float areaSize = 200;
    float cameraHeight = 100f;
    int textureResolution = 512;

    Camera mapCamera;
    RenderTexture renderTexture;

    private void Awake()
    {
        if (mapImage == null)
            throw new System.NullReferenceException("minimap image is null");

        renderTexture = new RenderTexture(textureResolution, textureResolution, 16);
        mapImage.texture = renderTexture;

        GameObject cameraObj = new GameObject("MinimapCamera");
        cameraObj.transform.SetPositionAndRotation(
            new Vector3(areaCenter.x, cameraHeight, areaCenter.y),
            Quaternion.Euler(90f, 0f, 0f));

        mapCamera = cameraObj.AddComponent<Camera>();
        mapCamera.orthographic = true;
        mapCamera.orthographicSize = areaSize / 2f;
        mapCamera.nearClipPlane = 0.3f;
        mapCamera.farClipPlane = cameraHeight + 10f;
        mapCamera.clearFlags = CameraClearFlags.SolidColor;
        mapCamera.backgroundColor = Color.black;
        mapCamera.cullingMask = cullingMask;
        mapCamera.targetTexture = renderTexture;
    }

    private void OnDestroy()
    {
        if (mapCamera != null)
            Destroy(mapCamera.gameObject);

        if (renderTexture != null)
            renderTexture.Release();
    }
}
