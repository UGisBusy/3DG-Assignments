using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerAnimation : MonoBehaviour
{
    const string CONTROLLER_PATH = "Animations/PlayerAnimator";
    const float RUN_ANIM_SPEED = 6f;
    const float MAX_ANIM_SPEED_SCALE = 2f;
    const float AIRBORNE_DELAY = 0.15f;
    const float JUMP_VELOCITY_THRESHOLD = 1f;
    const float FACE_MOVE_MIN_SPEED = 0.5f;
    const float TURN_SPEED = 720f;

    static readonly int SpeedHash = Animator.StringToHash("Speed");
    static readonly int GroundedHash = Animator.StringToHash("Grounded");
    static readonly int MoveSpeedScaleHash = Animator.StringToHash("MoveSpeedScale");

    Rigidbody rb;
    PlayerMovement movement;
    Animator animator;
    Transform model;
    Renderer[] modelRenderers;
    ShadowCastingMode[] shadowModes;
    float airTime;

    public void SetModelVisible(bool visible)
    {
        for (int i = 0; i < modelRenderers.Length; i++)
            modelRenderers[i].shadowCastingMode = visible ? shadowModes[i] : ShadowCastingMode.ShadowsOnly;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<PlayerMovement>();
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
            throw new System.NullReferenceException("player model animator is null");

        RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>(CONTROLLER_PATH);
        if (controller == null)
            throw new System.NullReferenceException($"no animator controller at Resources/{CONTROLLER_PATH}");

        animator.runtimeAnimatorController = controller;
        animator.applyRootMotion = false;
        model = animator.transform;

        modelRenderers = model.GetComponentsInChildren<Renderer>();
        shadowModes = new ShadowCastingMode[modelRenderers.Length];
        for (int i = 0; i < modelRenderers.Length; i++)
            shadowModes[i] = modelRenderers[i].shadowCastingMode;
    }

    private void Update()
    {
        Vector3 velocity = rb.linearVelocity;
        Vector3 horizontal = new Vector3(velocity.x, 0f, velocity.z);
        float speed = horizontal.magnitude;

        UpdateAirTime(velocity.y);

        animator.SetFloat(SpeedHash, speed);
        animator.SetFloat(MoveSpeedScaleHash, Mathf.Clamp(speed / RUN_ANIM_SPEED, 1f, MAX_ANIM_SPEED_SCALE));
        animator.SetBool(GroundedHash, airTime < AIRBORNE_DELAY);

        FaceMoveDirection(horizontal, speed);
    }

    private void UpdateAirTime(float verticalSpeed)
    {
        if (verticalSpeed > JUMP_VELOCITY_THRESHOLD)
        {
            airTime = AIRBORNE_DELAY;
            return;
        }

        airTime = movement.IsGrounded ? 0f : airTime + Time.deltaTime;
    }

    private void FaceMoveDirection(Vector3 horizontal, float speed)
    {
        Vector3 direction = speed > FACE_MOVE_MIN_SPEED ? horizontal : transform.forward;
        Quaternion desired = Quaternion.LookRotation(direction, Vector3.up);
        model.rotation = Quaternion.RotateTowards(model.rotation, desired, TURN_SPEED * Time.deltaTime);
    }
}
