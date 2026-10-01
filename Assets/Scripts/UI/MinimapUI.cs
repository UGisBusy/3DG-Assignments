using UnityEngine;
using UnityEngine.UI;

public class MinimapUI : MonoBehaviour
{
    [SerializeField] private RawImage mapImage;
    [SerializeField] private RectTransform playerMarker;
    Vector2 areaCenter = Vector2.zero;
    float areaSize = 200;
    float cameraHeight = 100f;
    int textureResolution = 512;

    Camera mapCamera;
    RenderTexture renderTexture;

    public void UpdatePlayer(Vector3 position, float yaw)
    {
        float u = (position.x - areaCenter.x) / areaSize + 0.5f;
        float v = (position.z - areaCenter.y) / areaSize + 0.5f;
        Vector2 anchor = new Vector2(Mathf.Clamp01(u), Mathf.Clamp01(v));

        playerMarker.anchorMin = anchor;
        playerMarker.anchorMax = anchor;
        playerMarker.anchoredPosition = Vector2.zero;
        playerMarker.localRotation = Quaternion.Euler(0f, 0f, -yaw);
    }

    private void Awake()
    {
        if (mapImage == null)
            throw new System.NullReferenceException("minimap image is null");

        if (playerMarker == null)
            throw new System.NullReferenceException("minimap player marker is null");

        renderTexture = new RenderTexture(textureResolution, textureResolution, 16);
        mapImage.texture = renderTexture;

        // fixed top-down camera covering the whole gameplay area
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
        mapCamera.cullingMask &= ~LayerMask.GetMask("UI");
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
