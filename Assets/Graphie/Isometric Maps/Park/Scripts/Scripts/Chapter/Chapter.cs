using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Chapter
{
    public string chapter;
    public bool isComplete;

    public Chapter(string chapterName, bool completionStatus)
    {
        chapter = chapterName;
        isComplete = completionStatus;
    }
}

[System.Serializable]
public class ChapterCompleteData
{
    public List<Chapter> ChapterComplete = new List<Chapter>();
}