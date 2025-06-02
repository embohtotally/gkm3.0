using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ValueHandler : MonoBehaviour
{
    [Header("Main Settings")]
    public float DataFloat;
    public TMP_Text TextFloat;

    [Header("Condition")]
    public float ConditionFloat;
    
    public UnityEvent ConditionEvent;

    private bool isExecuted = false;
    
    public void AddValue(float value)
    {
        DataFloat += value;
        UpdateDisplay();
        CheckCondition();
    }
    
    private void UpdateDisplay()
    {
        if (TextFloat != null)
        {
            TextFloat.text = DataFloat.ToString();
        }
        else
        {
            Debug.LogWarning("TextFloat is not assigned.");
        }
    }
    
    private void CheckCondition()
    {
        if (!isExecuted && Mathf.Approximately(DataFloat, ConditionFloat))
        {
            isExecuted = true;
            ConditionEvent?.Invoke();
        }
    }

    private void Awake()
    {
        UpdateDisplay();
    }

    private void Update()
    {
        UpdateDisplay();
    }
}
