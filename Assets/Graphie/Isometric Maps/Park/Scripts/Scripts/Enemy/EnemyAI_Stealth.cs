// EnemyAI_Stealth.cs
using UnityEngine;
using UnityEngine.UI; // Required for Slider
using UnityEngine.SceneManagement;
using System.Collections;

// Updated Enum for more distinct states
public enum EnemyAI_State { Patrolling, SuspectingPlayer, ChasingPlayer, SearchingLastKnownPosition, ReturningToStartPoint }

[RequireComponent(typeof(EnemyMovement_Simple))]
[RequireComponent(typeof(EnemyPatrol_Simple))]
// No RequireComponent for EnemyVision, assign via Inspector from child
public class EnemyAI_Stealth : MonoBehaviour
{
    [Header("AI Behavior Settings")]
    public EnemyAI_State startingState = EnemyAI_State.Patrolling;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float catchDistance = 1f;
    
    [Header("Suspicion & Alertness")]
    [Tooltip("Time in seconds to go from '?' to '!' if player remains visible.")]
    [SerializeField] private float suspicionTimeToAlert = 1.0f;
    [Tooltip("Time in seconds enemy searches last known area after losing sight before returning to patrol.")]
    [SerializeField] private float searchTimeBeforePatrol = 3.0f;

    [Header("UI Indicators (Assign from Child Canvas)")]
    [SerializeField] private GameObject questionMarkIcon;
    [SerializeField] private GameObject exclamationMarkIcon;
    [SerializeField] private Slider detectionSlider;

    [Header("Component References")]
    [Tooltip("Drag the CHILD GameObject that has the EnemyVision script attached.")]
    [SerializeField] private EnemyVision visionScript; 

    private EnemyMovement_Simple mover;
    private EnemyPatrol_Simple patrolScript;
    private Transform playerTransform;
    private Vector3 initialPosition;
    private Animator animator; 

    private EnemyAI_State currentState;
    private float currentSuspicionLevel = 0f; // For '?' slider (0 to suspicionTimeToAlert)
    private float currentSearchTimer = 0f;    // For '!' back to '?' slider and search duration

    private Vector3 lastSightingPosition;

    void Awake()
    {
        mover = GetComponent<EnemyMovement_Simple>();
        patrolScript = GetComponent<EnemyPatrol_Simple>();
        animator = GetComponent<Animator>(); 

        if (visionScript == null) { Debug.LogError($"[{gameObject.name}/EnemyAI] EnemyVision script NOT ASSIGNED! AI Disabled.", this); enabled = false; return; }
        if (mover == null) { Debug.LogError($"[{gameObject.name}/EnemyAI] EnemyMovement_Simple not found! AI Disabled.", this); enabled = false; return;}
        if (patrolScript == null) { Debug.LogWarning($"[{gameObject.name}/EnemyAI] EnemyPatrol_Simple not found. Patrolling may be impaired.", this); }
        
        initialPosition = transform.position;
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
        else Debug.LogError($"[{gameObject.name}/EnemyAI] Player not found! Chasing impaired.", this);
        
        // Initialize UI elements
        if (questionMarkIcon != null) questionMarkIcon.SetActive(false);
        if (exclamationMarkIcon != null) exclamationMarkIcon.SetActive(false);
        if (detectionSlider != null)
        {
            detectionSlider.gameObject.SetActive(false);
            detectionSlider.minValue = 0;
            // maxValue will be set based on context (suspicion or search time)
        }

        currentState = (EnemyAI_State)(-1); 
        ChangeState(startingState); 
    }

    void Update()
    {
        if (!enabled || visionScript == null) return;

        if (playerTransform == null && (currentState == EnemyAI_State.ChasingPlayer || currentState == EnemyAI_State.SuspectingPlayer || currentState == EnemyAI_State.SearchingLastKnownPosition))
        {
            ChangeState(EnemyAI_State.ReturningToStartPoint);
            return; 
        }
        
        bool canCurrentlySeePlayer = (playerTransform != null) && visionScript.CanSeePlayer;
        // Debug.Log($"[{gameObject.name}/AI] Update: State='{currentState}', CanSeePlayer='{canCurrentlySeePlayer}', Suspicion: {currentSuspicionLevel}");

        switch (currentState)
        {
            case EnemyAI_State.Patrolling:
                if (canCurrentlySeePlayer) ChangeState(EnemyAI_State.SuspectingPlayer);
                break;

            case EnemyAI_State.SuspectingPlayer:
                if (canCurrentlySeePlayer)
                {
                    currentSuspicionLevel += Time.deltaTime;
                    if (detectionSlider != null) detectionSlider.value = currentSuspicionLevel;
                    if (currentSuspicionLevel >= suspicionTimeToAlert)
                    {
                        ChangeState(EnemyAI_State.ChasingPlayer);
                    }
                }
                else // Lost sight during suspicion phase
                {
                    currentSuspicionLevel -= Time.deltaTime * 2f; // Decrease faster if sight lost
                    if (detectionSlider != null) detectionSlider.value = currentSuspicionLevel;
                    if (currentSuspicionLevel <= 0)
                    {
                        ChangeState(EnemyAI_State.Patrolling);
                    }
                }
                break;

            case EnemyAI_State.ChasingPlayer:
                if (playerTransform == null) { ChangeState(EnemyAI_State.ReturningToStartPoint); break; }

                if (canCurrentlySeePlayer)
                {
                    lastSightingPosition = playerTransform.position;
                    // No timer needed while actively seeing and chasing
                }
                else // Lost direct line of sight while chasing
                {
                    ChangeState(EnemyAI_State.SearchingLastKnownPosition);
                }
                
                if (Vector2.Distance(transform.position, playerTransform.position) <= catchDistance)
                { CaughtPlayer(); return; }
                break;
            
            case EnemyAI_State.SearchingLastKnownPosition:
                if (canCurrentlySeePlayer) { ChangeState(EnemyAI_State.ChasingPlayer); break; } // Re-spotted

                currentSearchTimer += Time.deltaTime;
                if (detectionSlider != null) detectionSlider.value = Mathf.Max(0, searchTimeBeforePatrol - currentSearchTimer);


                if (currentSearchTimer >= searchTimeBeforePatrol || 
                    Vector2.Distance(transform.position, lastSightingPosition) <= patrolScript.pointReachedThreshold)
                {
                    ChangeState(EnemyAI_State.ReturningToStartPoint);
                }
                break;

            case EnemyAI_State.ReturningToStartPoint:
                 if (canCurrentlySeePlayer) { ChangeState(EnemyAI_State.ChasingPlayer); break; }

                 float threshold = (patrolScript != null) ? patrolScript.pointReachedThreshold : mover.stopThreshold;
                 if(Vector2.Distance(transform.position, initialPosition) <= threshold) 
                 {
                     ChangeState(EnemyAI_State.Patrolling);
                 }
                 break;
        }
        
        ExecuteCurrentStateAction();
        UpdateAnimator(); 
    }

    void ChangeState(EnemyAI_State newState)
    {
        if (currentState == newState && Application.isPlaying) return; 

        Debug.Log($"[{gameObject.name}/EnemyAI] State Change: {currentState} -> {newState}", this);
        
        // Exit logic for old state (handling UI)
        if (questionMarkIcon != null) questionMarkIcon.SetActive(false);
        if (exclamationMarkIcon != null) exclamationMarkIcon.SetActive(false);
        if (detectionSlider != null) detectionSlider.gameObject.SetActive(false);

        currentState = newState;
        if(mover!=null) mover.StopMovement(); 

        switch (currentState)
        {
            case EnemyAI_State.Patrolling:
                if (patrolScript != null) patrolScript.enabled = true;
                currentSuspicionLevel = 0f;
                currentSearchTimer = 0f;
                break;

            case EnemyAI_State.SuspectingPlayer:
                if (patrolScript != null) patrolScript.enabled = false; // Stop patrolling
                mover.StopMovement(); // Enemy might stop or look around during suspicion
                if (questionMarkIcon != null) questionMarkIcon.SetActive(true);
                if (detectionSlider != null) {
                    detectionSlider.gameObject.SetActive(true);
                    detectionSlider.maxValue = suspicionTimeToAlert;
                    detectionSlider.value = currentSuspicionLevel; // Should be 0 when first entering usually
                }
                currentSearchTimer = 0f;
                break;

            case EnemyAI_State.ChasingPlayer:
                if (patrolScript != null) patrolScript.enabled = false;
                if (exclamationMarkIcon != null) exclamationMarkIcon.SetActive(true);
                currentSuspicionLevel = 0f; // Reset suspicion
                currentSearchTimer = 0f;
                if (playerTransform != null) {
                    lastSightingPosition = playerTransform.position; // Ensure we have a target
                } else {
                    ChangeState(EnemyAI_State.ReturningToStartPoint); // No player, can't chase
                }
                break;

            case EnemyAI_State.SearchingLastKnownPosition:
                if (patrolScript != null) patrolScript.enabled = false;
                if (questionMarkIcon != null) questionMarkIcon.SetActive(true);
                if (detectionSlider != null) {
                    detectionSlider.gameObject.SetActive(true);
                    detectionSlider.maxValue = searchTimeBeforePatrol;
                    detectionSlider.value = searchTimeBeforePatrol; // Start full, counts down
                }
                currentSearchTimer = 0f; // Reset timer for this state
                break;

            case EnemyAI_State.ReturningToStartPoint:
                if (patrolScript != null) patrolScript.enabled = false;
                currentSuspicionLevel = 0f;
                currentSearchTimer = 0f;
                break;
        }
    }
    
    void ExecuteCurrentStateAction()
    {
        if (mover == null) return;
        switch (currentState)
        {
            case EnemyAI_State.Patrolling:
                // Movement handled by EnemyPatrol_Simple.cs when enabled
                break;
            case EnemyAI_State.SuspectingPlayer:
                // Enemy is stopped (by ChangeState), player might look around (animation)
                // Or slowly turn towards player's sound/last known hint
                break;
            case EnemyAI_State.ChasingPlayer:
            case EnemyAI_State.SearchingLastKnownPosition: // Also moves to lastSightingPosition
                if (playerTransform != null || currentState == EnemyAI_State.SearchingLastKnownPosition) // For searching, playerTransform might be null if they truly escaped
                {
                    mover.MoveTowardsPoint(lastSightingPosition, chaseSpeed);
                }
                break;
            case EnemyAI_State.ReturningToStartPoint:
                float speedToReturn = (patrolScript != null) ? patrolScript.patrolSpeed : chaseSpeed * 0.75f;
                mover.MoveTowardsPoint(initialPosition, speedToReturn);
                break;
        }
    }
    
    void UpdateAnimator() 
    {
        if (animator == null || mover == null) return;
        Vector2 currentActualVelocity = mover.CurrentVelocity; 
        float speedFactor = currentActualVelocity.sqrMagnitude > 0.01f ? 1f : 0f;
        
        animator.SetFloat("Speed", speedFactor); 

        if (speedFactor > 0f)
        {
            Vector2 moveDir = currentActualVelocity.normalized;
            animator.SetFloat("MoveX", moveDir.x);
            animator.SetFloat("MoveY", moveDir.y);
            animator.SetFloat("LastMoveX", moveDir.x);
            animator.SetFloat("LastMoveY", moveDir.y);
        }
    }

    void CaughtPlayer()
    {
        Debug.LogWarning($"[{gameObject.name}/EnemyAI] PLAYER CAUGHT! Restarting level.", this);
        if(mover != null) mover.StopMovement();
        this.enabled = false; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Gizmos ... (can remain the same)
}