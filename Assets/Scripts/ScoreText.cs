using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private int _score = -1;
    
    private string highScoreKey = "highScore";
    private HighScoreData highScore = new HighScoreData();
    
    private void Awake()
    {
        if (BinaryFileStream.Exist(highScoreKey))
        {
            StartCoroutine(ReadDataFile());
        }
    }
    
    private IEnumerator ReadDataFile()
    {
        highScore = BinaryFileStream.Read<HighScoreData>(highScoreKey);
        yield return new WaitForEndOfFrame();
    }
    
    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        GameManager.OnCubeSpawned += GameManager_OnCubeSpawned;
    }

    private void OnDestroy()
    {
        GameManager.OnCubeSpawned -= GameManager_OnCubeSpawned;
    }
    
    private void OnEnable()
    {
        GameEvents.GameOver += GameEvents_GameOver;
        GameEvents.UpdateBestScore += GameEvents_UpdateBestScore;
    }

    private void OnDisable()
    {
        GameEvents.GameOver -= GameEvents_GameOver;
        GameEvents.UpdateBestScore -= GameEvents_UpdateBestScore;
    }

    private void GameEvents_GameOver()
    {
        GameEvents.UpdateBestScore(_score);
    }

    private void GameManager_OnCubeSpawned()
    {
        _score++;
        _text.text = "" + _score;
    }
    private void GameEvents_UpdateBestScore(int newBestScore)
    {
        if (newBestScore > highScore?.score)
        {
            highScore.score = newBestScore;
            BinaryFileStream.Save<HighScoreData>(highScore, highScoreKey);
        }
    }
}
