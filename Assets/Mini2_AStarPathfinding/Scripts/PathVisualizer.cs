using System.Collections.Generic;
using UnityEngine;

namespace Mini2
{
    // Shows a path as small spheres placed every `spacing` metres, so a longer
    // path gets more spheres. Spheres the character has walked past are hidden.
    public class PathVisualizer : MonoBehaviour
    {
        public Material dotMaterial;
        public Material goalMaterial;

        public float spacing = 0.6f;
        public float dotSize = 0.2f;
        public float goalSize = 0.45f;
        public float height = 0.15f;

        readonly List<GameObject> dots = new List<GameObject>(); // reused between paths
        readonly List<float> dotDistance = new List<float>();    // distance of each dot from the path start
        int activeCount;
        GameObject goal;

        // Places the spheres along the polyline and returns how many were used.
        public int Show(List<Vector3> points)
        {
            activeCount = 0;
            float nextDot = spacing; // first dot one spacing ahead, not under the character's feet
            float walked = 0f;       // path length before the current segment

            for (int i = 1; i < points.Count; i++)
            {
                Vector3 a = points[i - 1], b = points[i];
                float len = Vector3.Distance(a, b);
                if (len < 0.0001f) continue;

                while (nextDot <= walked + len)
                {
                    Vector3 p = Vector3.Lerp(a, b, (nextDot - walked) / len);
                    PlaceDot(activeCount++, p, nextDot);
                    nextDot += spacing;
                }
                walked += len;
            }

            for (int i = activeCount; i < dots.Count; i++) dots[i].SetActive(false);

            if (goal == null) goal = MakeSphere("Goal", goalMaterial, goalSize);
            goal.transform.position = points[points.Count - 1] + Vector3.up * height;
            goal.SetActive(true);

            return activeCount;
        }

        public void HidePassed(float travelled)
        {
            for (int i = 0; i < activeCount; i++)
                if (dots[i].activeSelf && dotDistance[i] <= travelled)
                    dots[i].SetActive(false);
        }

        public void Clear()
        {
            foreach (var d in dots) d.SetActive(false);
            activeCount = 0;
            if (goal != null) goal.SetActive(false);
        }

        void PlaceDot(int index, Vector3 position, float distance)
        {
            if (index == dots.Count)
            {
                dots.Add(MakeSphere($"Dot_{index:000}", dotMaterial, dotSize));
                dotDistance.Add(0f);
            }
            dots[index].transform.position = position + Vector3.up * height;
            dots[index].SetActive(true);
            dotDistance[index] = distance;
        }

        GameObject MakeSphere(string name, Material material, float size)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = name;
            go.transform.SetParent(transform, false);
            go.transform.localScale = Vector3.one * size;
            Destroy(go.GetComponent<Collider>()); // no colliders: must not block clicks or the grid
            if (material != null) go.GetComponent<Renderer>().sharedMaterial = material;
            return go;
        }
    }
}
