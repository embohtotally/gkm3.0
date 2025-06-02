// EnemyPatrol_Simple.cs
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(EnemyMovement_Simple))]
public class EnemyPatrol_Simple : MonoBehaviour
{
    [Header("Patrol Behavior")]
    [Tooltip("Assign Transforms for patrol points directly.")]
    [SerializeField] private List<Transform> patrolPoints;
    [Tooltip("Speed of the enemy while actively patrolling.")]
    public float patrolSpeed = 2f;
    [Tooltip("How long the enemy waits at each patrol point.")]
    [SerializeField] private float waitAtPointDuration = 1f;
    [Tooltip("If true, patrol path will loop. If false, it will ping-pong.")]
    [SerializeField] private bool loopPatrol = true;
    [Tooltip("How close the enemy needs to be to a point to consider it 'reached'.")]
    public float pointReachedThreshold = 0.2f;

    private EnemyMovement_Simple mover;
    private int currentPatrolPointIndex = 0;
    private bool isWaiting = false;
    private int patrolDirection = 1;

    void Awake()
    {
        mover = GetComponent<EnemyMovement_Simple>();
        if (mover == null)
        {
            Debug.LogError($"[{gameObject.name}/EnemyPatrol] EnemyMovement_Simple component not found!", this);
            enabled = false;
        }
    }

    void OnEnable()
    {
        if (mover == null || !this.enabled) return;

        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            Debug.LogWarning($"[{gameObject.name}/EnemyPatrol] OnEnable: No patrol points. Patrolling will not activate.", this);
            mover.StopMovement();
            return;
        }
        currentPatrolPointIndex = 0;
        isWaiting = false;
        patrolDirection = 1;
        StopAllCoroutines();
        MoveToCurrentPoint();
    }

    void OnDisable()
    {
        StopAllCoroutines();
        isWaiting = false;
        if (mover != null && Application.isPlaying)
        {
            mover.StopMovement();
        }
    }

    void Update()
    {
        if (isWaiting || patrolPoints == null || patrolPoints.Count == 0 || mover == null || !this.enabled)
        {
            return;
        }

        Transform targetPoint = GetCurrentTargetPoint();
        if (targetPoint == null)
        {
            mover.StopMovement();
            Debug.LogWarning($"[{gameObject.name}/EnemyPatrol] Current target patrol point is null. Stopping patrol.", this);
            return;
        }

        if (Vector2.Distance(transform.position, targetPoint.position) <= pointReachedThreshold)
        {
            if (!isWaiting) StartCoroutine(WaitAndAdvance());
        }
        else
        {
            mover.MoveTowardsPoint(targetPoint.position, patrolSpeed);
        }
    }

    Transform GetCurrentTargetPoint()
    {
        if (patrolPoints.Count == 0 || currentPatrolPointIndex < 0 || currentPatrolPointIndex >= patrolPoints.Count)
        {
            return null;
        }
        return patrolPoints[currentPatrolPointIndex];
    }

    IEnumerator WaitAndAdvance()
    {
        isWaiting = true;
        mover.StopMovement();
        if (waitAtPointDuration > 0)
        {
            yield return new WaitForSeconds(waitAtPointDuration);
        }
        AdvanceToNextPoint();
        isWaiting = false;
        MoveToCurrentPoint();
    }

    void AdvanceToNextPoint()
    {
        if (patrolPoints.Count == 0) return;
        if (patrolPoints.Count == 1) { currentPatrolPointIndex = 0; return; }

        if (loopPatrol)
        {
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Count;
        }
        else
        {
            currentPatrolPointIndex += patrolDirection;
            if (currentPatrolPointIndex >= patrolPoints.Count)
            {
                currentPatrolPointIndex = patrolPoints.Count - 2;
                patrolDirection = -1;
            }
            else if (currentPatrolPointIndex < 0)
            {
                currentPatrolPointIndex = 1;
                patrolDirection = 1;
            }
        }
    }

    void MoveToCurrentPoint()
    {
        Transform targetPoint = GetCurrentTargetPoint();
        if (targetPoint != null && mover != null)
        {
            mover.MoveTowardsPoint(targetPoint.position, patrolSpeed);
        }
    }
}