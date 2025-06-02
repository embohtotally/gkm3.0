using UnityEngine.SceneManagement;
using UnityEngine;

public class Scenema : MonoBehaviour
{
    public GameObject _pauseMenu;
    public void playbutton()
    {
        SceneManager.LoadScene("SampleSceneChapter");
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void PauseGame()
    {
        _pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }
    public void ResumeGame()
    {
        _pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }
    public void NextLeveL()
    {
        SceneManager.LoadScene("SampleSceneChapter");
        Time.timeScale = 1;
    }
}
