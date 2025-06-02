using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ComicPanelViewer : MonoBehaviour
{
    public Image comicDisplay;            // Placeholder Image component for displaying panels
    public Sprite[] comicPanelNames;      // Array to hold comic panel Sprites directly
    public GameObject button1;            // First button (to load one scene)
    public GameObject button2;            // Second button (to load another scene)
    public string scene1Name;             // Name of the first scene to load
    public string scene2Name;             // Name of the second scene to load

    private int currentPanelIndex = 0;    // Tracks the current panel being displayed

    void Start()
    {
        // Ensure buttons are hidden at the start
        button1.SetActive(false);
        button2.SetActive(false);

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

            // Check if this is the last panel
            if (panelIndex == comicPanelNames.Length - 1)
            {
                ShowButtons();  // Display buttons on the last panel
            }
            else
            {
                HideButtons();  // Hide buttons for all other panels
            }
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

    // Show buttons for scene navigation
    void ShowButtons()
    {
        button1.SetActive(true);
        button2.SetActive(true);
    }

    // Hide buttons
    void HideButtons()
    {
        button1.SetActive(false);
        button2.SetActive(false);
    }

    // Button1 action: Load the first scene
    public void LoadScene1()
    {
        SceneManager.LoadScene(scene1Name);
    }

    // Button2 action: Load the second scene
    public void LoadScene2()
    {
        SceneManager.LoadScene(scene2Name);
    }
}
