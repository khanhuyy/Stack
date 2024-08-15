using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class HighScoreData
{
    public int score = 0;
}

public class BestScoreText : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private const string HighScoreKey = "highScore";
    private HighScoreData _highScore = new HighScoreData();
    
    private void Awake()
    {
        if (BinaryFileStream.Exist(HighScoreKey))
        {
            StartCoroutine(ReadDataFile());
        }
    }
    
    private IEnumerator ReadDataFile()
    {
        _highScore = BinaryFileStream.Read<HighScoreData>(HighScoreKey);
        yield return new WaitForEndOfFrame();
    }
    
    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _text.text = "BEST SCORE: " + _highScore?.score;
    }
}
