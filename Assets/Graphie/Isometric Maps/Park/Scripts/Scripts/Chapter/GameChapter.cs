using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameChapter : MonoBehaviour
{
    private string filePath;
    public List<LevelButton> levelButtons = new List<LevelButton>();

    private void Start()
    {
        UpdateLevelButtons();
    }

    private void Awake()
    {
        filePath = Path.Combine(Application.dataPath, "ChapterData.json");
        EnsureJsonFileExists();
    }

    private void EnsureJsonFileExists()
    {
        if (!File.Exists(filePath))
        {
            ChapterCompleteData initialData = new ChapterCompleteData();
            initialData.ChapterComplete.Add(new Chapter("Level 1", true));
            
            string json = JsonUtility.ToJson(initialData, true);
            
            File.WriteAllText(filePath, json);
            Debug.Log("JSON file created at: " + filePath);
        }
    }

    public void GenerateJson(string chapterName)
    {
        string json = File.ReadAllText(filePath);
        ChapterCompleteData data = JsonUtility.FromJson<ChapterCompleteData>(json);

        Chapter newChapter = new Chapter(chapterName, true);
        data.ChapterComplete.Add(newChapter);

        json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
        UpdateLevelButtons();

        
        Debug.Log($"Added new chapter '{chapterName}' to JSON file.");
    }
    
    public List<Chapter> GetChapters()
    {
        string json = File.ReadAllText(filePath);
        ChapterCompleteData data = JsonUtility.FromJson<ChapterCompleteData>(json);
        return data.ChapterComplete;
    }

    public bool IsChapterComplete(string chapterName)
    {
        List<Chapter> chapters = GetChapters();
        foreach (Chapter chapter in chapters)
        {
            if (chapter.chapter == chapterName)
            {
                return chapter.isComplete;
            }
        }

        return false;
    }
    
    public void UpdateLevelButtons()
    {
        foreach (LevelButton levelButton in levelButtons)
        {
            levelButton.UpdateButtonStatus();
        }
    }
}