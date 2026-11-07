using UnityEngine;

// Slowly circles the camera around the level while the intro screen is shown.
// Put it on the same object as OrbitCamera: it turns OrbitCamera off during the
// intro and back on when the intro closes.
public class IntroCamera : MonoBehaviour
{
    [Tooltip("Point to circle around. Leave empty to use the center of all \"Wall\"-tagged objects (the whole level).")]
    [SerializeField]
    private Transform pivot;

    [Tooltip("Distance from the center. 0 = fit the level automatically.")]
    [SerializeField]
    private float radius = 0f;

    [Tooltip("Height above the center. 0 = automatic.")]
    [SerializeField]
    private float height = 0f;

    [Tooltip("Rotation speed in degrees per second.")]
    [SerializeField]
    private float speed = 8f;

    private OrbitCamera orbitCamera;
    private Vector3 center;
    private float angle;
    private bool isShowing;

    private void Awake()
    {
        orbitCamera = GetComponent<OrbitCamera>();

        // minimap icons float above the level; this camera must never draw them
        int iconLayer = LayerMask.NameToLayer(MinimapIcon.LAYER_NAME);
        if (iconLayer >= 0 && TryGetComponent(out Camera cam))
            cam.cullingMask &= ~(1 << iconLayer);

        UIEvents.ShowIntro += StartShow;
        UIEvents.HideIntro += StopShow;
    }

    private void OnDestroy()
    {
        UIEvents.ShowIntro -= StartShow;
        UIEvents.HideIntro -= StopShow;
    }

    private void StartShow()
    {
        FitLevel();

        isShowing = true;
        if (orbitCamera != null) orbitCamera.enabled = false;
    }

    private void StopShow()
    {
        isShowing = false;
        if (orbitCamera != null) orbitCamera.enabled = true;
    }

    private void LateUpdate()
    {
        if (!isShowing) return;

        angle += speed * Time.deltaTime;

        Vector3 offset = Quaternion.Euler(0f, angle, 0f) * Vector3.back * radius;
        transform.position = center + offset + Vector3.up * height;
        transform.LookAt(center);
    }

    // pick the center and, if not set, a distance that shows the whole level
    private void FitLevel()
    {
        Bounds bounds = pivot != null ? new Bounds(pivot.position, Vector3.zero) : WallBounds();
        center = bounds.center;

        float levelSize = Mathf.Max(bounds.extents.x, bounds.extents.z, 10f);
        if (radius <= 0f) radius = levelSize * 1.2f;
        if (height <= 0f) height = levelSize * 0.8f;
    }

    // combined bounds of every wall, i.e. roughly the whole level
    private static Bounds WallBounds()
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        bool hasAny = false;

        foreach (GameObject wall in GameObject.FindGameObjectsWithTag("Wall"))
        {
            foreach (Renderer r in wall.GetComponentsInChildren<Renderer>())
            {
                if (!hasAny) bounds = r.bounds;
                else bounds.Encapsulate(r.bounds);
                hasAny = true;
            }
        }

        return bounds;
    }
}
