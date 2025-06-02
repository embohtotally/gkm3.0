// CollectionManager.cs
using UnityEngine;
using UnityEngine.Rendering.PostProcessing; // For Post Processing Stack v2
// If you were using URP/HDRP, the 'using' and some API calls might differ slightly.
// This script assumes Built-in Render Pipeline with Post Processing Stack v2.
using System.Collections.Generic;

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance { get; private set; }

    [Header("Key Collectibles (for Door)")]
    [Tooltip("Drag all 'Key' GameObjects here that the player needs to collect TO OPEN THE DOOR.")]
    [SerializeField] private List<GameObject> targetKeys = new List<GameObject>();

    [Header("Firefly Collectibles (for Visibility)")]
    [Tooltip("Drag all 'Firefly' GameObjects here. These will affect vignette visibility.")]
    [SerializeField] private List<GameObject> targetFireflies = new List<GameObject>();

    // --- Internal Tracking ---
    private HashSet<GameObject> remainingKeysToCollect;
    private int totalKeysInitially = 0;
    private int keysCollected = 0;

    private HashSet<GameObject> remainingFirefliesToCollect;
    private int totalFirefliesInitially = 0;
    private int firefliesCollected = 0;

    [Header("Door Settings")]
    public GameObject doorToDisappear;

    [Header("Vignette Settings (Post Processing)")]
    [Tooltip("Assign the Post Process Volume GameObject from your scene that has the Vignette effect.")]
    [SerializeField] private PostProcessVolume postProcessVolume;
    [Tooltip("Intensity of the Vignette when visibility is lowest (e.g., 0.6). Higher means stronger/darker vignette edges.")]
    [SerializeField][Range(0f, 1f)] private float maxVignetteIntensity = 0.6f;
    [Tooltip("Intensity of the Vignette when all FIREFLIES are collected (e.g., 0). Lower means weaker/no vignette.")]
    [SerializeField][Range(0f, 1f)] private float minVignetteIntensity = 0.0f;

    private Vignette vignetteEffect; // Stores the reference to the Vignette settings in the profile

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (doorToDisappear == null)
        {
            Debug.LogError("CollectionManager: Door To Disappear is not assigned on " + gameObject.name + "!");
        }

        // Attempt to get the Vignette effect from the assigned PostProcessVolume's profile
        if (postProcessVolume != null && postProcessVolume.profile != null)
        {
            if (!postProcessVolume.profile.TryGetSettings(out vignetteEffect))
            {
                Debug.LogError("CollectionManager: Vignette effect NOT FOUND in the assigned Post Process Profile! Ensure Vignette is added to the profile and enabled.", postProcessVolume.profile);
                // vignetteEffect will remain null, and UpdateVignetteEffect will do nothing.
            }
            else
            {
                Debug.Log("CollectionManager: Successfully found and linked Vignette effect in Post Process Profile.");
            }
        }
        else if (postProcessVolume == null)
        {
            Debug.LogWarning("CollectionManager: Post Process Volume is not assigned in the Inspector. Vignette visibility effect will not work.");
        }

        InitializeCollectibles();
        UpdateVignetteEffect(); // Set initial vignette intensity
        UpdateCollectionDisplay();
        CheckIfAllKeysCollected();
    }

    void InitializeCollectibles()
    {
        remainingKeysToCollect = new HashSet<GameObject>();
        keysCollected = 0;
        totalKeysInitially = 0;
        ProcessSpecificList(targetKeys, remainingKeysToCollect, ref totalKeysInitially, "Keys");

        remainingFirefliesToCollect = new HashSet<GameObject>();
        firefliesCollected = 0;
        totalFirefliesInitially = 0;
        ProcessSpecificList(targetFireflies, remainingFirefliesToCollect, ref totalFirefliesInitially, "Fireflies");

        Debug.Log($"CollectionManager initialized. Tracking {totalKeysInitially} Keys for the door. Tracking {totalFirefliesInitially} Fireflies for vignette visibility.");
    }

    void ProcessSpecificList(List<GameObject> itemList, HashSet<GameObject> remainingSet, ref int totalCount, string listNameForLogging)
    {
        if (itemList == null) { Debug.LogWarning($"CollectionManager: Target {listNameForLogging} list is null."); return; }
        remainingSet.Clear();
        foreach (GameObject item in itemList)
        {
            if (item != null && item.activeInHierarchy) { if (remainingSet.Add(item)) { totalCount++; } }
            else if (item == null) { Debug.LogWarning($"CollectionManager: Null entry in {listNameForLogging} list ignored."); }
            else { Debug.LogWarning($"CollectionManager: Item '{item.name}' in {listNameForLogging} list is inactive, ignored."); }
        }
    }

    public void ItemWasCollected(GameObject collectedItemGameObject)
    {
        bool itemProcessedThisCall = false;

        if (remainingKeysToCollect.Contains(collectedItemGameObject))
        {
            remainingKeysToCollect.Remove(collectedItemGameObject);
            keysCollected++;
            itemProcessedThisCall = true;
            Debug.Log($"Key collected: {collectedItemGameObject.name}. Keys Progress: {keysCollected}/{totalKeysInitially}.");
            CheckIfAllKeysCollected();
            // Keys do NOT affect vignette in this version
        }
        else if (remainingFirefliesToCollect.Contains(collectedItemGameObject))
        {
            remainingFirefliesToCollect.Remove(collectedItemGameObject);
            firefliesCollected++;
            itemProcessedThisCall = true;
            Debug.Log($"Firefly collected: {collectedItemGameObject.name}. Fireflies Progress: {firefliesCollected}/{totalFirefliesInitially}.");
            UpdateVignetteEffect(); // ONLY update vignette for fireflies
        }

        if (itemProcessedThisCall) { UpdateCollectionDisplay(); }
        else { Debug.LogWarning($"Item '{collectedItemGameObject?.name}' was collected but not found in target Keys or Fireflies lists, or already processed."); }
    }

    void CheckIfAllKeysCollected()
    {
        Debug.Log($"[CM] CheckKeys. Collected: {keysCollected}, Required: {totalKeysInitially}");
        if (totalKeysInitially > 0)
        {
            if (keysCollected >= totalKeysInitially)
            {
                Debug.Log("[CM] SUCCESS: All KEYS collected! Deactivating door.");
                if (doorToDisappear != null)
                {
                    if (doorToDisappear.activeSelf) doorToDisappear.SetActive(false);
                    else Debug.LogWarning($"[CM] Door '{doorToDisappear.name}' already inactive.");
                }
                else Debug.LogError("[CM] doorToDisappear NOT ASSIGNED!");
            }
            else Debug.Log($"[CM] PROGRESS: Not all keys collected. Keys: {keysCollected}/{totalKeysInitially}.");
        }
        else Debug.LogWarning("[CM] No keys required for door. Door state unchanged.");
    }

    void UpdateVignetteEffect()
    {
        if (vignetteEffect == null)
        {
            // Log this only if postProcessVolume was assigned, to avoid spam if intentionally not used.
            if (postProcessVolume != null) Debug.LogWarning("[CM] UpdateVignetteEffect called, but vignetteEffect reference is null. Was it found in the Profile?");
            return;
        }

        float progress = 0f;
        if (totalFirefliesInitially > 0)
        {
            progress = (float)firefliesCollected / totalFirefliesInitially;
        }
        else
        {
            // If no fireflies are part of the system, vignette should be at its minimum intensity (clearest)
            progress = 1f;
        }

        float targetIntensity = Mathf.Lerp(maxVignetteIntensity, minVignetteIntensity, progress);

        // For Post Processing Stack v2, you override the parameter's value
        vignetteEffect.intensity.Override(Mathf.Clamp01(targetIntensity));

        Debug.Log($"[CollectionManager] Updated Vignette Intensity to: {targetIntensity} (Firefly Progress: {progress * 100f}%)");

        // If all fireflies are collected, ensure min intensity is definitely set
        if (firefliesCollected >= totalFirefliesInitially && totalFirefliesInitially > 0)
        {
            vignetteEffect.intensity.Override(Mathf.Clamp01(minVignetteIntensity));
        }
    }

    void UpdateCollectionDisplay() { /* Optional UI updates */ }
}