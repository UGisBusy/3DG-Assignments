using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Top-down minimap. Put this on a RawImage inside the GamePanel.
// It creates an orthographic camera that follows the player from above, renders it
// into a RenderTexture shown by the RawImage, and toggles with the M key.
// Icons (MinimapIcon) are on the "MiniMap" layer: drawn here, hidden from the main camera.
[RequireComponent(typeof(RawImage))]
public class Minimap : MonoBehaviour
{
    [Tooltip("What the map centers on. Found automatically if left empty.")]
    [SerializeField]
    private Transform target;

    [Tooltip("Half the width of the area shown, in world units. Bigger = more zoomed out.")]
    [SerializeField]
    private float viewRadius = 25f;

    [SerializeField]
    private float cameraHeight = 60f;

    [SerializeField]
    private int textureResolution = 512;

    [SerializeField]
    private Color backgroundColor = new Color(0.1f, 0.1f, 0.15f, 1f);

    private RawImage image;
    private Camera mapCamera;
    private RenderTexture renderTexture;
    private InputAction toggleAction;
    private bool isShown = true;

    private void Awake()
    {
        image = GetComponent<RawImage>();

        if (target == null)
        {
            var player = FindAnyObjectByType<Player>();
            if (player != null) target = player.transform;
            else Debug.LogError("Minimap has no target and found no Player in the scene");
        }

        int iconLayer = LayerMask.NameToLayer(MinimapIcon.LAYER_NAME);
        int uiLayer = LayerMask.NameToLayer("UI");

        // main camera must not see the icons floating above everything
        if (Camera.main != null && iconLayer >= 0)
            Camera.main.cullingMask &= ~(1 << iconLayer);

        renderTexture = new RenderTexture(textureResolution, textureResolution, 16) { name = "MinimapTexture" };
        image.texture = renderTexture;

        mapCamera = new GameObject("MinimapCamera").AddComponent<Camera>();
        mapCamera.orthographic = true;
        mapCamera.orthographicSize = viewRadius;
        mapCamera.nearClipPlane = 0.3f;
        mapCamera.farClipPlane = cameraHeight * 2f;
        mapCamera.clearFlags = CameraClearFlags.SolidColor;
        mapCamera.backgroundColor = backgroundColor;
        mapCamera.cullingMask = ~(1 << uiLayer); // everything except world-space UI like monster life bars
        mapCamera.targetTexture = renderTexture;
        mapCamera.depth = -10; // render before the main camera
        mapCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f); // straight down, north up

        toggleAction = new InputAction(binding: "<Keyboard>/m");
        toggleAction.performed += _ => Toggle();
    }

    // follows GamePanel: active while playing, off on win/lose screens
    private void OnEnable()
    {
        toggleAction.Enable();
        Apply();
    }

    private void OnDisable()
    {
        toggleAction.Disable();
        if (mapCamera != null) mapCamera.enabled = false;
    }

    private void OnDestroy()
    {
        toggleAction?.Dispose();

        if (mapCamera != null) Destroy(mapCamera.gameObject);
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
    }

    private void LateUpdate()
    {
        if (target == null || !isShown) return;

        Vector3 position = target.position;
        mapCamera.transform.position = new Vector3(position.x, position.y + cameraHeight, position.z);
    }

    private void Toggle()
    {
        isShown = !isShown;
        Apply();
    }

    // hide the image and stop rendering the camera while the map is toggled off
    private void Apply()
    {
        image.enabled = isShown;
        mapCamera.enabled = isShown;
    }
}
