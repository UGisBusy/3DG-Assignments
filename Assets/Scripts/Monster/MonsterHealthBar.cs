using UnityEngine;
using UnityEngine.UI;

// Life bar floating above a monster's head.
// Builds its own world-space canvas at runtime, so no prefab setup is needed,
// and always turns to face the camera.
public class MonsterHealthBar : MonoBehaviour
{
    [Tooltip("Height above the monster's pivot, in local units.")]
    [SerializeField]
    private float heightOffset = 1.5f;

    [Tooltip("Bar size in world units.")]
    [SerializeField]
    private Vector2 size = new Vector2(1.2f, 0.15f);

    [Tooltip("Gap between the background edge and the fill, in world units.")]
    [SerializeField]
    private float padding = 0.02f;

    [SerializeField]
    private Color backgroundColor = new Color(0f, 0f, 0f, 0.6f);

    [SerializeField]
    private Color fillColor = new Color(0.85f, 0.15f, 0.15f);

    private RectTransform root;
    private RectTransform fill;
    private Transform cam;

    private void Awake()
    {
        Build();
        SetRatio(1f);
    }

    private void LateUpdate()
    {
        if (cam == null)
        {
            if (Camera.main == null) return;
            cam = Camera.main.transform;
        }

        // billboard: same rotation as the camera so the bar always faces the screen
        root.rotation = cam.rotation;
    }

    // ratio: 1 = full health, 0 = dead
    public void SetRatio(float ratio)
    {
        fill.anchorMax = new Vector2(Mathf.Clamp01(ratio), 1f);
    }

    private void Build()
    {
        // world-space canvas holding the bar
        var canvasObject = new GameObject("HealthBar", typeof(RectTransform), typeof(Canvas));
        root = (RectTransform)canvasObject.transform;
        root.SetParent(transform, false);
        root.localPosition = Vector3.up * heightOffset;
        root.sizeDelta = size;
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;

        // dark background filling the whole canvas
        RectTransform background = CreateImage("Background", root, backgroundColor);
        Stretch(background, 0f);

        // padded area that the fill grows/shrinks inside
        var fillArea = new GameObject("FillArea", typeof(RectTransform)).GetComponent<RectTransform>();
        fillArea.SetParent(background, false);
        Stretch(fillArea, padding);

        // fill anchored to the left; SetRatio moves its right edge
        fill = CreateImage("Fill", fillArea, fillColor);
        Stretch(fill, 0f);

        // UI layer keeps the bar off the minimap
        int uiLayer = LayerMask.NameToLayer("UI");
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
            t.gameObject.layer = uiLayer;
    }

    private static RectTransform CreateImage(string name, Transform parent, Color color)
    {
        var imageObject = new GameObject(name, typeof(RectTransform), typeof(Image));
        imageObject.transform.SetParent(parent, false);

        var image = imageObject.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;

        return (RectTransform)imageObject.transform;
    }

    // fill the parent rect, inset by `inset` on every side
    private static void Stretch(RectTransform rect, float inset)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(inset, inset);
        rect.offsetMax = new Vector2(-inset, -inset);
    }
}
