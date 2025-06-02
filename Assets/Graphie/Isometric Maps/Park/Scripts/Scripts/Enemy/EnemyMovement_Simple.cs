// EnemyMovement_Simple.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class EnemyMovement_Simple : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f; // Default speed, often overridden by AI states
    [Tooltip("How close the enemy mover considers 'at the point' for its own operations.")]
    public float stopThreshold = 0.1f;

    public Vector2 CurrentVelocity { get; private set; }
    public Vector2 FacingDirection { get; private set; }

    private Rigidbody2D rb;
    private Animator animator;

    private readonly int animMoveX = Animator.StringToHash("MoveX");
    private readonly int animMoveY = Animator.StringToHash("MoveY");
    private readonly int animSpeed = Animator.StringToHash("Speed");
    private readonly int animLastMoveX = Animator.StringToHash("LastMoveX");
    private readonly int animLastMoveY = Animator.StringToHash("LastMoveY");

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rb == null) Debug.LogError($"[{gameObject.name}/EnemyMovement_Simple] Rigidbody2D not found!", this);
        if (animator == null) Debug.LogWarning($"[{gameObject.name}/EnemyMovement_Simple] Animator not found. Animations will not play.", this);

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.gravityScale = 0;
        }
        FacingDirection = transform.up;
    }

    public void MoveTowardsPoint(Vector3 targetPosition, float speed)
    {
        if (rb == null) return;

        if (speed <= 0.01f)
        {
            StopMovement();
            return;
        }

        Vector2 direction = ((Vector2)targetPosition - rb.position).normalized;
        CurrentVelocity = direction * speed;

        if (direction.sqrMagnitude > 0.01f)
        {
            FacingDirection = direction;
        }
        UpdateAnimatorParameters(direction, speed);
    }

    public void StopMovement()
    {
        CurrentVelocity = Vector2.zero;
        if (rb != null) rb.velocity = Vector2.zero;
        UpdateAnimatorParameters(Vector2.zero, 0f);
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        if (CurrentVelocity.sqrMagnitude > 0.01f)
        {
            rb.MovePosition(rb.position + CurrentVelocity * Time.fixedDeltaTime);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void UpdateAnimatorParameters(Vector2 direction, float currentSpeedValue)
    {
        if (animator == null) return;

        animator.SetFloat(animMoveX, direction.x);
        animator.SetFloat(animMoveY, direction.y);
        animator.SetFloat(animSpeed, currentSpeedValue > 0.01f ? 1f : 0f);

        if (direction.sqrMagnitude > 0.01f)
        {
            animator.SetFloat(animLastMoveX, direction.x);
            animator.SetFloat(animLastMoveY, direction.y);
        }
    }

    void OnDisable()
    {
        if (rb != null) CurrentVelocity = Vector2.zero;
        if (animator != null) UpdateAnimatorParameters(Vector2.zero, 0f);
    }
}