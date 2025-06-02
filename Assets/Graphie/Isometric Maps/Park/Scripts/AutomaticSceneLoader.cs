// AutomaticSceneLoader.cs
using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes

public class AutomaticSceneLoader : MonoBehaviour
{
    [Header("Scene Transition Settings")]
    [Tooltip("Enter the exact name of the scene you want to load.")]
    [SerializeField] private string sceneNameToLoad;

    [Tooltip("The tag of the GameObject that should trigger the scene load (usually 'Player').")]
    [SerializeField] private string triggeringTag = "Player";

    [Tooltip("Should this trigger only once? Prevents multiple load attempts if player re-enters quickly.")]
    [SerializeField] private bool triggerOnce = true;

    private bool hasBeenTriggered = false;

    void Awake()
    {
        // Ensure the GameObject this script is on has a Collider component set to be a trigger.
        Collider2D col2D = GetComponent<Collider2D>();

        if (col2D != null)
        {
            if (!col2D.isTrigger)
            {
                Debug.LogWarning($"Collider2D on {gameObject.name} is not set to 'Is Trigger'. AutomaticSceneLoader might not work as expected. Please check 'Is Trigger'.");
            }
        }
        else
        {
            Debug.LogError($"No Collider or Collider2D found on {gameObject.name}. AutomaticSceneLoader requires a trigger collider to function.");
            this.enabled = false; // Disable script if no collider
        }

        if (string.IsNullOrEmpty(sceneNameToLoad))
        {
            Debug.LogError($"SceneNameToLoad is not set on {gameObject.name}. Transition will not work.");
            this.enabled = false;
        }
    }

    // For 2D Physics
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object entering the trigger has the specified tag (e.g., "Player")
        if (other.gameObject.CompareTag(triggeringTag))
        {
            HandleSceneTransition();
        }
    }


    private void HandleSceneTransition()
    {
        if (triggerOnce && hasBeenTriggered)
        {
            return; // Already triggered, do nothing
        }

        if (string.IsNullOrEmpty(sceneNameToLoad))
        {
            Debug.LogError($"SceneNameToLoad is not set on {gameObject.name} at the moment of triggering!");
            return;
        }

        Debug.Log($"Player entered trigger for {gameObject.name}. Loading scene: {sceneNameToLoad}");
        hasBeenTriggered = true;

        // Optional: Implement a fade out effect here before loading the scene for a smoother transition

        SceneManager.LoadScene(sceneNameToLoad);
    }
}
