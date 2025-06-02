using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils; // For UtilsClass.GetRandomDir(), if available

[RequireComponent(typeof(EnemyPathfindingMovement))]
public class EnemyAI : MonoBehaviour
{
    // --- State Enum ---
    private enum State
    {
        Roaming,
        ChaseTarget,
        ShootingTarget,
        GoingBackToStart,
    }

    // --- Fields ---

    [Header("State Machine")]
    private State currentState;

    [Header("Pathfinding & Movement")]
    private EnemyPathfindingMovement pathfindingMovement;
    private Vector3 startingPosition; // Initial Z will be preserved for 2D roaming
    private Vector3 roamPosition;

    [Header("Roaming Behaviour")]
    [Tooltip("Maximum distance from the starting point the AI will roam in the XY plane.")]
    [SerializeField] private float roamRadius = 10f; // Adjusted default for typical 2D
    [Tooltip("Minimum distance from the starting point for a new roam position.")]
    [SerializeField] private float minRoamDistanceFromStart = 1f; // Adjusted default
    [Tooltip("Distance at which a destination is considered reached.")]
    [SerializeField] private float reachedPositionDistance = 0.5f; // Adjusted default

    [Header("Targeting")]
    [Tooltip("The tag used to find the player GameObject.")]
    [SerializeField] private string playerTag = "Player";
    [Tooltip("Distance at which the AI will start chasing the target.")]
    [SerializeField] private float chaseRange = 10f;
    [Tooltip("Distance at which the AI will start shooting. Must be <= Chase Range.")]
    [SerializeField] private float shootingRange = 7f;
    private Transform target;

    [Header("Combat")]
    [Tooltip("Time in seconds between shots.")]
    [SerializeField] private float shootCooldown = 1.5f;
    [Tooltip("Speed at which the AI rotates to face its target when shooting (degrees per second).")]
    [SerializeField] private float rotationSpeed = 360f; // For 2D rotation
    private float shootTimer;

    // --- Unity Methods ---

    private void Awake()
    {
        pathfindingMovement = GetComponent<EnemyPathfindingMovement>();
        if (pathfindingMovement == null)
        {
            Debug.LogError("EnemyPathfindingMovement component not found on " + gameObject.name, this);
        }
        startingPosition = transform.position; // Store initial position, including Z for 2D depth
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            target = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player not found with tag: '" + playerTag + "'.", this);
        }

        roamPosition = GetRoamingPosition();
        currentState = State.Roaming;
        shootTimer = shootCooldown;
    }

    private void Update()
    {
        if (target != null && !target.gameObject.activeInHierarchy)
        {
            target = null;
            if (currentState == State.ChaseTarget || currentState == State.ShootingTarget)
            {
                currentState = State.GoingBackToStart;
            }
        }

        switch (currentState)
        {
            case State.Roaming:
                HandleRoamingState();
                break;
            case State.ChaseTarget:
                HandleChaseTargetState();
                break;
            case State.ShootingTarget:
                HandleShootingTargetState();
                break;
            case State.GoingBackToStart:
                HandleGoingBackToStartState();
                break;
        }
    }

    // --- State Handler Methods ---

    private void HandleRoamingState()
    {
        if (pathfindingMovement == null) return;
        pathfindingMovement.MoveTo(roamPosition);

        // Corrected line (was around original line 218)
        bool atDestination = pathfindingMovement.HasReachedDestination() ||
                             Vector3.Distance(transform.position, roamPosition) < reachedPositionDistance;

        if (atDestination)
        {
            roamPosition = GetRoamingPosition();
        }

        if (target != null && Vector3.Distance(transform.position, target.position) < chaseRange)
        {
            currentState = State.ChaseTarget;
        }
    }

    private void HandleChaseTargetState()
    {
        if (pathfindingMovement == null) return;
        if (target == null)
        {
            currentState = State.GoingBackToStart;
            return;
        }

        pathfindingMovement.MoveTo(target.position);
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget < shootingRange)
        {
            currentState = State.ShootingTarget;
            pathfindingMovement.Stop();
        }
        else if (distanceToTarget > chaseRange)
        {
            currentState = State.GoingBackToStart;
        }
    }

    private void HandleShootingTargetState()
    {
        if (pathfindingMovement == null) return;
        if (target == null)
        {
            currentState = State.GoingBackToStart;
            return;
        }

        pathfindingMovement.Stop();

        // 2D Rotation to face target (around Z-axis)
        Vector2 directionToTarget2D = (target.position - transform.position); // No need to normalize for Atan2
        if (directionToTarget2D != Vector2.zero)
        {
            float angle = Mathf.Atan2(directionToTarget2D.y, directionToTarget2D.x) * Mathf.Rad2Deg;
            // Assuming sprite's 'forward' is its right (0 degrees). Adjust if sprite faces up (angle - 90).
            Quaternion targetRotation2D = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation2D, rotationSpeed * Time.deltaTime);
        }

        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            ShootAtTarget();
            shootTimer = shootCooldown;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget > shootingRange)
        {
            currentState = (distanceToTarget < chaseRange) ? State.ChaseTarget : State.GoingBackToStart;
        }
    }

    private void HandleGoingBackToStartState()
    {
        if (pathfindingMovement == null) return;
        pathfindingMovement.MoveTo(startingPosition);

        // Corrected line (was around original line 125)
        bool atStartDestination = pathfindingMovement.HasReachedDestination() ||
                                  Vector3.Distance(transform.position, startingPosition) < reachedPositionDistance;

        if (atStartDestination)
        {
            roamPosition = GetRoamingPosition();
            currentState = State.Roaming;
        }

        if (target != null && Vector3.Distance(transform.position, target.position) < chaseRange)
        {
            currentState = State.ChaseTarget;
        }
    }

    // --- Helper Methods ---

    private Vector3 GetRoamingPosition()
    {
        Vector2 randomDirection2D = Vector2.right; // Initialize with a default value;

        // Attempt to use CodeMonkey.Utils.GetRandomDir()
        // For 2D, we'd ideally want a 2D direction or adapt a 3D one.
        // We'll primarily use a 2D fallback here.
        System.Reflection.MethodInfo getRandomDirMethod = null;
        bool useFallback = true;
        try
        {
            getRandomDirMethod = typeof(UtilsClass).GetMethod("GetRandomDir");
            if (getRandomDirMethod != null && getRandomDirMethod.ReturnType == typeof(Vector3) && getRandomDirMethod.GetParameters().Length == 0)
            {
                Vector3 randomDir3D = (Vector3)getRandomDirMethod.Invoke(null, null);
                randomDirection2D = new Vector2(randomDir3D.x, randomDir3D.y).normalized; // Take XY, normalize
                if (randomDirection2D.sqrMagnitude < 0.001f) randomDirection2D = Vector2.right; // Fallback if zero
                useFallback = false;
            }
        }
        catch (System.Exception ex)
        {
            // Silently ignore if UtilsClass or GetRandomDir is not found or fails, fallback will be used.
            // Debug.LogWarning("Issue with UtilsClass.GetRandomDir: " + ex.Message + ". Using fallback.");
        }

        if (useFallback)
        {
            // Fallback simple random 2D direction
            randomDirection2D = Random.insideUnitCircle.normalized;
            if (randomDirection2D.sqrMagnitude < 0.001f) // Ensure non-zero
            {
                randomDirection2D = new Vector2(Random.Range(0.1f, 1f), Random.Range(0.1f, 1f)).normalized; // Another attempt
                if (randomDirection2D.sqrMagnitude < 0.001f) randomDirection2D = Vector2.right; // Final fallback
            }
        }

        float actualMinRoamDistance = Mathf.Clamp(minRoamDistanceFromStart, 0f, roamRadius);
        if (actualMinRoamDistance > roamRadius) actualMinRoamDistance = roamRadius;

        float randomDistance = Random.Range(actualMinRoamDistance, roamRadius);
        Vector2 roamOffset = randomDirection2D * randomDistance;

        // Return position in XY plane, preserving original Z depth
        return new Vector3(startingPosition.x + roamOffset.x, startingPosition.y + roamOffset.y, startingPosition.z);
    }

    private void ShootAtTarget()
    {
        if (target != null)
        {
            Debug.Log(gameObject.name + " shoots at " + target.name + " (2D Action) at time: " + Time.time, this);
            // Implement your 2D shooting logic here (e.g., instantiate a 2D projectile)
        }
    }
}