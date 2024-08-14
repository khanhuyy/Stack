using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class MovingCube : MonoBehaviour
{
    public static MovingCube CurrentCube { get; private set; }
    public static MovingCube LastCube { get; private set; }
    private static Color ChainColor { get; set; }
    private static Vector3 LastCubePerfectPos { get; set; }
    
    public MoveDirection MoveDirection { get; internal set; }

    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject perfectQuad;
    private MovingCube rootCube;
    private const float LimitDistance = 1.75f;
    private const float Offset = 0.033f; // Perfect stack
    private float _deltaRed = 0.12f;
    private float _deltaGreen = 0.12f;
    private float _deltaBlue = 0.12f;
    private static int combo;
    private bool _forward = true;

    private void OnEnable()
    {
        if (LastCube == null)
        {
            LastCube = GameObject.Find("Start").GetComponent<MovingCube>();
            LastCubePerfectPos = transform.position + Vector3.up * 0.45f;
        }

        if (ChainColor == Color.black || ChainColor == Color.white)
        {
            ChainColor = Color.cyan;
        }
        else
        {
            var selectColor = Random.Range(0, 3);
            switch (selectColor)
            {
                case 0:
                    if (ChainColor.r + _deltaRed is < 0 or > 1)
                    {
                        _deltaRed *= -1;
                    }
                    ChainColor += new Color(_deltaRed, 0, 0, 0);
                    break;
                case 1:
                    if (ChainColor.r + _deltaGreen is < 0 or > 1)
                    {
                        _deltaGreen *= -1;
                    }
                    ChainColor += new Color(0, _deltaGreen, 0, 0);
                    break;
                case 2:
                    if (ChainColor.r + _deltaBlue is < 0 or > 1)
                    {
                        _deltaBlue *= -1;
                    }
                    ChainColor += new Color(0, 0, _deltaBlue, 0);
                    break;
            }
        }
        rootCube = GameObject.Find("Start").GetComponent<MovingCube>();
        CurrentCube = this;
        GetComponent<Renderer>().material.color = ChainColor;
        transform.localScale = new Vector3(LastCube.transform.localScale.x, transform.localScale.y, LastCube.transform.localScale.z);
    }

    private Color GetRandomColor()
    {
        return new Color(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f));
    }

    internal void Stop()
    {
        audioSource.Play();
        moveSpeed = 0;
        float hangover = GetHangover();
        float maxHangover = MoveDirection == MoveDirection.X
            ? LastCube.transform.localScale.x
            : LastCube.transform.localScale.z;
        
        if (Mathf.Abs(hangover) >= maxHangover)
        {
            GameEvents.GameOver();
        }
        
        float direction = hangover > 0 ? 1f : -1f;

        if (LastCube)
        {
            if (MoveDirection == MoveDirection.Z)
            {
                SplitCubeOnZ(hangover, direction);
            }
            else
            {
                SplitCubeOnX(hangover, direction);
            }
        }
        
        LastCube = this;
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, Camera.main.transform.position + Vector3.up * 0.1f, 1f);
    }

    private float GetHangover()
    {
        if (MoveDirection == MoveDirection.Z)
            return transform.position.z - LastCube.transform.position.z;
        return transform.position.x - LastCube.transform.position.x;
    }

    private void SplitCubeOnX(float hangover, float direction)
    {
        var makeDropCube = true;
        float newXSize = LastCube.transform.localScale.x - Mathf.Abs(hangover);
        float fallingBlockXSize = transform.localScale.x - newXSize;
        if (fallingBlockXSize is <= Offset and >= 0)
        {
            fallingBlockXSize += newXSize;
            newXSize += Mathf.Abs(hangover);
            hangover = 0;
            makeDropCube = false;
            SolvePerfectQuad();
        }
        else
        {
            combo = 0;
            if (LastCube != rootCube)
                LastCubePerfectPos = LastCube.transform.position + new Vector3(hangover / 2, 0.1f, 0);
            else
            {
                LastCubePerfectPos = LastCube.transform.position + new Vector3(0, 0.5f, + hangover / 2);
            }
        }
        float fallingCubeEdge = transform.position.x;
        if (newXSize > 0)
        {
            float newXPosition = LastCube.transform.position.x + hangover / 2;
            transform.localScale = new Vector3(newXSize, transform.localScale.y, transform.localScale.z);
            transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
            fallingCubeEdge += newXSize / 2f * direction;
        }
        else
        {
            fallingBlockXSize += newXSize;
            newXSize += Mathf.Abs(hangover);
            Destroy(gameObject);
            LastCube = null;
            CurrentCube = null;
        }

        float fallingBlockXPosition = fallingCubeEdge + fallingBlockXSize / 2f * direction;
        // todo refactor
        if (makeDropCube)
            SpawnDropCube(fallingBlockXPosition, fallingBlockXSize);
    }
    
    private void SplitCubeOnZ(float hangover, float direction)
    {
        var makeDropCube = true;
        float newZSize = LastCube.transform.localScale.z - Mathf.Abs(hangover);
        float fallingBlockZSize = transform.localScale.z - newZSize;
        if (fallingBlockZSize is <= Offset and >= 0)
        {
            fallingBlockZSize += newZSize;
            newZSize += Mathf.Abs(hangover);
            hangover = 0;
            makeDropCube = false;
            SolvePerfectQuad();
        }
        else
        {
            combo = 0;
            if (LastCube != rootCube)
                LastCubePerfectPos = LastCube.transform.position + new Vector3(0, 0.1f, + hangover / 2);
            else
            {
                LastCubePerfectPos = LastCube.transform.position + new Vector3(0, 0.5f, + hangover / 2);
            }
        }
        float fallingCubeEdge = transform.position.z;
        
        if (newZSize > 0)
        {
            float newZPosition = LastCube.transform.position.z + hangover / 2;
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newZSize);
            transform.position = new Vector3(transform.position.x, transform.position.y, newZPosition);
            fallingCubeEdge += newZSize / 2f * direction;
        }
        else
        {
            fallingBlockZSize += newZSize;
            newZSize += Mathf.Abs(hangover);
            Destroy(gameObject);
            LastCube = null;
            CurrentCube = null;
        }
        
        
        float fallingBlockZPosition = fallingCubeEdge + fallingBlockZSize / 2f * direction;

        // todo refactor
        if (makeDropCube)
            SpawnDropCube(fallingBlockZPosition, fallingBlockZSize);
    }

    private void SpawnDropCube(float fallingBlockPosition, float fallingBlockSize)
    {
        if (CurrentCube == rootCube)
        {
            return;
        }
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        if (MoveDirection == MoveDirection.Z)
        {
            cube.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, Mathf.Abs(fallingBlockSize));
            cube.transform.position = new Vector3(transform.position.x, transform.position.y, fallingBlockPosition);    
        }
        else
        {
            cube.transform.localScale = new Vector3(Mathf.Abs(fallingBlockSize), transform.localScale.y, transform.localScale.z);
            cube.transform.position = new Vector3(fallingBlockPosition, transform.position.y, transform.position.z);
        }
        

        cube.AddComponent<Rigidbody>();
        cube.GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color;
        Destroy(cube.gameObject, 1f);
    }


    void Update()
    {
        if (MoveDirection == MoveDirection.Z)
        {
            if (_forward)
                transform.position += transform.forward * (Time.deltaTime * moveSpeed);
            else 
                transform.position -= transform.forward * (Time.deltaTime * moveSpeed);
            if (Mathf.Abs(transform.position.z) >= LimitDistance)
            {
                _forward = !_forward;
            }
        }
        else
        {
            if (_forward)
                transform.position += transform.right * (Time.deltaTime * moveSpeed);
            else 
                transform.position -= transform.right * (Time.deltaTime * moveSpeed);
            if (Mathf.Abs(transform.position.x) >= LimitDistance)
            {
                _forward = !_forward;
            }
        }
    }

    private void SolvePerfectQuad()
    {
        combo++;
        if (CurrentCube == rootCube) return;
        var perfectEffectWidth = combo <= 5 ? combo * 0.008f : 0.04;
        LastCubePerfectPos += Vector3.up * 0.1f;
        var perfection = Instantiate(perfectQuad, LastCubePerfectPos + Vector3.down * 0.05f, Quaternion.identity);
        perfection.transform.localScale = transform.localScale * 0.1f + Vector3.one * (float)perfectEffectWidth;
        Destroy(perfection, 0.4f);
    }   

    // private IEnumerator AnimatePerfectSquad(GameObject perfectEffect)
    // {
    //     var maxScale = perfectEffect.transform.localScale * 0.11f;
    //     var minSide = Mathf.Min(perfectEffect.transform.localScale.x, perfectEffect.transform.localScale.z);
    //     var minSide2 = Mathf.Min(maxScale.x, maxScale.z);
    //     var buldeSpeed = 0.66f / Time.deltaTime;
    //     float timeElapsed = 0f;
    //     while (timeElapsed < 0.66f)
    //     {
    //         perfectEffect.transform.localScale +=  Vector3.one * 0.01f;
    //         timeElapsed += Time.deltaTime * 0.001f;
    //         yield return null;
    //     }
    //     Destroy(perfectEffect, 0.66f);
    // }
}
