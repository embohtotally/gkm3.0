using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScroll : MonoBehaviour
{
    public RectTransform creditsText; // Reference to the text RectTransform
    public float scrollSpeed = 50f;   // Speed of the scroll
    public float endOffset = 100f;   // Distance from the top to stop scrolling
    public string firstSceneName = "MainMenu"; // Name of the scene to load after credits

    private float initialPosition;

    void Start()
    {
        // Record the initial position of the text
        initialPosition = creditsText.anchoredPosition.y;
    }

    void Update()
    {
        // Move the text upward
        creditsText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        // Check if the text has scrolled past the end
        if (creditsText.anchoredPosition.y >= initialPosition + creditsText.rect.height + endOffset)
        {
            LoadMainMenu();
        }

        // Check for key press (optional)
        if (Input.GetKeyDown(KeyCode.Escape)) // Replace KeyCode.Escape with your preferred key
        {
            LoadMainMenu();
        }
    }

    // Method to load the main menu (for button)
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(firstSceneName);
    }
}
