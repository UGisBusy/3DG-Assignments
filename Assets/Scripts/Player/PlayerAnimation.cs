using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerAnimation : MonoBehaviour
{
    const string CONTROLLER_PATH = "Animations/PlayerAnimator";
    const float RUN_ANIM_SPEED = 6f;
    const float MAX_ANIM_SPEED_SCALE = 2f;
    const float IDLE_SPEED = 0.05f; // below this horizontal speed the player counts as standing still

    static readonly int SpeedHash = Animator.StringToHash("Speed");
    static readonly int MoveSpeedScaleHash = Animator.StringToHash("MoveSpeedScale");

    // when off, the animator pauses while standing still, so the model holds a pose
    [SerializeField]
    bool playIdleAnimation = false;

    Rigidbody rb;
    Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
            throw new System.NullReferenceException("player model animator is null");

        RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>(
            CONTROLLER_PATH
        );
        if (controller == null)
            throw new System.NullReferenceException(
                $"no animator controller at Resources/{CONTROLLER_PATH}"
            );

        animator.runtimeAnimatorController = controller;
        animator.applyRootMotion = false;
    }

    private void Update()
    {
        Vector3 velocity = rb.linearVelocity;
        float speed = new Vector3(velocity.x, 0f, velocity.z).magnitude;

        animator.SetFloat(SpeedHash, speed);
        animator.SetFloat(
            MoveSpeedScaleHash,
            Mathf.Clamp(speed / RUN_ANIM_SPEED, 1f, MAX_ANIM_SPEED_SCALE)
        );

        bool isIdle = speed < IDLE_SPEED;
        animator.speed = (isIdle && !playIdleAnimation) ? 0f : 1f;
    }
}
