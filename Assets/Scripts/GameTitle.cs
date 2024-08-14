using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTitle : MonoBehaviour
{
    public GameObject[] idleTexts;
    
    private void OnEnable()
    {
        GameEvents.GameStart += GameEvents_GameStart;
    }

    private void OnDisable()
    {
        GameEvents.GameStart -= GameEvents_GameStart;
    }

    private void Start()
    {
        // GameEvents.GameStart += GameEvents_GameStart;
        foreach (var text in idleTexts)
        {
            text.SetActive(true);
        }
    }

    private void GameEvents_GameStart()
    {
        foreach (var text in idleTexts)
        {
            text.SetActive(false);
        }
    }
}
