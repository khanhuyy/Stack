using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    
    public static Action GameStart;
    
    public static Action GameOver;

    public static Action<int> UpdateBestScore;
}
