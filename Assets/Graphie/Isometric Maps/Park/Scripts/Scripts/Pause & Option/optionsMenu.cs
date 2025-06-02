using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class options : MonoBehaviour
{
    [SerializeField] private GameObject _optionMenu;
    // Start is called before the first frame update
    void Start()
    {
    }
    public void closeTab()
    {
        _optionMenu.SetActive(false);
    }
}
