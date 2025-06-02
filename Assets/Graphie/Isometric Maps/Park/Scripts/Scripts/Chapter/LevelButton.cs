using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

[System.Serializable]
public class LevelButton
{
    public string chapterName;
    public GameObject chapterSelector;
    // public Color enabledColor;
    // public Color disabledColor;

    public void UpdateButtonStatus()
    {
        GameChapter gameChapter = GameObject.FindObjectOfType<GameChapter>();
        Button button = chapterSelector.GetComponent<Button>();
        bool isComplete = gameChapter.IsChapterComplete(chapterName);

        if(chapterSelector != null) {
            Transform activeChild = chapterSelector.transform.Find("Active");
            Transform inactiveChild = chapterSelector.transform.Find("Inactive");

            button.interactable = isComplete;

            if (isComplete)
            {
                if (activeChild != null) {
                    activeChild.gameObject.SetActive(true);
                }
                if (inactiveChild != null) {
                    inactiveChild.gameObject.SetActive(false);
                }
            }
            else
            {
                if (activeChild != null) {
                    activeChild.gameObject.SetActive(false);
                }
                if (inactiveChild != null) {
                    inactiveChild.gameObject.SetActive(true);
                }
            }
        }
    }
}
