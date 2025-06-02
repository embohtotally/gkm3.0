using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ComicPanelViewer1 : MonoBehaviour
{
    public Image comicDisplay;            // Placeholder Image component for displaying panels
    public Sprite[] comicPanelNames;      // Array to hold comic panel Sprites directly
    public string nextSceneName;          // Name of the scene to load after the last panel

    private int currentPanelIndex = 0;    // Tracks the current panel being displayed

    void Start()
    {
        // Load the first panel if available
        if (comicPanelNames.Length > 0)
        {
            LoadPanel(currentPanelIndex);
        }
    }

    void Update()
    {
        // Detect mouse clicks to navigate panels
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 clickPosition = Input.mousePosition;

            // Check where the screen was clicked
            if (clickPosition.x < Screen.width / 2)
            {
                PreviousPanel();  // Left side click
            }
            else
            {
                NextPanel();      // Right side click
            }
        }
    }

    // Load and display the panel at the current index
    void LoadPanel(int panelIndex)
    {
        if (panelIndex >= 0 && panelIndex < comicPanelNames.Length)
        {
            comicDisplay.sprite = comicPanelNames[panelIndex];  // Set the display to the current panel
        }
        else
        {
            Debug.LogError("Panel index out of range: " + panelIndex);
        }
    }

    // Navigate to the next panel
    void NextPanel()
    {
        if (currentPanelIndex < comicPanelNames.Length - 1)
        {
            currentPanelIndex++;
            LoadPanel(currentPanelIndex);
        }
        else
        {
            // Transition to the next scene after the last panel
            SceneManager.LoadScene(nextSceneName);
        }
    }

    // Navigate to the previous panel
    void PreviousPanel()
    {
        if (currentPanelIndex > 0)
        {
            currentPanelIndex--;
            LoadPanel(currentPanelIndex);
        }
    }
}
