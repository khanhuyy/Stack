using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPopup : MonoBehaviour
{
    public GameObject gameOverText;
    private void OnEnable()
    {
        GameEvents.GameOver += GameEvents_GameOver;
    }

    private void OnDisable()
    {
        GameEvents.GameOver -= GameEvents_GameOver;
    }

    private void GameEvents_GameOver()
    {
        gameOverText.SetActive(true);
    }
}
