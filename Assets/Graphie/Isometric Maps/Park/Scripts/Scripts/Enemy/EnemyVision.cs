// EnemyVision.cs
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyVision : MonoBehaviour
{
    [Header("Vision Settings")]
    [Tooltip("Layer mask for obstacles that block line of sight. Should NOT include Player or this Enemy's layer.")]
    public LayerMask obstacleLayer;
    [Tooltip("Tag of the player GameObject.")]
    public string playerTag = "Player";
    [Tooltip("Public for AI Brain to reference if needed for Gizmos, actual vision range is PolygonCollider2D shape.")]
    public float viewRadius = 8f; // Still useful for AI_Stealth Gizmo
    [Range(0, 360)]
    [Tooltip("Public for AI Brain to reference if needed for Gizmos, actual vision angle is PolygonCollider2D shape.")]
    public float viewAngle = 90f; // Still useful for AI_Stealth Gizmo

    public bool CanSeePlayer { get; private set; } = false;
    public Transform LastSightingOfPlayer { get; private set; }

    private Transform _playerCurrentlyInTriggerZone;
    private PolygonCollider2D _visionConeCollider;
    private EnemyMovement_Simple _parentMover;

    void Awake()
    {
        _visionConeCollider = GetComponent<PolygonCollider2D>();
        if (_visionConeCollider == null || !_visionConeCollider.isTrigger)
        {
            Debug.LogError($"[{gameObject.name}/EnemyVision] Requires a PolygonCollider2D set to 'Is Trigger'. Vision disabled.", this);
            enabled = false; return;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) { rb.isKinematic = true; rb.gravityScale = 0; }
        else { Debug.LogError($"[{gameObject.name}/EnemyVision] Rigidbody2D component not found!", this); }

        if (transform.parent != null) _parentMover = transform.parent.GetComponent<EnemyMovement_Simple>();
        if (_parentMover == null) Debug.LogError($"[{gameObject.name}/EnemyVision] EnemyMovement_Simple not found on parent '{(transform.parent ? transform.parent.name : "null parent")}'. Cone orientation will be static.", this);
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj == null)
        {
            Debug.LogError($"[{gameObject.name}/EnemyVision] Player not found by tag '{playerTag}'. Vision disabled.", this);
            enabled = false; return;
        }
        // _playerCurrentlyInTriggerZone set by OnTriggerEnter2D
    }

    void LateUpdate()
    {
        OrientVisionCone();
    }

    void OrientVisionCone()
    {
        if (_parentMover != null)
        {
            Vector2 facingDir = _parentMover.FacingDirection;
            if (_parentMover.CurrentVelocity.sqrMagnitude > 0.01f)
            {
                facingDir = _parentMover.CurrentVelocity.normalized;
            }
            if (facingDir != Vector2.zero)
            {
                float angle = Mathf.Atan2(facingDir.y, facingDir.x) * Mathf.Rad2Deg - 90f;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) _playerCurrentlyInTriggerZone = other.transform;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.transform == _playerCurrentlyInTriggerZone) CheckLineOfSightToPlayerInZone(_playerCurrentlyInTriggerZone);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform == _playerCurrentlyInTriggerZone)
        {
            _playerCurrentlyInTriggerZone = null;
            SetPlayerVisibility(false, null);
        }
    }

    void CheckLineOfSightToPlayerInZone(Transform playerTarget)
    {
        if (playerTarget == null) { SetPlayerVisibility(false, null); return; }

        Vector3 linecastStart = transform.position;
        RaycastHit2D hit = Physics2D.Linecast(linecastStart, playerTarget.position, obstacleLayer);
        Debug.DrawLine(linecastStart, playerTarget.position, hit.collider && hit.transform != playerTarget ? Color.magenta : Color.green, 0.02f);

        if (hit.collider == null || hit.transform == playerTarget) SetPlayerVisibility(true, playerTarget);
        else SetPlayerVisibility(false, null);
    }

    void SetPlayerVisibility(bool isVisible, Transform playerSpottedTransform)
    {
        bool previousCanSeePlayer = CanSeePlayer;
        CanSeePlayer = isVisible;
        LastSightingOfPlayer = isVisible ? playerSpottedTransform : null;

        if (CanSeePlayer && !previousCanSeePlayer) Debug.LogWarning($"[{transform.parent?.name}/EnemyVision] !!!!! PLAYER SPOTTED !!!!! at {LastSightingOfPlayer.position}", this);
        else if (!CanSeePlayer && previousCanSeePlayer && _playerCurrentlyInTriggerZone != null) Debug.LogWarning($"[{transform.parent?.name}/EnemyVision] !!!!! PLAYER SIGHT LOST (still in trigger, but LOS now blocked) !!!!!", this);
    }

    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && CanSeePlayer && LastSightingOfPlayer != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, LastSightingOfPlayer.position);
        }
    }
}