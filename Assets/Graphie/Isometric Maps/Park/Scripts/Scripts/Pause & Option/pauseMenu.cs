using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class pauseMenu : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 1;
    }
    public void pauseGame()
    {
        Time.timeScale = 0;
    }
    public void Resume()
    {
        Time.timeScale = 1;
    }
    public void option()
    {
        Time.timeScale = 0;
    }
    public void mainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }
}
