using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class Brightness : MonoBehaviour
{
    public Slider brightnessSlider;

    public PostProcessProfile brightnessProfile;
    public PostProcessLayer layer;

    private AutoExposure exposure;
    private const string BRIGHTNESS_KEY = "BrightnessValue";

    void Start()
    {
        if (brightnessProfile.TryGetSettings(out exposure))
        {
            float savedBrightness = PlayerPrefs.GetFloat(BRIGHTNESS_KEY, 1f);
            brightnessSlider.value = Mathf.Clamp(savedBrightness, 0.05f, 2f);
            AdjustBrightness(brightnessSlider.value);
        }
        else
        {
            Debug.LogError("AutoExposure not found in PostProcessProfile!");
        }
    }

    public void AdjustBrightness(float value)
    {
        if (exposure != null)
        {
            exposure.keyValue.value = Mathf.Clamp(value, 0.05f, 2f);
            PlayerPrefs.SetFloat(BRIGHTNESS_KEY, exposure.keyValue.value);
            PlayerPrefs.Save();
        }
    }
}
