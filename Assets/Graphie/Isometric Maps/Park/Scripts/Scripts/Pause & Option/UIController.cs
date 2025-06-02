using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Slider _musicSlider, _sfxSlider;
    public GameObject defeatPanel;
    public GameObject HUD;
    public GameObject PauseButton;
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    private void Start()
    {
        _musicSlider.value = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.5f);
        _sfxSlider.value = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.5f);

        if (defeatPanel != null)
        {
            defeatPanel.SetActive(false);
        }
    }

    public void ShowDefeatPanel()
    {
        if (defeatPanel != null)
        {
            defeatPanel.SetActive(true); 
            HUD.SetActive(false);
            PauseButton.SetActive(false);
        }
    }

    public void ApplySettings()
    {
        float musicVolume = _musicSlider.value;
        float sfxVolume = _sfxSlider.value;

        AudioManager.instance.MusicVolume(musicVolume);
        AudioManager.instance.SFXVolume(sfxVolume);

        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
        PlayerPrefs.Save();
    }

    public void ToggleMusic()
    {
        AudioManager.instance.ToggleMusic();
    }

    public void ToggleSFX()
    {
        AudioManager.instance.ToggleSFX();
    }

    public void ButtonSFX()
    {
        AudioManager.instance.PlaySFX("Button");
    }
}
