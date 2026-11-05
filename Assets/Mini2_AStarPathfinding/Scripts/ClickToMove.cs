using UnityEngine;
using UnityEngine.InputSystem;

namespace Mini2
{
    // Left-click: cast a ray from the mouse and send the hit point to the PathFollower.
    public class ClickToMove : MonoBehaviour
    {
        public PathFollower follower;
        public Camera cam;
        public float maxDistance = 500f;

        void Awake()
        {
            if (cam == null) cam = Camera.main;
            if (follower == null) follower = GetComponent<PathFollower>();
        }

        void Update()
        {
            var mouse = Mouse.current;
            if (mouse == null || cam == null || follower == null) return;
            if (!mouse.leftButton.wasPressedThisFrame) return;

            Ray ray = cam.ScreenPointToRay(mouse.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, ~0, QueryTriggerInteraction.Ignore))
                follower.SetDestination(hit.point);
        }

        void OnGUI()
        {
            if (follower == null) return;
            GUI.Label(new Rect(10, 10, 600, 25), follower.Status);
        }
    }
}
