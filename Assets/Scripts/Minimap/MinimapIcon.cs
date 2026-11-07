using System.Collections.Generic;
using UnityEngine;

public enum MinimapIconShape
{
    Circle,
    Arrow,   // points the way the target faces (used for the player)
    Diamond,
}

// Flat colored marker drawn above a target, on the "MiniMap" layer.
// Only the minimap camera renders that layer, so the main view never sees it.
// The icon is a separate root object (not a child) so the target's scale doesn't
// change its size; it follows the target, hides with it and is destroyed with it.
public class MinimapIcon : MonoBehaviour
{
    public const string LAYER_NAME = "MiniMap";

    [SerializeField]
    private MinimapIconShape shape = MinimapIconShape.Circle;

    [SerializeField]
    private Color color = Color.white;

    [Tooltip("Icon width in world units.")]
    [SerializeField]
    private float size = 2f;

    [Tooltip("Higher numbers draw on top of lower ones.")]
    [SerializeField]
    private int sortingOrder = 0;

    [Tooltip("World height above the target, so walls don't cover the icon.")]
    [SerializeField]
    private float height = 15f;

    private Transform icon;

    // shared sprites, generated once per shape
    private static readonly Dictionary<MinimapIconShape, Sprite> sprites = new();

    // add an icon from code, e.g. MinimapIcon.Attach(gameObject, MinimapIconShape.Circle, Color.red)
    public static MinimapIcon Attach(GameObject target, MinimapIconShape shape, Color color, float size = 2f, int sortingOrder = 0)
    {
        var minimapIcon = target.GetComponent<MinimapIcon>();
        if (minimapIcon != null)
            return minimapIcon; // already set up on the prefab, keep its settings

        minimapIcon = target.AddComponent<MinimapIcon>();
        minimapIcon.shape = shape;
        minimapIcon.color = color;
        minimapIcon.size = size;
        minimapIcon.sortingOrder = sortingOrder;
        return minimapIcon;
    }

    // built in Start (not Awake) so the values set by Attach are used
    private void Start()
    {
        int layer = LayerMask.NameToLayer(LAYER_NAME);
        if (layer < 0)
        {
            Debug.LogError($"Layer \"{LAYER_NAME}\" is missing. Add it in Project Settings > Tags and Layers.");
            return;
        }

        // keep icons out of the main view (done here, since the Minimap UI may still be hidden, e.g. on the intro)
        if (Camera.main != null)
            Camera.main.cullingMask &= ~(1 << layer);

        var iconObject = new GameObject($"{name} MinimapIcon") { layer = layer };
        icon = iconObject.transform;
        icon.localScale = Vector3.one * size;

        var spriteRenderer = iconObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = GetSprite(shape);
        spriteRenderer.color = color;
        spriteRenderer.sortingOrder = sortingOrder;
        spriteRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        spriteRenderer.receiveShadows = false;

        Follow();
    }

    private void LateUpdate() => Follow();

    private void OnEnable()
    {
        if (icon != null) icon.gameObject.SetActive(true);
    }

    // e.g. a collected item gets deactivated, so its icon disappears too
    private void OnDisable()
    {
        if (icon != null) icon.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (icon != null) Destroy(icon.gameObject);
    }

    private void Follow()
    {
        if (icon == null) return;

        icon.position = transform.position + Vector3.up * height;

        // lie flat facing up; the arrow also turns with the target
        float yaw = shape == MinimapIconShape.Arrow ? transform.eulerAngles.y : 0f;
        icon.rotation = Quaternion.Euler(90f, yaw, 0f);
    }

    // white shape on a transparent 64x64 texture, tinted later by SpriteRenderer.color
    private static Sprite GetSprite(MinimapIconShape shape)
    {
        if (sprites.TryGetValue(shape, out Sprite cached) && cached != null)
            return cached;

        const int res = 64;
        var texture = new Texture2D(res, res, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear,
        };

        var pixels = new Color32[res * res];
        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                // u, v in [-1, 1], (0, 0) at the center, +v is "forward"
                float u = (x + 0.5f) / res * 2f - 1f;
                float v = (y + 0.5f) / res * 2f - 1f;
                bool inside = shape switch
                {
                    MinimapIconShape.Arrow => Mathf.Abs(u) <= (1f - v) * 0.5f, // triangle, tip at the top
                    MinimapIconShape.Diamond => Mathf.Abs(u) + Mathf.Abs(v) <= 1f,
                    _ => u * u + v * v <= 1f,
                };
                pixels[y * res + x] = inside ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 0);
            }
        }
        texture.SetPixels32(pixels);
        texture.Apply();

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f), res);
        sprites[shape] = sprite;
        return sprite;
    }
}
