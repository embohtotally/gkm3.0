// PlayerMovement.cs (Focus ONLY on Locomotion & its Animations)
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    [Tooltip("Speed multiplier when shielding (e.g., 0.3 for 30% speed). PlayerHealth will call UpdateShieldingStatus.")]
    [SerializeField] private float shieldSpeedMultiplier = 0.3f;

    // Public properties for other scripts to potentially read (e.g., for attack direction later)
    public Vector2 NormalizedMovementInput { get; private set; }
    public Vector2 LastFacedDirection { get; private set; }

    // Component References
    private Rigidbody2D rb;
    private Animator animator; // Public getter removed for now, as no other script *needs* it for this pure movement setup

    private float _actualSpeed; // Current effective speed after modifiers like shielding
    private bool _isCurrentlyShielding = false; // To be updated by PlayerHealth or similar script

    // Animator Parameter Names (using const strings for safety against typos)
    private const string AnimParamMoveX = "MoveX";
    private const string AnimParamMoveY = "MoveY";
    private const string AnimParamSpeed = "Speed";
    private const string AnimParamLastMoveX = "LastMoveX";
    private const string AnimParamLastMoveY = "LastMoveY";

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rb == null) Debug.LogError("[PlayerMovement] Rigidbody2D component not found!");
        if (animator == null) Debug.LogError("[PlayerMovement] Animator component not found!");

        LastFacedDirection = Vector2.down; // Default facing direction
        _actualSpeed = moveSpeed; // Initialize with base speed
    }

    void Update()
    {
        GatherInput();
        CalculateActualSpeed(); // Calculate speed considering shielding
        UpdateAnimatorParameters();
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    void GatherInput()
    {
        float horizontalInput = 0f;
        float verticalInput = 0f;

        // Using GetKey for continuous input feel
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) verticalInput = 1f;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) verticalInput = -1f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horizontalInput = -1f;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) horizontalInput = 1f;

        NormalizedMovementInput = new Vector2(horizontalInput, verticalInput).normalized;
    }

    void CalculateActualSpeed()
    {
        // We can re-introduce a run toggle later if needed. For now, one base speed.
        float baseSpeed = moveSpeed;
        _actualSpeed = _isCurrentlyShielding ? (baseSpeed * shieldSpeedMultiplier) : baseSpeed;
    }

    void UpdateAnimatorParameters()
    {
        if (animator == null) return;

        // --- Parameters for Movement Blend Tree ---
        // These reflect the current frame's movement attempt.
        animator.SetFloat(AnimParamMoveX, NormalizedMovementInput.x);
        animator.SetFloat(AnimParamMoveY, NormalizedMovementInput.y);

        // --- Speed Parameter for Idle/Movement State Transition ---
        // This is 0 if no input, 1 if there is input.
        // If shielding and moving, Speed is still > 0 so movement animation plays, actual speed is just slower.
        float currentSpeedFactor = NormalizedMovementInput.sqrMagnitude > 0.01f ? 1f : 0f;
        animator.SetFloat(AnimParamSpeed, currentSpeedFactor);

        // Debug.Log($"Animator Params Set -> MoveX: {NormalizedMovementInput.x}, MoveY: {NormalizedMovementInput.y}, Speed: {currentSpeedFactor}");

        // --- Parameters for Idle Facing Blend Tree ---
        if (NormalizedMovementInput.sqrMagnitude > 0.01f) // If there is movement
        {
            LastFacedDirection = NormalizedMovementInput; // Update the last direction faced
            animator.SetFloat(AnimParamLastMoveX, LastFacedDirection.x);
            animator.SetFloat(AnimParamLastMoveY, LastFacedDirection.y);
            // Debug.Log($"LastMove Updated -> LastX: {LastFacedDirection.x}, LastY: {LastFacedDirection.y}");
        }
        // When idle (NormalizedMovementInput.sqrMagnitude is 0), LastMoveX/Y animator parameters are NOT updated by this block.
        // This means they retain their previous values, allowing the idle animation to face the last direction moved.
    }

    void ApplyMovement()
    {
        if (rb == null) return;

        if (NormalizedMovementInput.sqrMagnitude > 0.01f)
        {
            Vector2 velocity = NormalizedMovementInput * _actualSpeed; // Use the calculated actual speed
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
        else
        {
            rb.velocity = Vector2.zero; // Explicitly stop if no input
        }
    }

    // This public method allows PlayerHealth (or other scripts) to tell PlayerMovement about shielding status
    public void UpdateShieldingStatus(bool isShielding)
    {
        _isCurrentlyShielding = isShielding;
        // Speed will be recalculated in the next Update() call by CalculateActualSpeed()
        Debug.Log($"[PlayerMovement] Shielding status set to: {isShielding}. Effective speed will be adjusted on next Update.");
    }
}