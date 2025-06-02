using UnityEngine;

public class EnemyPathfindingMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3.5f;
    // rotationSpeed field is no longer needed if we remove rotation
    // [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float stoppingDistance = 0.5f;

    private Vector3 currentTargetPosition;
    private bool isMoving = false;
    private bool targetReached = true;

    void Update()
    {
        if (isMoving)
        {
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        // Calculate direction towards the target.
        // Note: For a 2D game in the XY plane, you might want to ensure Z values are consistent.
        Vector3 directionToTarget3D = (currentTargetPosition - transform.position);
        // For 2D movement in XY plane, ensure Z difference doesn't affect distance or direction if not intended
        // directionToTarget3D.z = 0; // If your game is strictly 2D in XY plane and Z should be ignored for movement logic

        float distanceToTarget = directionToTarget3D.magnitude;

        if (distanceToTarget > stoppingDistance)
        {
            targetReached = false;
            Vector3 movementDirection = directionToTarget3D.normalized;
            transform.position += movementDirection * moveSpeed * Time.deltaTime;

            // --- ROTATION LOGIC REMOVED ---
            // The following block, which handled rotation, has been removed:
            /*
            if (movementDirection != Vector3.zero) // Or check based on 2D direction if applicable
            {
                // For 2D (XY plane, rotating around Z-axis):
                // float angle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
                // Quaternion targetRotation2D = Quaternion.AngleAxis(angle, Vector3.forward);
                // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation2D, rotationSpeed * Time.deltaTime);

                // For 3D (XZ plane, rotating around Y-axis):
                // Quaternion targetRotation = Quaternion.LookRotation(new Vector3(movementDirection.x, 0, movementDirection.z));
                // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            */
        }
        else
        {
            isMoving = false;
            targetReached = true;
            // Optionally snap to a position if needed
            // transform.position = currentTargetPosition - directionToTarget3D.normalized * stoppingDistance;
        }
    }

    public void MoveTo(Vector3 targetPosition)
    {
        this.currentTargetPosition = targetPosition;
        // If strictly 2D in XY plane and you want to ensure the target Z matches current Z:
        // this.currentTargetPosition.z = transform.position.z;
        this.isMoving = true;
        this.targetReached = false;
    }

    public void Stop()
    {
        this.isMoving = false;
    }

    public bool HasReachedDestination()
    {
        if (isMoving)
        {
            // For 2D, ensure Z is not part of distance calculation if it's irrelevant
            Vector3 positionNoZ = new Vector3(transform.position.x, transform.position.y, currentTargetPosition.z);
            return Vector3.Distance(positionNoZ, currentTargetPosition) <= stoppingDistance;
            // Or simply use the 3D distance if Z consistency is handled elsewhere or intended:
            // return Vector3.Distance(transform.position, currentTargetPosition) <= stoppingDistance;
        }
        return targetReached;
    }

    public void SetSpeed(float speed)
    {
        this.moveSpeed = speed;
    }

    public float GetSpeed()
    {
        return this.moveSpeed;
    }
}