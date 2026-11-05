using UnityEngine;

namespace Mini2
{
    // Code-driven walk cycle for the primitive character.
    // Legs and arms swing about their local x-axis with a sine wave, opposite to each other.
    // The cycle speed follows how fast the character actually moves, and the swing
    // fades in when walking starts and back to the rest pose when it stops.
    public class WalkAnimator : MonoBehaviour
    {
        public Transform body;
        public Transform leftLeg, rightLeg, leftArm, rightArm; // hip / shoulder pivots

        [Tooltip("Metres covered by one full cycle (two steps).")]
        public float strideLength = 1.2f;
        public float legSwing = 35f;    // degrees
        public float armSwing = 30f;    // degrees
        public float bobHeight = 0.05f; // body rises once per step
        public float blendSpeed = 6f;   // how fast the swing fades in / out

        Vector3 lastPosition;
        float phase;  // radians
        float amount; // 0 = standing, 1 = full walk
        Vector3 bodyRest;
        Quaternion leftLegRest, rightLegRest, leftArmRest, rightArmRest;

        void Awake()
        {
            lastPosition = transform.position;
            if (body != null) bodyRest = body.localPosition;
            if (leftLeg != null) leftLegRest = leftLeg.localRotation;
            if (rightLeg != null) rightLegRest = rightLeg.localRotation;
            if (leftArm != null) leftArmRest = leftArm.localRotation;
            if (rightArm != null) rightArmRest = rightArm.localRotation;
        }

        void LateUpdate()
        {
            float dt = Time.deltaTime;
            if (dt <= 0f) return;

            Vector3 delta = transform.position - lastPosition;
            delta.y = 0f;
            lastPosition = transform.position;
            float speed = delta.magnitude / dt;

            amount = Mathf.MoveTowards(amount, speed > 0.05f ? 1f : 0f, blendSpeed * dt);
            phase = Mathf.Repeat(phase + speed / strideLength * 2f * Mathf.PI * dt, 2f * Mathf.PI);

            float s = Mathf.Sin(phase) * amount;
            Swing(leftLeg, leftLegRest, s * legSwing);
            Swing(rightLeg, rightLegRest, -s * legSwing);
            Swing(leftArm, leftArmRest, -s * armSwing);
            Swing(rightArm, rightArmRest, s * armSwing);

            if (body != null)
                body.localPosition = bodyRest + Vector3.up * (Mathf.Abs(Mathf.Sin(phase)) * bobHeight * amount);
        }

        static void Swing(Transform t, Quaternion rest, float angle)
        {
            if (t != null) t.localRotation = rest * Quaternion.AngleAxis(angle, Vector3.right);
        }
    }
}
