using System.IO;
using UnityEngine;

public static class SettingsManager
{
    private static string settingsFilePath = Application.persistentDataPath + "/settings.json";

    public static void SaveSettings(SettingsData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(settingsFilePath, json);
    }

    public static SettingsData LoadSettings()
    {
        if (File.Exists(settingsFilePath))
        {
            string json = File.ReadAllText(settingsFilePath);
            return JsonUtility.FromJson<SettingsData>(json);
        }
        else
        {
            // Default settings if no file exists
            return new SettingsData
            {
                musicVolume = 0.5f,
                sfxVolume = 0.5f,
                brightness = 1f
            };
        }
    }
}
