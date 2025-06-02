using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] // Good to keep if you use Rigidbody2D
[RequireComponent(typeof(Animator))]   // Add this for the Animatora
public class CharacterController2D : MonoBehaviour
{
    // Movement
    private Rigidbody2D rigidbody2D;
    private Vector2 moveDir;
    public float moveSpeed = 5f;

    // Animation
    private Animator animator;
    private Vector2 lastFacedDirection;

    // Animator Parameter Names (copied from PlayerMovement.cs)
    private const string AnimParamMoveX = "MoveX";
    private const string AnimParamMoveY = "MoveY";
    private const string AnimParamSpeed = "Speed";
    private const string AnimParamLastMoveX = "LastMoveX";
    private const string AnimParamLastMoveY = "LastMoveY";

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Get the Animator component

        if (rigidbody2D == null)
        {
            Debug.LogError("[CharacterController2D] Rigidbody2D component not found!");
        }
        if (animator == null)
        {
            Debug.LogError("[CharacterController2D] Animator component not found!");
        }

        lastFacedDirection = Vector2.down; // Default facing direction (e.g., for initial idle)
                                           // You might want Vector2.right if it's a side-scroller starting facing right
    }

    private void Update()
    {
        // --- Gather Input for Movement ---
        float moveX = 0f;
        float moveY = 0f;

        // Using GetKey for continuous input feel
        // (You can also use Input.GetAxis("Horizontal") and Input.GetAxis("Vertical") for smoother input)
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) moveY = +1f;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) moveY = -1f;
        // Ensure separate if conditions for X and Y if you want them to be independent
        // and not else-if, if you want diagonal input from single axis keys to be possible
        // The PlayerMovement script used separate if blocks which is fine.
        // For simplicity, I'll use a structure that allows independent X and Y:

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) moveX = -1f;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) moveX = +1f;

        // If you want W/S to take precedence or A/D for X/Y respectively, use your original structure:
        // if (Input.GetKey(KeyCode.W)) moveY = +1f;
        // if (Input.GetKey(KeyCode.S)) moveY = -1f;
        // if (Input.GetKey(KeyCode.A)) moveX = -1f;
        // if (Input.GetKey(KeyCode.D)) moveX = +1f;


        moveDir = new Vector2(moveX, moveY).normalized;

        // --- Update Animator Parameters ---
        UpdateAnimationParameters();
    }

    private void FixedUpdate()
    {
        if (rigidbody2D != null)
        {
            // Apply movement using velocity
            rigidbody2D.velocity = moveDir * moveSpeed;
        }
    }

    void UpdateAnimationParameters()
    {
        if (animator == null) return;

        // --- Parameters for Movement Blend Tree ---
        // These reflect the current frame's movement attempt.
        animator.SetFloat(AnimParamMoveX, moveDir.x);
        animator.SetFloat(AnimParamMoveY, moveDir.y);

        // --- Speed Parameter for Idle/Movement State Transition ---
        // This is 0 if no input, 1 if there is input.
        float currentSpeedFactor = moveDir.sqrMagnitude > 0.01f ? 1f : 0f;
        animator.SetFloat(AnimParamSpeed, currentSpeedFactor);

        // --- Parameters for Idle Facing Blend Tree ---
        if (moveDir.sqrMagnitude > 0.01f) // If there is movement
        {
            lastFacedDirection = moveDir; // Update the last direction faced
                                          // No need to set LastMoveX/Y here *again* as they are set below
                                          // using the updated lastFacedDirection.
                                          // The original PlayerMovement script sets them inside this if, which is fine.
        }

        // Set LastMoveX and LastMoveY based on the lastFacedDirection.
        // These parameters will retain their values when idle if lastFacedDirection is only updated during movement.
        animator.SetFloat(AnimParamLastMoveX, lastFacedDirection.x);
        animator.SetFloat(AnimParamLastMoveY, lastFacedDirection.y);

        // Optional Debug Log (from PlayerMovement.cs, useful for testing)
        // Debug.Log($"Animator Params Set -> MoveX: {moveDir.x}, MoveY: {moveDir.y}, Speed: {currentSpeedFactor}, LastMoveX: {lastFacedDirection.x}, LastMoveY: {lastFacedDirection.y}");
    }
}