using UnityEngine;

// Draws the area the player's sword sweep covers: a flat, fading arc on the ground.
// PlayerAttack calls Show() on every swing with its range, angle and direction.
public class AttackArcVisual : MonoBehaviour
{
    [SerializeField]
    private Color color = new Color(1f, 0.9f, 0.5f, 0.6f);

    [Tooltip("Seconds the arc takes to fade out.")]
    [SerializeField]
    private float fadeTime = 0.25f;

    [Tooltip("Inner edge as a fraction of the range; 0 = filled fan, higher = thinner slash.")]
    [SerializeField, Range(0f, 0.9f)]
    private float innerRatio = 0.3f;

    [Tooltip("Optional. Leave empty to use a built-in transparent material.")]
    [SerializeField]
    private Material material;

    const int SEGMENTS = 24;

    private Transform arc;
    private Mesh mesh;
    private Material instanceMaterial;
    private Collider body;
    private Quaternion facing;
    private float timeLeft;
    private float builtRange = -1f;
    private float builtAngle = -1f;

    private void Awake()
    {
        body = GetComponent<Collider>();

        var arcObject = new GameObject("AttackArc", typeof(MeshFilter), typeof(MeshRenderer));
        arc = arcObject.transform;
        arc.SetParent(transform, false);

        mesh = new Mesh { name = "AttackArc" };
        arcObject.GetComponent<MeshFilter>().sharedMesh = mesh;

        instanceMaterial = material != null ? new Material(material) : new Material(Shader.Find("Sprites/Default"));
        var meshRenderer = arcObject.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = instanceMaterial;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;

        arcObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Destroy(mesh);
        Destroy(instanceMaterial);
    }

    // range/angle: what the sweep covers; direction: where the swing points (flat)
    public void Show(float range, float angle, Vector3 direction)
    {
        if (!Mathf.Approximately(range, builtRange) || !Mathf.Approximately(angle, builtAngle))
            BuildMesh(range, angle);

        direction.y = 0f;
        facing = direction.sqrMagnitude > 0.0001f ? Quaternion.LookRotation(direction) : transform.rotation;
        timeLeft = fadeTime;

        arc.gameObject.SetActive(true);
        LateUpdate();
    }

    private void LateUpdate()
    {
        if (!arc.gameObject.activeSelf) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            arc.gameObject.SetActive(false);
            return;
        }

        // just above the feet, pointing where the swing went even while the body turns
        float groundY = body != null ? body.bounds.min.y : transform.position.y;
        arc.position = new Vector3(transform.position.x, groundY + 0.05f, transform.position.z);
        arc.rotation = facing;

        Color c = color;
        c.a *= timeLeft / fadeTime;
        instanceMaterial.color = c;
    }

    // flat ring slice around +Z, from innerRatio*range to range;
    // inner edge transparent, outer edge solid, so it reads like a sword swoosh
    private void BuildMesh(float range, float angle)
    {
        builtRange = range;
        builtAngle = angle;

        var vertices = new Vector3[(SEGMENTS + 1) * 2];
        var colors = new Color[vertices.Length];
        var triangles = new int[SEGMENTS * 6];

        float inner = range * innerRatio;
        for (int i = 0; i <= SEGMENTS; i++)
        {
            float a = Mathf.Deg2Rad * Mathf.Lerp(-angle / 2f, angle / 2f, (float)i / SEGMENTS);
            var dir = new Vector3(Mathf.Sin(a), 0f, Mathf.Cos(a));

            vertices[i * 2] = dir * inner;
            vertices[i * 2 + 1] = dir * range;
            colors[i * 2] = new Color(1f, 1f, 1f, 0f);
            colors[i * 2 + 1] = Color.white;
        }

        for (int i = 0; i < SEGMENTS; i++)
        {
            int v = i * 2, t = i * 6;
            triangles[t] = v;
            triangles[t + 1] = v + 1;
            triangles[t + 2] = v + 3;
            triangles[t + 3] = v;
            triangles[t + 4] = v + 3;
            triangles[t + 5] = v + 2;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }
}
