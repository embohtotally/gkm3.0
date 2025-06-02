// CollectibleItem.cs
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Header("Collection Effects")]
    [Tooltip("Optional: Visual effect to spawn when this item is collected.")]
    public GameObject collectionEffectPrefab;

    [Header("Action On Collect")]
    [Tooltip("Drag the GameObject from your Hierarchy here that should be turned OFF when this item is collected.")]
    public GameObject objectToDeactivateOnCollect; // The specific GameObject to turn off

    private bool playerIsInRange = false; // Tracks if the player is in the trigger zone

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerIsInRange = true;
            Debug.Log($"[CollectibleItem] Player '{other.name}' ENTERED range of '{gameObject.name}'. Can press 'F'.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerIsInRange = false;
            Debug.Log($"[CollectibleItem] Player '{other.name}' EXITED range of '{gameObject.name}'.");
        }
    }

    void Update()
    {
        if (playerIsInRange && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log($"[CollectibleItem] '{gameObject.name}': 'F' key pressed. Calling Collect().");
            Collect();
        }
    }

    void Collect()
    {
        Debug.Log($"[CollectibleItem] ----- Collect() method entered for '{gameObject.name}' -----");

        // 1. Perform the main action: Deactivate the assigned GameObject
        if (objectToDeactivateOnCollect != null)
        {
            Debug.Log($"[CollectibleItem] '{gameObject.name}': Attempting to deactivate '{objectToDeactivateOnCollect.name}'.");
            objectToDeactivateOnCollect.SetActive(false);
            Debug.Log($"[CollectibleItem] '{gameObject.name}': Successfully DEACTIVATED '{objectToDeactivateOnCollect.name}'.");
        }
        else
        {
            Debug.LogWarning($"[CollectibleItem] '{gameObject.name}': No 'Object To Deactivate On Collect' assigned in the Inspector. No object was turned off by this item.");
        }

        // 2. Notify the CollectionManager that this collectible has been "processed"
        // This allows the CollectionManager to count it towards opening the main door, changing visibility, etc.
        Debug.Log($"[CollectibleItem] '{gameObject.name}': Notifying CollectionManager...");
        if (CollectionManager.Instance != null)
        {
            CollectionManager.Instance.ItemWasCollected(this.gameObject);
            Debug.Log($"[CollectibleItem] '{gameObject.name}': Successfully notified CollectionManager.");
        }
        else
        {
            Debug.LogError($"[CollectibleItem] '{gameObject.name}': CollectionManager.Instance is NULL! Cannot report collection.");
        }

        // 3. Instantiate a visual effect (optional)
        if (collectionEffectPrefab != null)
        {
            Debug.Log($"[CollectibleItem] '{gameObject.name}': Instantiating collection effect.");
            Instantiate(collectionEffectPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.Log($"[CollectibleItem] '{gameObject.name}': No collection effect prefab assigned.");
        }

        // 4. Make THIS collectible item itself disappear
        // If you want the item you pressed 'F' on to also disappear, keep this line.
        // If the item you pressed 'F' on should REMAIN (e.g., it's a lever that stays pulled), comment out or remove this line.
        Debug.Log($"[CollectibleItem] '{gameObject.name}': Preparing to DESTROY this collectible item itself.");
        Destroy(gameObject);
    }
}