// SceneTransitionObject.cs
using UnityEngine;
using UnityEngine.SceneManagement; // Required for changing scenes

public class SceneTransitionObject : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Enter the exact name of the scene to load.")]
    [SerializeField] private string sceneNameToLoad;
    // Alternatively, you could use a scene build index:
    // [SerializeField] private int sceneBuildIndexToLoad = -1; 

    [Header("Interaction Settings")]
    [Tooltip("Optional: A UI element to show when the player is in range (e.g., 'Press F to Enter').")]
    [SerializeField] private GameObject interactPromptUI;

    private bool playerIsInRange = false;

    void Awake()
    {
        // Ensure the interact prompt is hidden at the start
        if (interactPromptUI != null)
        {
            interactPromptUI.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) // Use OnTriggerEnter for 3D
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerIsInRange = true;
            Debug.Log($"[SceneTransition] Player '{other.name}' ENTERED range of '{gameObject.name}'. Can press 'F'.");
            if (interactPromptUI != null)
            {
                interactPromptUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) // Use OnTriggerExit for 3D
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerIsInRange = false;
            Debug.Log($"[SceneTransition] Player '{other.name}' EXITED range of '{gameObject.name}'.");
            if (interactPromptUI != null)
            {
                interactPromptUI.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (playerIsInRange && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log($"[SceneTransition] '{gameObject.name}': 'F' key pressed. Attempting to load scene: {sceneNameToLoad}");
            LoadTargetScene();
        }
    }

    void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(sceneNameToLoad))
        {
            // Optional: Add a fade-out effect here before loading
            SceneManager.LoadScene(sceneNameToLoad);
        }
        // else if (sceneBuildIndexToLoad >= 0)
        // {
        //     SceneManager.LoadScene(sceneBuildIndexToLoad);
        // }
        else
        {
            Debug.LogError($"[SceneTransition] Scene name or build index to load is not set on '{gameObject.name}'!");
        }
    }
}